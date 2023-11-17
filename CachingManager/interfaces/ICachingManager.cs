using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachingManager.interfaces
{
    /// <summary>
    /// Interface for a cache manager responsible for handling caching operations.
    /// </summary>
    public interface ICachingManager
    {
        /// <summary>
        /// Gets cached data of type T based on the provided key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve from the cache.</typeparam>
        /// <param name="key">The unique key associated with the cached data.</param>
        /// <returns>The cached data of type T if available; otherwise, the default value for type T.</returns>
        T GetData<T>(string key);

        /// <summary>
        /// Sets cached data of type T with the specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of data to cache.</typeparam>
        /// <param name="key">The unique key to associate with the cached data.</param>
        /// <param name="value">The data to be cached.</param>
        /// <param name="expiresIn">The duration for which the data should be cached before expiration.</param>
        void SetData<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Deletes cached data associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the data to be deleted from the cache.</param>
        bool DeleteByKey(string key);

        /// <summary>
        /// Gets a list of all keys present in the cache.
        /// </summary>
        /// <returns>A list of all keys in the cache.</returns>
        List<string> GetAllKeys();

        /// <summary>
        /// Gets a dictionary containing all key-value pairs present in the cache.
        /// </summary>
        /// <returns>A dictionary containing all key-value pairs in the cache.</returns>
        Dictionary<string, string> GetAll();

        /// <summary>
        /// Removes all items from the cache, effectively truncating it.
        /// </summary>
        bool Truncate();
    }
}
