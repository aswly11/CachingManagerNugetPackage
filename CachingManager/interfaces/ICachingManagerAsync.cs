namespace CachingManager.Interfaces
{
	public interface ICachingManagerAsync
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

		/// Asynchronously sets cached data of type T with the specified key and expiration time.
		/// </summary>
		/// <typeparam name="T">The type of data to cache.</typeparam>
		/// <param name="key">The unique key to associate with the cached data.</param>
		/// <param name="value">The data to be cached.</param>
		Task SetDataAsync<T>(string key, T value);

		/// <summary>
		/// Asynchronously deletes cached data associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the data to be deleted from the cache.</param>
		Task DeleteByKeyAsync(string key);

		/// <summary>
		/// Asynchronously gets a list of all keys present in the cache.
		/// </summary>
		/// <returns>A list of all keys in the cache.</returns>
		Task<List<string>> GetAllKeysAsync();

		/// <summary>
		/// Asynchronously gets a dictionary containing all key-value pairs present in the cache.
		/// </summary>
		/// <returns>A dictionary containing all key-value pairs in the cache.</returns>
		Task<Dictionary<string, string>> GetAllAsync();

		/// <summary>
		/// Asynchronously removes all items from the cache, effectively truncating it.
		/// </summary>
		Task TruncateAsync();
	}
}