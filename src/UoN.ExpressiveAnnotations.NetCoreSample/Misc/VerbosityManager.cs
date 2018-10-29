using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Misc
{
    public class VerbosityManager
    {
        static VerbosityManager()
        {
            Instance = new VerbosityManager();
        }

        private VerbosityManager()
        {
        }

        public static VerbosityManager Instance { get; }

        public void Save(bool value, HttpContext httpContext)
        {
            SetValueToCookie(value, httpContext);
        }

        public bool Load(HttpContext httpContext)
        {
            var verbose = GetValueFromCookie(httpContext);
            if (verbose != null)
                return verbose.Value;

            verbose = IsDebug();
            SetValueToCookie(verbose.Value, httpContext);
            return verbose.Value;
        }

        private bool? GetValueFromCookie(HttpContext httpContext)
        {
            if (!httpContext.Request.Cookies.TryGetValue("expressiv.mvcwebsample.verbosity", out var cookieValue))
            {
                return null;
            }

            bool.TryParse(cookieValue, out var result);
            return result;
        }

        private void SetValueToCookie(bool value, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("expressiv.mvcwebsample.verbosity", 
                value.ToString().ToLowerInvariant(), 
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMonths(1)
                });
        }

        private static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}