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
        public CostAttributesCalculatedDataRepo(string connectionString, string databaseName)
        {
           // BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }
        public static async Task<List<CalculatedElementData>> GetByBidId(int bidId)
        {
            var collection = database.GetCollection<CalculatedElementData>("CostAttributesCalculatedData");

            //var filter = new BsonDocument("bidId", bidId);//Builders<Allocation>.Filter.Empty;
            //var filter = new BsonDocument("$and", new BsonArray{

            //    new BsonDocument("bidId", bidId),
            //    new BsonDocument("ff",0)//,                
            //});

            //Guid guid = new Guid("{07f9cc00-101f-4e7b-a539-fd8d7c6b349a}");

            var result = await collection.Aggregate()           
           .Match(x => //x.BidCostGroupId == guid && 
                       x.CostingVersionId == bidId && 
                       x.AppliedFinancialFactors == 0 && 
                       x.AllocationId == BsonNull.Value 
                       )
           .ToListAsync();

            /*
            var filter1 = Builders<CalculatedElementData>.Filter.Eq("bidId", bidId);
            var filter2 = Builders<CalculatedElementData>.Filter.Eq("ff",0);
            var filter3 = Builders<CalculatedElementData>.Filter.Eq("allocId",BsonNull.Value);
            var filter4 = Builders<CalculatedElementData>.Filter.Eq("attributes.t",1);

            var filter = Builders<CalculatedElementData>.Filter.And(new List<FilterDefinition<CalculatedElementData>>{ filter1, filter2, filter3, filter4 });

            var options = new FindOptions<CalculatedElementData> 
            {
                BatchSize = Properties.Settings.Default.BatchSize
            };

            List<CalculatedElementData> allocations = new List<CalculatedElementData>();

            //var pipeline = usersCollection.Aggregate()
            //                .Unwind<OriginalType, NewResultType>(....

            using (var cursor = await collection.FindAsync(filter, options))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    allocations.AddRange(batch);
                    //break;
                }
            }            
            return allocations;
            */
            return result;
        }
    }
}
