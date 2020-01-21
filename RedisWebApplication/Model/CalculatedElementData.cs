using System;

namespace RedisWebApplication.Model
{
    public class CalculatedElementData
    {
        public int CostingVersionId { get; set; }
        public Guid BidCostGroupId { get; set; }
        public Guid ElementId { get; set; }
        public int CurrencyId { get; set; }
        public short StartYear { get; set; }
        public byte StartMonth { get; set; }
        public byte AppliedFinancialFactors { get; set; }
        // atributes[0].value[0] = 123.23
        // atributes[0].value[1] = 123.23
        // atributes[0].value[1] = 123.23
        // atributes[0].value[1] = 123.23
        // atributes[0].value[1] = 123.23
        // atributes[0].ch = 3
        public CalculatedAttributeData[] Attributes { get; set; }
        public Guid? AllocationId { get; set; }
    }
}
