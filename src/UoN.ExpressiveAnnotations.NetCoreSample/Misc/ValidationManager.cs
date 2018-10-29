using System;
using Microsoft.AspNetCore.Http;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Misc
{
    public class ValidationManager
    {
        static ValidationManager()
        {
            Instance = new ValidationManager();
        }

        private ValidationManager()
        {
        }

        public static ValidationManager Instance { get; }

        public void Save(string type, HttpContext httpContext)
        {
            SetValueToCookie(type, httpContext);
        }

        public string Load(HttpContext httpContext)
        {
            var type = GetValueFromCookie(httpContext);
            if (type != null)
                return type;

            type = "client";
            SetValueToCookie(type, httpContext);
            return type;
        }

        private string GetValueFromCookie(HttpContext httpContext) => 
            httpContext.Request.Cookies.TryGetValue("expressiv.mvcwebsample.validation", out var cookieValue) ? cookieValue : null;

        private void SetValueToCookie(string type, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("expressiv.mvcwebsample.validation",
                type,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMonths(1)
                });
        }
    }
}
