using CachingManager.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CachingManager.Implementation
{
	public class RedisCachingManager : ICachingManagerAsync
	{
		private readonly IDatabase _database;
		private readonly JsonSerializerOptions _jsonOptions;

		private TimeSpan? _expiresIn { get; set; }

		/// <summary>
		/// Initializes a new instance of the RedisCachingManager class.
		/// </summary>
		/// <param name="connectionMultiplexer">The Redis connection multiplexer.</param>
		/// <param name="expiresIn">Optional expiration time for cached items. Defaults to one hour if not specified.</param>
		public RedisCachingManager(IConnectionMultiplexer connectionMultiplexer, TimeSpan? expiresIn)
		{
			_database = connectionMultiplexer.GetDatabase();
			_jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				WriteIndented = true
			};
			_expiresIn = expiresIn ?? TimeSpan.FromHours(1);
		}

		/// <summary>
		/// Retrieves all key-value pairs from the cache.
		/// </summary>
		/// <returns>A dictionary containing all key-value pairs in the cache.</returns>
		public Task<Dictionary<string, string>> GetAllAsync()
		{
			var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
			var keys = server.Keys();
			var allKeys = keys.Select(key => (string)key).ToList();
			var GetAllDictionary = new Dictionary<string, string>();
			foreach (var key in allKeys)
			{
				GetAllDictionary.Add(key, _database.StringGet(key));
			}
			return Task.FromResult(GetAllDictionary);
		}

		/// <summary>
		/// Retrieves all keys from the cache.
		/// </summary>
		/// <returns>A list containing all keys in the cache.</returns>
		public Task<List<string>> GetAllKeysAsync()
		{
			try
			{
				var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
				var keys = server.Keys();
				var listkeys = keys.Select(key => (string)key).ToList();
				return Task.FromResult(listkeys);
			}
			catch (Exception ex)
			{
				Console.WriteLine(value: $"error : {ex.Message}");
				throw ex;
			}
		}

		/// <summary>
		/// Retrieves data of type T from the cache based on the specified key.
		/// </summary>
		/// <typeparam name="T">The type of data to retrieve.</typeparam>
		/// <param name="key">The key associated with the cached data.</param>
		/// <returns>The cached data of type T.</returns>
		public async Task<T> GetDataAsync<T>(string key)
		{
			try
			{
				var serializedData = await _database.StringGetAsync(key);
				if (serializedData.IsNullOrEmpty)
					return default;

				return JsonSerializer.Deserialize<T>(serializedData, _jsonOptions);
			}
			catch (Exception ex)
			{
				Console.WriteLine(value: $"error : {ex.Message}");
				throw ex;
			}
		}

		/// <summary>
		/// Stores data of type T in the cache with the specified key and expiration time.
		/// </summary>
		/// <typeparam name="T">The type of data to store in the cache.</typeparam>
		/// <param name="key">The key to associate with the cached data.</param>
		/// <param name="value">The data of type T to store in the cache.</param>
		/// <param name="expiresIn">The expiration time for the cached data.</param>
		public async Task SetDataAsync<T>(string key, T value, TimeSpan expiresIn)
		{
			try
			{
				var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
				await _database.StringSetAsync(key, serializedData, expiresIn);
			}
			catch (Exception ex)
			{
				Console.WriteLine(value: $"error : {ex.Message}");
				throw ex;
			}
		}

		/// <summary>
		/// Truncates (clears) all data in the cache.
		/// </summary>
		/// <returns>A completed task once the truncation is complete.</returns>
		public Task TruncateAsync()
		{
			var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
			var keys = server.Keys();

			foreach (var key in keys)
			{
				_database.KeyDelete(key);
			}
			return Task.CompletedTask;
		}

		/// <summary>
		/// Deletes data from the cache based on the specified key.
		/// </summary>
		/// <param name="key">The key associated with the data to delete.</param>
		public async Task DeleteByKeyAsync(string key)
		{
			try
			{
				await _database.KeyDeleteAsync(key);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"error : {ex.Message}");
				throw ex;
			}
		}

		/// <summary>
		/// Stores data of type T in the cache with the specified key and default expiration time.
		/// </summary>
		/// <typeparam name="T">The type of data to store in the cache.</typeparam>
		/// <param name="key">The key to associate with the cached data.</param>
		/// <param name="value">The data of type T to store in the cache.</param>
		public async Task SetDataAsync<T>(string key, T value)
		{
			try
			{
				var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
				await _database.StringSetAsync(key, serializedData, _expiresIn);
			}
			catch (Exception ex)
			{
				Console.WriteLine(value: $"error : {ex.Message}");
				throw ex;
			}
		}
	}
}