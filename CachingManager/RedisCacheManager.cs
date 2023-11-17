using CachingManager.interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CachingManager
{
    internal class RedisCacheManager : ICachingManager , ICachingManagerAsync
    {

        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheManager(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public bool DeleteByKey(string key)
        {
            try
            {
                _database.KeyDelete(key);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"error : {ex.Message}");
                return false;
            }            
        }

        public async Task<bool> DeleteByKeyAsync(string key)
        {
            try
            {
              await  _database.KeyDeleteAsync(key);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"error : {ex.Message}");
                return false;
            }
        }

        public Dictionary<string, string> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllKeys()
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                var keys = server.Keys();
                return keys.Select(key => (string)key).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(value: $"error : {ex.Message}");
                throw new NotImplementedException();

            }
        }

        public T GetData<T>(string key)
        {
            try
            {
                var serializedData =  _database.StringGet(key);
                if (serializedData.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(serializedData, _jsonOptions);
            }
            catch (Exception ex)
            {

                Console.WriteLine(value: $"error : {ex.Message}");
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
        }

        public void SetData<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
                  _database.StringSet(key, serializedData, expiresIn);
            }
            catch (Exception ex)
            {

                Console.WriteLine(value: $"error : {ex.Message}");
                throw new NotImplementedException();
            }
        }

        public async Task SetDataAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
              await  _database.StringSetAsync(key, serializedData, expiresIn);
            }
            catch (Exception ex)
            {

                Console.WriteLine(value: $"error : {ex.Message}");
                throw new NotImplementedException();
            }
        }

        public bool Truncate()
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys();

            foreach (var key in keys)
            {
                 _database.KeyDelete(key);
            }

            return true;
        }
    }
}
