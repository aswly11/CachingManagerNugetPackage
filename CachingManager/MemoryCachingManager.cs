using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachingManager
{
    public class MemoryCacheManager : ICachingManager
    {
        private readonly IMemoryCache _memoryCache;

        private readonly HashSet<string> _trackedKeys;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _trackedKeys = new HashSet<string>();
        }

        public T GetCachedData<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T cachedData))
            {
                // Cache hit
                return cachedData;
            }

            // Cache miss
            return default(T);
        }

        public void SetCachedData<T>(string key, T value, TimeSpan expiresIn)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiresIn
            };

            _memoryCache.Set(key, value, cacheEntryOptions);
            _trackedKeys.Add(key);
        }

        public void DeleteCachedData(string key)
        {
            _memoryCache.Remove(key);
            _trackedKeys.Remove(key);
        }

        public List<string> GetAllKeys()
        {
            return new List<string>(_trackedKeys);
        }

        public Dictionary<string, string> GetAll()
        {
            var allData = new Dictionary<string, string>();

            foreach (var key in _trackedKeys)
            {
                if (_memoryCache.TryGetValue(key, out string value))
                {
                    allData.Add(key, value);
                }
            }

            return allData;
        }

        public void Truncate()
        {
            foreach (var key in _trackedKeys)
            {
                _memoryCache.Remove(key);
            }

            _trackedKeys.Clear();
        }
    }
}
