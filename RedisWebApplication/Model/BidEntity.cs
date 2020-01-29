using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RedisWebApplication.Model
{
    [BsonIgnoreExtraElements]
    [Serializable]
    public class BidEntity //: BaseEntity<int>
    {
        [BsonElement("bidId")]
        public int BidId { get; set; }
    }
}
