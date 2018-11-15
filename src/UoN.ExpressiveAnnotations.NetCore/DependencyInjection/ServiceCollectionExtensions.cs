using Microsoft.Extensions.DependencyInjection;
using UoN.ExpressiveAnnotations.NetCore.Caching;

namespace UoN.ExpressiveAnnotations.NetCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddExpressiveAnnotations(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<RequestCache>();
        }
    }
}
