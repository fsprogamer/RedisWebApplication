using Microsoft.Extensions.Configuration;
using Serilog;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class RedisService
    {
        IRedisCacheClient cacheClient;
        public RedisService(IRedisCacheClient _cacheClient)
        {
            cacheClient = _cacheClient;
        }
        public void Connect()
        {

        }
        public async Task<bool> Set(string key, User value)
        {
            bool added = await cacheClient.Db0.AddAsync(key, value, DateTimeOffset.Now.AddMinutes(10));            
            return added;
        }

        public async Task<bool> SetCollection(IList<Tuple<string, User>> values)
        {
            bool added = await cacheClient.Db0.AddAllAsync(values);
            return added;
        }

        public async Task SetHashSet(RedisKey hashKey, HashEntry[] values)
        {
            await cacheClient.Db0.Database.HashSetAsync(hashKey, values);
                //.HashSetAsync(values);            
        }

        internal async Task<HashEntry[]> HashGetAllAsync(RedisKey hashKey)
        {
            return await cacheClient.Db0.Database.HashGetAllAsync(hashKey);
        }
        public async Task<string> Get(string key)
        {            
            var cachedUser = await cacheClient.Db0.GetAsync<User>("my cache key"); 
            return string.Empty;

            //var db = _redis.GetDatabase();
            //return await db.StringGetAsync(key);
        }

    }

    public class User
    {
       public long Id { get;set;}
       public string Firstname { get;set;}
       public string Lastname { get;set;}
       public string Twitter { get;set;}
       public string Blog { get;set;}
    }
}
