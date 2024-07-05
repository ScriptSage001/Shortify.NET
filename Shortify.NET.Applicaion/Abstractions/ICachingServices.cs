namespace Shortify.NET.Applicaion.Abstractions
{
    /// <summary>
    /// Defines the interface for caching services, providing methods for getting, setting,
    /// and removing cached items.
    /// </summary>
    public interface ICachingServices
    {
        /// <summary>
        /// Retrieves a cached item by its key.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The key of the cached item.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the cached item,
        /// or null if the item does not exist.
        /// </returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Retrieves a cached item by its key. If the item does not exist, it is created using the specified factory function
        /// and added to the cache.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The key of the cached item.</param>
        /// <param name="factory">A function to create the item if it does not exist in the cache.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the cached item.
        /// </returns>
        Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Adds or updates a cached item with the specified key and value.
        /// </summary>
        /// <typeparam name="T">The type of the item to be cached.</typeparam>
        /// <param name="key">The key of the cached item.</param>
        /// <param name="value">The value of the cached item.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <param name="absoluteExpirationRelativeToNow">The absolute expiration time relative to now.</param>
        /// <param name="slidingExpiration">The sliding expiration time.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetAsync<T>(
            string key, 
            T value, 
            CancellationToken cancellationToken = default, 
            TimeSpan? absoluteExpirationRelativeToNow = null, 
            TimeSpan? slidingExpiration = null)
            where T : class;

        /// <summary>
        /// Removes a cached item by its key.
        /// </summary>
        /// <param name="key">The key of the cached item to remove.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes cached items by their key prefix.
        /// </summary>
        /// <param name="prefix">The prefix of the keys of the cached items to remove.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all the cached items.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ClearAllAsync(CancellationToken cancellationToken = default);
    }
}