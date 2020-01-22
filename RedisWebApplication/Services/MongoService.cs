using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RedisWebApplication.Model;
using RedisWebApplication.Repositiries.Mongo;
using System.Linq;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class MongoService
    {
        CostAttributesCalculatedDataRepo costAttributesCalculatedDataRepo;
        public MongoService(IOptions<MongoDbSettings> settings)
        {
            costAttributesCalculatedDataRepo = new CostAttributesCalculatedDataRepo(settings.Value.ConnectionString, settings.Value.Database);
        }
        // получаем один документ по id
        public async Task<CalculatedElementData> Get(int bidId)
        {
            return (await CostAttributesCalculatedDataRepo.GetByBidId(bidId)).FirstOrDefault();
        }
    }
}
