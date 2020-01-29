using RedisWebApplication.Model;
using System;
using System.Collections.Generic;

namespace RedisWebApplication.Common
{
    public static class TestClass
    {
        public static List<CalculatedElementData> FillElementList(int amount)
        {
            int Min = 0;
            int Max = 20;
            Random randNum = new Random();
            List<CalculatedElementData> calculatedElementDatas = new List<CalculatedElementData>();

            for (int index = 0; index < amount; index++)
            {
                decimal[] Values = new decimal[50];
                for (int i = 0; i < Values.Length; i++)
                {
                    Values[i] = randNum.Next(Min, Max);
                }

                CalculatedAttributeData[] calculatedAttributeDatas = new CalculatedAttributeData[20];
                for (int i = 0; i < calculatedAttributeDatas.Length; i++)
                {
                    calculatedAttributeDatas[i] = new CalculatedAttributeData { Characteristics = 3, Type = 1, CostAttributeName = randNum.Next(Min, Max).ToString(), Values = Values };
                }
                calculatedElementDatas.Add(new CalculatedElementData() { BidId = index, StartMonth = 1, StartYear = 2021, AppliedFinancialFactors = 1, Attributes = calculatedAttributeDatas });
            }
            return calculatedElementDatas;
        }
        public static List<CalculatedElementData> FillElement(int id, int amount)
        {
            int Min = 0;
            int Max = 20;
            Random randNum = new Random();
            List<CalculatedElementData> calculatedElementDatas = new List<CalculatedElementData>();

            for (int index = 0; index < amount; index++)
            {
                decimal[] Values = new decimal[50];
                for (int i = 0; i < Values.Length; i++)
                {
                    Values[i] = randNum.Next(Min, Max);
                }

                CalculatedAttributeData[] calculatedAttributeDatas = new CalculatedAttributeData[20];
                for (int i = 0; i < calculatedAttributeDatas.Length; i++)
                {
                    calculatedAttributeDatas[i] = new CalculatedAttributeData { Characteristics = 3, Type = 1, CostAttributeName = randNum.Next(Min, Max).ToString(), Values = Values };
                }
                calculatedElementDatas.Add(new CalculatedElementData() { BidId = id, BidCostGroupId = Guid.NewGuid(), StartMonth = 1, StartYear = 2021, AppliedFinancialFactors = 1, Attributes = calculatedAttributeDatas });
            }
            return calculatedElementDatas;
        }
    }
}
