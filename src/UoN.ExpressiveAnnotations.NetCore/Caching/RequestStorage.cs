/* https://github.com/uon-nuget/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 The University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using Microsoft.Extensions.Caching.Memory;

namespace UoN.ExpressiveAnnotations.NetCore.Caching
{
    /// <summary>
    ///     Persists arbitrary key-value pairs for the lifespan of the current HTTP request.
    /// </summary>
    public class RequestStorage
    {
        private readonly IMemoryCache _cache;

        public RequestStorage(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public T Get<T>(string key)
        {
            if (!_cache.TryGetValue(key, out T value))
            {
                value = default(T);
            }

            return value;
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
