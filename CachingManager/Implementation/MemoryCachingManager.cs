using CachingManager.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace CachingManager.Implementation
{
    internal class MemoryCachingManager : ICachingManager
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCachingManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task<T> GetDataAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T cachedData))
            {
                // Cache hit
                return Task.FromResult(cachedData);
            }

            // Cache miss
            return default;
        }

        public Task SetDataAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            // Get the current list of keys
            var trackedKeys = GetTrackedKeys();

            // Add the new key to the list
            trackedKeys.Add(key);

            // Store the updated list of keys in the cache
            UpdateTrackedKeys(trackedKeys);

            // Set the data in the cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiresIn
            };

            _memoryCache.Set(key, value, cacheEntryOptions);

            return Task.CompletedTask;
        }

        public Task DeleteByKeyAsync(string key)
        {
            // Get the current list of keys
            var trackedKeys = GetTrackedKeys();

            // Remove the key from the list
            trackedKeys.Remove(key);

            // Store the updated list of keys in the cache
            UpdateTrackedKeys(trackedKeys);

            // Remove the data from the cache
            _memoryCache.Remove(key);

            return Task.CompletedTask;
        }

        public Task<List<string>> GetAllKeysAsync()
        {
            // Get the current list of keys
            return Task.FromResult(GetTrackedKeys());
        }

        public Task<Dictionary<string, string>> GetAllAsync()
        {
            var allData = new Dictionary<string, string>();

            // Get the current list of keys
            var trackedKeys = GetTrackedKeys();

            // Retrieve data for each key in the list
            foreach (var key in trackedKeys)
            {
                if (_memoryCache.TryGetValue(key, out string value))
                {
                    allData.Add(key, value);
                }
            }

            return Task.FromResult(allData);
        }

        public Task TruncateAsync()
        {
            // Get the current list of keys
            var trackedKeys = GetTrackedKeys();

            // Clear all entries in the cache
            foreach (var key in trackedKeys)
            {
                _memoryCache.Remove(key);
            }

            // Clear the list of tracked keys
            UpdateTrackedKeys(new List<string>());

            return Task.CompletedTask;
        }

        private List<string> GetTrackedKeys()
        {
            // Retrieve the list of keys from the cache
            if (_memoryCache.TryGetValue("CachedKeys", out string keysJson))
            {
                return JsonSerializer.Deserialize<List<string>>(keysJson);
            }

            // If no keys are found, return an empty list
            return new List<string>();
        }

        private void UpdateTrackedKeys(List<string> keys)
        {
            // Store the updated list of keys in the cache
            var keysJson = JsonSerializer.Serialize(keys);
            _memoryCache.Set("CachedKeys", keysJson);
        }
    }
}
