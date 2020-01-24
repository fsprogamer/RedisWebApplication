using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RedisWebApplication.Model;

namespace RedisWebApplication.Repositiries.Mongo
{
    public class CostAttributesCalculatedDataRepo
    {
        static IMongoDatabase database;
        const string collectionName = "CostAttributesCalculatedData";
        public CostAttributesCalculatedDataRepo(string connectionString, string databaseName)
        {
           // BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }
        public static async Task<List<CalculatedElementData>> GetByBidId(int bidId)
        {
            var collection = database.GetCollection<CalculatedElementData>(collectionName);

            var result = await collection.Aggregate()           
           .Match(x => //x.BidCostGroupId == guid && 
                       x.CostingVersionId == bidId && 
                       x.AppliedFinancialFactors == 0 && 
                       x.AllocationId == BsonNull.Value 
                       )
           .ToListAsync();

            return result;
        }
        public static async Task Add(CalculatedElementData values)
        {
            var collection = database.GetCollection<CalculatedElementData>(collectionName);
            await collection.InsertOneAsync(values);
        }
        public static async Task Add(IEnumerable<CalculatedElementData> values)
        {
            var collection = database.GetCollection<CalculatedElementData>(collectionName);
            await collection.InsertManyAsync(values);
        }
        public static async Task Delete()
        {
            var collection = database.GetCollection<CalculatedElementData>(collectionName);
            await collection.DeleteManyAsync(Builders<CalculatedElementData>.Filter.And(
                                                               Builders<CalculatedElementData>.Filter.Eq("startYr", 2021),
                                                               Builders<CalculatedElementData>.Filter.Eq("startMth", 1)   
                                                               ));
        }
        public static async Task Delete(int id)
        {
            var collection = database.GetCollection<CalculatedElementData>(collectionName);
            await collection.DeleteManyAsync(Builders<CalculatedElementData>.Filter.Eq("bidId", id));
        }
    }
}
