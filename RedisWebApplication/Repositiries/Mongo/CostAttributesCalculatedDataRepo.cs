using RedisWebApplication.Model;

namespace RedisWebApplication.Repositiries.Mongo
{
    public class CostAttributesCalculatedDataRepo : BaseMongoRepository<CalculatedElementData>
    {
        public CostAttributesCalculatedDataRepo(string connectionString, string databaseName) : base(connectionString, databaseName, "CostAttributesCalculatedData")
        {
        }
    }
}
