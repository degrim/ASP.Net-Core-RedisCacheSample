using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using RedisSample.Models;
using System;

namespace RedisSample.Services
{
    public static class RedisCacheExtensions
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, RedisCredentials creds,
            int database = 0, TimeSpan? idleTimeout = null)
        {
            return services.AddInstance(typeof(IDistributedCache), new RedisCacheService(creds, database, idleTimeout));
        }
    }
}
