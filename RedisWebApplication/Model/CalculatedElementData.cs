using System;
using MongoDB.Bson.Serialization.Attributes;

namespace RedisWebApplication.Model
{
     [BsonIgnoreExtraElements]

    // struct cannot be deserialized
    public class CalculatedElementData
    {
        [BsonElement("bidId")]
        public int CostingVersionId { get; set; }

        [BsonElement("costGrpId")]
        public Guid BidCostGroupId { get; set; }

        [BsonElement("elId")]
        public Guid ElementId { get; set; }

        [BsonElement("currencyId")]
        public int CurrencyId { get; set; }

        [BsonElement("startYr")]
        public short StartYear { get; set; }

        [BsonElement("startMth")]
        public byte StartMonth { get; set; }

        [BsonElement("ff")]
        public byte AppliedFinancialFactors { get; set; }

        [BsonElement("attributes")]
        public CalculatedAttributeData[] Attributes { get; set; }
        [BsonElement("allocId")]
        public Guid? AllocationId { get; set; }
    }
}
