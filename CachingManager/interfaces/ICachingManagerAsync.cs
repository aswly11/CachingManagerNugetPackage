using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachingManager.interfaces
{
    internal interface ICachingManagerAsync
    {
        /// <summary>
        /// Asynchronously retrieves cached data of type T based on the provided key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve from the cache.</typeparam>
        /// <param name="key">The unique key associated with the cached data.</param>
        /// <returns>The cached data of type T if available; otherwise, the default value for type T.</returns>
        Task<T> GetDataAsync<T>(string key);

        /// <summary>
        /// Asynchronously sets cached data of type T with the specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of data to cache.</typeparam>
        /// <param name="key">The unique key to associate with the cached data.</param>
        /// <param name="value">The data to be cached.</param>
        /// <param name="expiresIn">The duration for which the data should be cached before expiration.</param>
        Task SetDataAsync<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Asynchronously deletes cached data associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the data to be deleted from the cache.</param>
        Task<bool> DeleteByKeyAsync(string key);
    }

}
