using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RedisWebApplication.Model;
using RedisWebApplication.Repositiries.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class MongoService: IMongoService<CalculatedElementData>
    {
        CostAttributesCalculatedDataRepo costAttributesCalculatedDataRepo;       

        public MongoService(IOptions<MongoDbSettings> settings)
        {
            costAttributesCalculatedDataRepo = new CostAttributesCalculatedDataRepo(settings.Value.ConnectionString, settings.Value.Database);          
        }
        // get document by id
        public async Task<CalculatedElementData> Get(Expression<Func<CalculatedElementData, bool>> expression)
        {     
            return (await costAttributesCalculatedDataRepo.Get(expression)).FirstOrDefault();
            //return (await costAttributesCalculatedDataRepo.GetByBidId(new CalculatedElementData { BidId = bidId })).FirstOrDefault();
        }
        // add document
        public async Task Add(CalculatedElementData element)
        {
            await costAttributesCalculatedDataRepo.Insert(element);
        }
        // add collection
        public async Task Add(List<CalculatedElementData> elements)
        {
            int chunk_size = 10000;
            int chunk_count = (int)Math.Floor((decimal)elements.Count() / chunk_size);
            List<CalculatedElementData> part;
            for (int i = 0; i < chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (elements.Count() % chunk_size) : chunk_size;
                part = elements.GetRange(chunk_size * i, chunk_length);
                await costAttributesCalculatedDataRepo.Insert(part);
            }
            //await CostAttributesCalculatedDataRepo.Add(elements);
        }
        public async Task Delete(int bidId)
        {
            await costAttributesCalculatedDataRepo.Delete(new CalculatedElementData() { BidId = bidId });
        }
    }
}
