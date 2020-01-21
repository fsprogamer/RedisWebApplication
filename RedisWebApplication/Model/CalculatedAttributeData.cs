namespace RedisWebApplication.Model
{
    public class CalculatedAttributeData
    {
        public string CostAttributeName { get; set; }
        public decimal[] Values { get; set; }
        public byte Type { get; set; }
        public byte Characteristics { get; set; }
    }
}
