using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shortify.NET.Applicaion.Abstractions;
using System.Collections.Concurrent;

namespace Shortify.NET.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="ICachingServices"/> that provides caching functionality
    /// using an <see cref="IDistributedCache"/>.
    /// </summary>
    public class CachingServices(IDistributedCache distributedCache) : ICachingServices
    {
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();

        private readonly IDistributedCache _distributedCache = distributedCache;

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(
            string key, 
            CancellationToken cancellationToken = default) 
            where T : class
        {
            var cachedValue = await _distributedCache
                                        .GetStringAsync(key, cancellationToken);

            return cachedValue is null ? 
                        null : 
                        JsonConvert.DeserializeObject<T>(cachedValue);
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(
            string key, 
            Func<Task<T>> factory, 
            CancellationToken cancellationToken = default) 
            where T : class
        {
            T? cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            await SetAsync(key, cachedValue, cancellationToken);

            return cachedValue;
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(
            string key, 
            T value, 
            CancellationToken cancellationToken = default) 
            where T : class
        {
            await _distributedCache
                        .SetStringAsync(
                                key,
                                JsonConvert.SerializeObject(value),
                                cancellationToken);

            CacheKeys.TryAdd(key, true);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(
            string key, 
            CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);

            CacheKeys.TryRemove(key, out bool _);
        }

        /// <inheritdoc/>
        public async Task RemoveByPrefixAsync(
            string prefix, 
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Task> removeTasks = CacheKeys
                                                .Keys
                                                .Where(k => k.StartsWith(prefix))
                                                .Select(k => RemoveAsync(k, cancellationToken));

            await Task.WhenAll(removeTasks);
        }
    }
}