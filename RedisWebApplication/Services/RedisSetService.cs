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
    public class RedisSetService
    {
        IRedisCacheClient cacheClient;
        ISerializer serializer;

        public RedisSetService(IRedisCacheClient _cacheClient, ISerializer _serializer)
        {
            cacheClient = _cacheClient;
            serializer = _serializer;
        }

        public async Task<long> SAdd<T>(string key, T[] values) where T : class
        {    
            //SADD
            var added = await cacheClient.Db0.SetAddAllAsync(key, CommandFlags.None, values);
            return added;
        }

        public async Task<IEnumerable<T>> SMembers<T>(string key)
        {
            //SMEMBERS
            var result = await cacheClient.Db0.SetMembersAsync<T>(key);
            return result;
        }
        public async Task<long> SetLength(string hashKey)
        {
            //SCARD
            var result = await cacheClient.Db0.Database.SetLengthAsync(hashKey);
            return result;
        }
        public IEnumerable<RedisValue> SScan(string key, int offset, int pagesize)
        {
            //SSCAN
            var result = cacheClient.Db0.Database.SetScan(key, pageOffset:offset );
            return result;
        }

    }
}
