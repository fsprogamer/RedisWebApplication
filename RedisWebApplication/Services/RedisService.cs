using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class RedisService
    {
        IRedisCacheClient cacheClient;
        ISerializer serializer;

        public RedisService(IRedisCacheClient _cacheClient, ISerializer _serializer)
        {
            cacheClient = _cacheClient;
            serializer = _serializer;
        }

        public async Task<bool> Set<T>(string key, T value)
        {
            //SET
            bool added = await cacheClient.Db0.AddAsync(key, value);            
            return added;
        }

        public async Task<bool> SetAll<T>(IList<Tuple<string, T>> values)
        {
            //SET
            bool added = await cacheClient.Db0.AddAllAsync(values);
            return added;
        }
        public async Task<T> Get<T>(string key)
        {            
            //GET
            return await cacheClient.Db0.GetAsync<T>(key);             

            //var db = _redis.GetDatabase();
            //return await db.StringGetAsync(key);
        }
        public async Task<IDictionary<string, T>> GetAll<T>(IEnumerable<string> keys)
        {            
            //GET
            return await cacheClient.Db0.GetAllAsync<T>(keys);
        }

    }

}
