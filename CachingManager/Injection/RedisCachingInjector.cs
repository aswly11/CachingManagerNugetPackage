using CachingManager.Implementation;
using CachingManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CachingManager.Injection
{
    public static class RedisCachingInjector
    {
        public static void AddInRedisCaching(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                var configure = ConfigurationOptions.Parse(connectionString, true);
                return ConnectionMultiplexer.Connect(configure);
            });

            //services.AddSingleton<ICachingManagerAsync, RedisCachingManager>();

        }
    }
}
