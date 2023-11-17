using CachingManager.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CachingManager.Implementation
{
    internal class RedisCachingManager : ICachingManager
    {
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCachingManager(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }
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
    }
}
