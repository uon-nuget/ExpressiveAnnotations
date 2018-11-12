using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace UoN.ExpressiveAnnotations.NetCoreSample.Misc
{
    public class CultureManager
    {
        static CultureManager()
        {
            Instance = new CultureManager();
        }

        private CultureManager()
        {
        }

        public static CultureManager Instance { get; }

        public void Save(string lang, HttpContext httpContext)
        {
            var culture = CultureInfo.CreateSpecificCulture(lang);
            SetValueToCookie(culture, httpContext);
        }

        public CultureInfo Load(HttpContext httpContext)
        {
            var culture = GetValueFromCookie(httpContext);
            if (culture != null)
                return culture;

            culture = CultureInfo.CreateSpecificCulture("en"); // force default culture to be "en"
            SetValueToCookie(culture, httpContext);
            return culture;
        }

        private CultureInfo GetValueFromCookie(HttpContext httpContext) => 
            httpContext.Request.Cookies.TryGetValue("expressiv.mvcwebsample.culture", out var cookieValue)
            ? CultureInfo.CreateSpecificCulture(cookieValue)
            : null;

        private void SetValueToCookie(CultureInfo culture, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("expressiv.mvcwebsample.culture",
                culture.Name,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMonths(1)
                });
        }
    }
}
