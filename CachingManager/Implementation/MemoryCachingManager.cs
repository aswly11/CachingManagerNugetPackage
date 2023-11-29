using CachingManager.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace CachingManager.Implementation
{
	/// <summary>
	/// Implementation of the ICachingManagerAsync interface using an in-memory cache provided by Microsoft.Extensions.Caching.Memory.
	/// </summary>
	public class MemoryCachingManager : ICachingManagerAsync
	{
		private readonly IMemoryCache _memoryCache;

		/// <summary>
		/// Initializes a new instance of the MemoryCachingManager class.
		/// </summary>
		/// <param name="memoryCache">The in-memory cache implementation.</param>
		/// <param name="expiresIn">Optional expiration time for cached items. Defaults to one hour if not specified.</param>
		public MemoryCachingManager(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		
		}

		/// <summary>
		/// Retrieves data of type T from the in-memory cache based on the specified key.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="key">The key associated with the cached data.</param>
		/// <returns>The cached data of type T if present; otherwise, the default value of T.</returns>
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

		/// <summary>
		/// Stores data of type T in the in-memory cache with the specified key and expiration time.
		/// </summary>
		/// <typeparam name="T">The type of data to store in the cache.</typeparam>
		/// <param name="key">The key to associate with the cached data.</param>
		/// <param name="value">The data of type T to store in the cache.</param>
		/// <param name="expiresIn">The expiration time for the cached data.</param>
		/// <returns>A completed task once the data is stored in the cache.</returns>
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

		/// <summary>
		/// Deletes data from the in-memory cache based on the specified key.
		/// </summary>
		/// <param name="key">The key associated with the data to delete.</param>
		/// <returns>A completed task once the data is deleted from the cache.</returns>
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

		/// <summary>
		/// Retrieves all keys from the in-memory cache.
		/// </summary>
		/// <returns>A list containing all keys in the cache.</returns>
		public Task<List<string>> GetAllKeysAsync()
		{
			// Get the current list of keys
			return Task.FromResult(GetTrackedKeys());
		}

		/// <summary>
		/// Retrieves all key-value pairs from the in-memory cache.
		/// </summary>
		/// <returns>A dictionary containing all key-value pairs in the cache.</returns>
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

		/// <summary>
		/// Clears all data from the in-memory cache.
		/// </summary>
		/// <returns>A completed task once the cache is cleared.</returns>
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

		/// <summary>
		/// Stores data of type T in the in-memory cache with the specified key and default expiration time.
		/// </summary>
		/// <typeparam name="T">The type of data to store in the cache.</typeparam>
		/// <param name="key">The key to associate with the cached data.</param>
		/// <param name="value">The data of type T to store in the cache.</param>
		/// <returns>A completed task once the data is stored in the cache.</returns>
		public Task SetDataAsync<T>(string key, T value)
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
				AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
			};

			_memoryCache.Set(key, value, cacheEntryOptions);

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