using MongoDB.Driver;
using RedisWebApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RedisWebApplication.Repositiries.Mongo
{
    public class BaseMongoRepository<T> : IMongoRepository<T> where T : BidEntity
    {
       static IMongoDatabase database;
       protected static string _collectionName { get; set; }
       public BaseMongoRepository(string connectionString, string databaseName, string collectionName)
        {
           // BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
            _collectionName = collectionName;
        }
        //public async Task<List<T>> GetByBidId(T entity)
        //{            
        //    var collection = database.GetCollection<T>(_collectionName);

        //    var result = await collection.Aggregate()           
        //   .Match(x => x.BidId == entity.BidId)
        //   .ToListAsync();
            
        //    return result;
        //}
        public async Task<List<T>> Get(Expression<Func<T, bool>> filter)
        {
            var collection = database.GetCollection<T>(_collectionName);

            var result = await collection.Aggregate()
            .Match(filter)
            .ToListAsync();

            return result;
        }
        public async Task Insert(T entity)
        {
            var collection = database.GetCollection<T>(_collectionName);
            await collection.InsertOneAsync(entity);
        }
        public async Task Insert(IEnumerable<T> values)
        {
            var collection = database.GetCollection<T>(_collectionName);
            await collection.InsertManyAsync(values);
        }
        public async Task<long> Delete(T entity)
        {
            var collection = database.GetCollection<T>(_collectionName);
            DeleteResult result = await collection.DeleteManyAsync(Builders<T>.Filter.Eq("bidId", entity.BidId));
            return result.DeletedCount;
        }
        public async Task<long> Delete(IEnumerable<T> entityList)
        {
            long count = 0;
            var collection = database.GetCollection<T>(_collectionName);
            foreach (var entity in entityList)
                count += (await collection.DeleteManyAsync(Builders<T>.Filter.Eq("bidId", entity.BidId))).DeletedCount;
            return count;
        }
    }
}
