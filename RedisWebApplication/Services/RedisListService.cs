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
    public class RedisListService
    {
        IRedisCacheClient cacheClient;
        ISerializer serializer;

        public RedisListService(IRedisCacheClient _cacheClient, ISerializer _serializer)
        {
            cacheClient = _cacheClient;
            serializer = _serializer;
        }

        public async Task<long> LPush<T>(string key, T[] values) where T : class
        {
            //LPUSH
            Parallel.ForEach(values, new ParallelOptions(){ MaxDegreeOfParallelism = 1},
            async value =>
            {
                await cacheClient.Db0.ListAddToLeftAsync<T>(key, value );
            });

            //foreach(var value in values)
            // await cacheClient.Db0.ListAddToLeftAsync<T>(key, value );
            return 0;
        }
        internal async Task AddToList<T>(string key, T[] elements) where T : class
        {
            //LPUSH
            foreach(var element in elements)
             await cacheClient.Db0.ListAddToLeftAsync(key, element);            
        }
        public async Task<RedisValue[]> LRangeAsync<T>(string key, long start, long stop)
        {
            //LRANGE
            var result = await cacheClient.Db0.Database.ListRangeAsync(key, start, stop);
            return result;
        }
        public IEnumerable<CalculatedElementData> LRange<T>(string key)
        {
            //LRANGE
            int chunk_size = 1000;            
            Log.Information($"Get costattribute list, begin");

            var length = cacheClient.Db0.Database.ListLength("list_key");
            int chunk_count = (int)Math.Floor((decimal)length / chunk_size);

            var partitoner = Partitioner.Create(0, length, chunk_size);
            RedisValue[] bag = new RedisValue[length];

            Parallel.ForEach(partitoner, new ParallelOptions() { MaxDegreeOfParallelism = 4 },
            value =>
            {
                cacheClient.Db0.Database.ListRange("list_key", value.Item1, value.Item2).CopyTo(bag, value.Item1);
            });

            var result = bag.Select(c => serializer.Deserialize<CalculatedElementData>(c)).ToList();

            //for (int i = 0; i < chunk_count; i++)
            //{
            //    var chunk_length = (i == chunk_count) ? (length % chunk_size) : chunk_size;
            //    var result = await _redisService.LRangeAsync<CalculatedElementData>("list_key", i * chunk_size, (i + 1) * chunk_size - 1);
            //}
                        
            return result;
        }
        public async Task<long> LLen(string Key)
        {
            //LLEN
            var result = await cacheClient.Db0.Database.ListLengthAsync(Key);
            return result;
        }

        //------------------------------------------------------
    }
}
