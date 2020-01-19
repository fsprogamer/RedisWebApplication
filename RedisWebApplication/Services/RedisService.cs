using Microsoft.Extensions.Configuration;
using Serilog;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;

        private ConnectionMultiplexer _redis;

        public RedisService(IConfiguration config)
        {
            _redisHost = config["Redis:Host"];
            _redisPort = Convert.ToInt32(config["Redis:Port"]);
        }
        public void Connect()
        {
            try
            {
                var configString = $"{_redisHost}:{_redisPort},connectRetry=3";
                _redis = ConnectionMultiplexer.Connect(configString);
            }
            catch (RedisConnectionException err)
            {
                Log.Error(err.ToString());
                throw err;
            }
            Log.Debug("Connected to Redis");
        }
        public async Task<bool> Set(string key, string value)
        {
            var db = _redis.GetDatabase();
            return await db.StringSetAsync(key, value);
        }

        public async Task<string> Get(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }
    }
}
