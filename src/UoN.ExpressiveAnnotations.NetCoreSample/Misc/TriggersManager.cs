using System;
using Microsoft.AspNetCore.Http;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Misc
{
    public class TriggersManager
    {
        static TriggersManager()
        {
            Instance = new TriggersManager();
        }

        private TriggersManager()
        {
        }

        public static TriggersManager Instance { get; }

        public void Save(string events, HttpContext httpContext)
        {
            SetValueToCookie(events, httpContext);
        }

        public string Load(HttpContext httpContext)
        {
            var events = GetValueFromCookie(httpContext);
            if (events != null)
                return events;

            events = "change afterpaste keyup";
            SetValueToCookie(events, httpContext);
            return events;
        }

        private string GetValueFromCookie(HttpContext httpContext) => 
            httpContext.Request.Cookies.TryGetValue("expressiv.mvcwebsample.triggers", out var cookieValue) ? cookieValue : null;

        private void SetValueToCookie(string events, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("expressiv.mvcwebsample.triggers",
                events,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMonths(1)
                });
        }
    }
}
