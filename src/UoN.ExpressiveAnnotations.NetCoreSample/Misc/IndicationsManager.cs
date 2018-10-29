using System;
using Microsoft.AspNetCore.Http;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Misc
{
    public class IndicationsManager
    {
        static IndicationsManager()
        {
            Instance = new IndicationsManager();
        }

        private IndicationsManager()
        {
        }

        public static IndicationsManager Instance { get; }

        public void Save(string value, HttpContext httpContext)
        {
            SetValueToCookie(value, httpContext);
        }

        public string Load(HttpContext httpContext)
        {
            var indication = GetValueFromCookie(httpContext);
            if (indication != null)
                return indication;

            indication = "asterisks"; // asterisks by default
            SetValueToCookie(indication, httpContext);
            return indication;
        }

        private string GetValueFromCookie(HttpContext httpContext) =>
            httpContext.Request.Cookies.TryGetValue("expressiv.mvcwebsample.indication", out var cookieValue) ? cookieValue : null;

        private void SetValueToCookie(string value, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("expressiv.mvcwebsample.indication",
                value,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMonths(1)
                });
        }
    }
}