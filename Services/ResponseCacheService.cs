using WarSim.Services;

namespace WarSim.Services
{
    /// <summary>
    /// Generic response caching service for API controllers.
    /// Provides tick-based caching for world state dependent data.
    /// </summary>
    public class ResponseCacheService
    {
        private readonly WorldStateService _worldState;
        private readonly Dictionary<string, (long tick, object data)> _cache = new();
        private readonly object _cacheLock = new();

        public ResponseCacheService(WorldStateService worldState)
        {
            _worldState = worldState;
        }

        /// <summary>
        /// Get or compute a cached response based on current world tick.
        /// If cached data exists for the current tick, returns it immediately.
        /// Otherwise, executes the factory function and caches the result.
        /// </summary>
        /// <typeparam name="T">Type of data to cache</typeparam>
        /// <param name="cacheKey">Unique cache key (e.g., "movement_snapshot" or "unit_details_123")</param>
        /// <param name="factory">Function to create the data if not cached</param>
        /// <returns>Cached or newly created data</returns>
        public T GetOrCreate<T>(string cacheKey, Func<T> factory)
        {
            var snapshot = _worldState.GetSnapshot();

            lock (_cacheLock)
            {
                if (_cache.TryGetValue(cacheKey, out var cached) && cached.tick == snapshot.Tick)
                {
                    return (T)cached.data;
                }

                var data = factory();
                _cache[cacheKey] = (snapshot.Tick, data!);

                return data;
            }
        }

        /// <summary>
        /// Get or compute a cached response with explicit tick parameter.
        /// Useful when snapshot is already retrieved in the controller.
        /// </summary>
        public T GetOrCreate<T>(string cacheKey, long currentTick, Func<T> factory)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(cacheKey, out var cached) && cached.tick == currentTick)
                {
                    return (T)cached.data;
                }

                var data = factory();
                _cache[cacheKey] = (currentTick, data!);

                return data;
            }
        }

        /// <summary>
        /// Get or compute a permanent cache entry (not tick-based).
        /// Useful for static data like map definitions.
        /// </summary>
        /// <param name="cacheKey">Unique cache key</param>
        /// <param name="factory">Function to create the data if not cached</param>
        /// <returns>Cached or newly created data</returns>
        public T GetOrCreatePermanent<T>(string cacheKey, Func<T> factory)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(cacheKey, out var cached))
                {
                    return (T)cached.data;
                }

                var data = factory();
                _cache[cacheKey] = (-1, data!); // -1 tick = permanent

                return data;
            }
        }

        /// <summary>
        /// Invalidate a specific cache entry.
        /// </summary>
        public void Invalidate(string cacheKey)
        {
            lock (_cacheLock)
            {
                _cache.Remove(cacheKey);
            }
        }

        /// <summary>
        /// Invalidate multiple cache entries by prefix.
        /// </summary>
        public void InvalidateByPrefix(string prefix)
        {
            lock (_cacheLock)
            {
                var keysToRemove = _cache.Keys.Where(k => k.StartsWith(prefix)).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// Clear all cached entries.
        /// </summary>
        public void ClearAll()
        {
            lock (_cacheLock)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// Get cache statistics for monitoring.
        /// </summary>
        public CacheStatistics GetStatistics()
        {
            lock (_cacheLock)
            {
                return new CacheStatistics
                {
                    TotalEntries = _cache.Count,
                    TickBasedEntries = _cache.Count(c => c.Value.tick >= 0),
                    PermanentEntries = _cache.Count(c => c.Value.tick == -1)
                };
            }
        }
    }

    public class CacheStatistics
    {
        public int TotalEntries { get; set; }
        public int TickBasedEntries { get; set; }
        public int PermanentEntries { get; set; }
    }
}
