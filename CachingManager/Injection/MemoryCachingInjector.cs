using CachingManager.Implementation;
using CachingManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachingManager.Injection
{
    public static class MemoryCachingInjector
    {
        public static void AddInMemoryCaching(this IServiceCollection services)
        {
            services.AddSingleton<ICachingManager, MemoryCachingManager>();
        }
    }
}
