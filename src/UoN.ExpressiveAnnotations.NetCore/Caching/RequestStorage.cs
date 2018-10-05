/* https://github.com/MmmBerry/ExpressiveAnnotations
 * Original work Copyright (c) 2014 Jarosław Waliszko
 * Modified work Copyright (c) 2018 University of Nottingham
 * Licensed MIT: http://opensource.org/licenses/MIT */

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace UoN.ExpressiveAnnotations.NetCore.Caching
{
    /// <summary>
    ///     Persists arbitrary key-value pairs for the lifespan of the current HTTP request.
    /// </summary>
    public static class RequestStorage
    {
        private static IHttpContextAccessor _accessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }

        public static HttpContext HttpContext => _accessor.HttpContext;

        private static IDictionary<object, object> Items
        {
            get
            {
                if (HttpContext == null)
                    throw new ApplicationException("HttpContext not available.");
                return HttpContext.Items; // location that could be used throughtout the entire HTTP request lifetime
            }                                             // (contrary to a session, this one exists only within the period of a single request).
        }

        public static T Get<T>(string key)
        {
            return Items[key] == null
                ? default(T)
                : (T) Items[key];
        }

        public static void Set<T>(string key, T value)
        {
            Items[key] = value;
        }

        public static void Remove(string key)
        {
            Items.Remove(key);
        }
    }
}
