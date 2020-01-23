using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RedisWebApplication.Model
{
    [BsonIgnoreExtraElements]
    [Serializable]
    public class CalculatedAttributeData
    {
        [BsonElement("costAttrName")]
        public string CostAttributeName { get; set; }

        [BsonElement("values")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] Values { get; set; }

        [BsonElement("t")]
        public byte Type { get; set; }

        [BsonElement("ch")]
        public byte Characteristics { get; set; }
    }
}
