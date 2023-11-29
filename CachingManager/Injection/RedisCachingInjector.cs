using CachingManager.Implementation;
using CachingManager.Interfaces;
using CachingManager.Models;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CachingManager.Injection
{
	public static class RedisCachingInjector
	{
		public static void AddRedisCaching(this IServiceCollection services, CachingManagerOptions cachingManagerOptions)
		{
			var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
			{
				EndPoints = { cachingManagerOptions.ConnectionString },
				AbortOnConnectFail = false,
			});
			services.AddSingleton<ICachingManagerAsync>(new RedisCachingManager(connectionMultiplexer, cachingManagerOptions.ExpiresIn));
		}
	}
}