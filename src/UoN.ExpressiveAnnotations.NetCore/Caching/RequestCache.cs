using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace UoN.ExpressiveAnnotations.NetCore.Caching
{
    // Request Cache class enables us to have a separate MemoryCache which
    // can be scoped to the request.

    public class RequestCache : MemoryCache
    {
        public RequestCache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}
