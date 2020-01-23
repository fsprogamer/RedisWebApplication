using Microsoft.Extensions.Configuration;
using RedisWebApplication.Model;
using Serilog;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class RedisHashSetService
    {
        IRedisCacheClient cacheClient;
        ISerializer serializer;

        public RedisHashSetService(IRedisCacheClient _cacheClient, ISerializer _serializer)
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

        public async Task<bool> SetCollection<T>(IList<Tuple<string, T>> values)
        {
            //SET
            bool added = await cacheClient.Db0.AddAllAsync(values);
            return added;
        }


        public async Task<IDictionary<string,T>> HGetAll<T>(IEnumerable<string> keys)
        {
            //HGETALL
            var result = await cacheClient.Db0.GetAllAsync<T>(keys);
            return result;
        }

        public async Task SetHashSet(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                RedisKey hashKey = $"user.{user.Id}";
                HashEntry[] values = { new HashEntry("Firstname", user.Firstname),
                                          new HashEntry("Lastname", user.Lastname),
                                          new HashEntry("Twitter", user.Twitter),
                                          new HashEntry("Blog", user.Blog)
                                        };
                await cacheClient.Db0.Database.HashSetAsync(hashKey, values);
            }
        }

        internal async Task<HashEntry[]> HashGetAllAsync(RedisKey hashKey)
        {
            return await cacheClient.Db0.Database.HashGetAllAsync(hashKey);
        }
        public async Task<T> Get<T>(string key)
        {            
            return await cacheClient.Db0.GetAsync<T>(key);             

            //var db = _redis.GetDatabase();
            //return await db.StringGetAsync(key);
        }

        internal async Task AddToList<T>(string key, T[] elements) where T : class
        {
            foreach(var element in elements)
             await cacheClient.Db0.ListAddToLeftAsync(key, element);            
        }
    }

}
