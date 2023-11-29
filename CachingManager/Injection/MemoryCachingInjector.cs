using CachingManager.Implementation;
using CachingManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CachingManager.Injection
{
    public static class MemoryCachingInjector
    {
        public static void AddInMemoryCaching(this IServiceCollection services)
        {
            services.AddMemoryCache(); // Add memory caching services

            services.AddSingleton<ICachingManagerAsync, MemoryCachingManager>();
        }
    }
}
