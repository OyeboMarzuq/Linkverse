using Linkverse.Application.Interfaces.IServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly HashSet<string> _keys = new();
        private readonly Lock _keyLock = new();

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan ttl,
            CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(key, out T? cached) && cached is not null)
            {
                _logger.LogDebug("Cache HIT — key: {Key}", key);
                return cached;
            }

            _logger.LogDebug("Cache MISS — key: {Key}. Fetching from source.", key);

            T result = await factory();
            Set(key, result, ttl);
            return result;
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl,
                // Evict this entry under memory pressure before shorter-lived entries
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(key, value, options);

            lock (_keyLock)
            {
                _keys.Add(key);
            }

            _logger.LogDebug("Cache SET — key: {Key}, TTL: {TTL}", key, ttl);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);

            lock (_keyLock)
            {
                _keys.Remove(key);
            }

            _logger.LogDebug("Cache REMOVE — key: {Key}", key);
        }

        public void RemoveByPrefix(string prefix)
        {
            List<string> matchingKeys;

            lock (_keyLock)
            {
                matchingKeys = _keys
                    .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            foreach (var key in matchingKeys)
            {
                _cache.Remove(key);
            }

            lock (_keyLock)
            {
                foreach (var key in matchingKeys)
                    _keys.Remove(key);
            }

            _logger.LogDebug("Cache REMOVE BY PREFIX — prefix: {Prefix}, removed {Count} entries",
                prefix, matchingKeys.Count);
        }
    }
}
