using Microsoft.Extensions.Caching.Distributed;
using RedisSample.Models;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisSample.Services
{
    public class RedisCacheService : IDistributedCache
    {
        private readonly string _connectionString;
        private readonly int _databaseIndex;
        private readonly TimeSpan? _idleTimeout;
        private IDatabase _redisDb;

        public RedisCacheService(RedisCredentials creds, int databaseIndex, TimeSpan? idleTimeout)
        {
            _connectionString = string.Format("{0}:{1},password={2}",
                        creds.Hostname,
                        Int32.Parse(creds.Port),
                        creds.Password);
            _databaseIndex = databaseIndex;
            _idleTimeout = idleTimeout;
        }

        public void Connect()
        {
            var redis = ConnectionMultiplexer.Connect(_connectionString);
            _redisDb = redis.GetDatabase(_databaseIndex);
        }

        public Task ConnectAsync()
        {
            return Task.Factory.StartNew(async () =>
            {
                var redis = await ConnectionMultiplexer.ConnectAsync(_connectionString);
                _redisDb = redis.GetDatabase(_databaseIndex);
            });
        }

        public byte[] Get(string key)
        {
            return _redisDb.StringGet(key);
        }

        public async Task<byte[]> GetAsync(string key)
        {
            return await _redisDb.StringGetAsync(key);
        }

        public void Refresh(string key)
        {
            _redisDb.KeyExpire(key, _idleTimeout);
        }

        public Task RefreshAsync(string key)
        {
            return Task.Factory.StartNew(async () =>
            {
                await _redisDb.KeyExpireAsync(key, _idleTimeout);
            });
        }

        public void Remove(string key)
        {
            _redisDb.KeyDelete(key);
        }

        public Task RemoveAsync(string key)
        {
            return Task.Factory.StartNew(async () =>
            {
                await _redisDb.KeyDeleteAsync(key);
            });
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _redisDb.StringSet(key, value, options.SlidingExpiration);
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            await _redisDb.StringSetAsync(key, value, options.SlidingExpiration);
        }
    }
}
