using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Model;
using RedisWebApplication.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RedisWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly MongoService _mongoService;
        public ElementController(RedisService redisService,
                                MongoService mongoService)
        {
            _redisService = redisService;
            _mongoService = mongoService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Ready" };
        }

        //------------------------------ELEMENT----------------------------------
        [HttpGet("getcostattribute/{id}")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttribute(int id)
        {
            var keyValue = $"CalculatedElementData:CostingVersionId:{id}";

            var ret = await _redisService.Get<CalculatedElementData>(keyValue);

            Log.Information($"Get user: Ok");
            return Ok(ret);
        }

        [HttpGet("getcostattributes")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributes()
        {
            const int amount = 200000;
            IEnumerable<string> keyValues = Enumerable.Range(0, amount).Select(x=>$"CalculatedElementData:CostingVersionId:{x}");
            
            var ret = await _redisService.GetAll<CalculatedElementData>(keyValues);

            Log.Information($"Get user: Ok");
            return Ok();
        }

        [HttpPost("addcostattribute")]
        public async Task<ActionResult<CalculatedElementData>> AddCostAttribute([FromBody]CalculatedElementData calculatedElementData)
        {
            var keyValue = $"CalculatedElementData:CostingVersionId:{calculatedElementData.CostingVersionId}";

            var result = await _redisService.Set(keyValue, calculatedElementData);            

            Log.Information($"Add user: {result}");
            return Ok();
        }

        [HttpGet("addcostattributes")]
        public async Task<ActionResult<int>> AddCostAttributes()
        {
            const int amount = 200000;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<CalculatedElementData> calculatedElementDatas = FillElementList(amount);
            sw.Stop();
            Log.Information("FillElementList {0} ms", sw.ElapsedMilliseconds);

            int chunk_size = 1000;
            int chunk_count = (int)Math.Floor((decimal)calculatedElementDatas.Count / chunk_size);
            List<CalculatedElementData> part;

            Log.Information($"Add costattribute, begin");
            for (int i = 0; i < chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (calculatedElementDatas.Count % chunk_size) : chunk_size;
                part = calculatedElementDatas.GetRange(chunk_size * i, chunk_length);
                
                IList<Tuple<string, CalculatedElementData>> values = new List<Tuple<string, CalculatedElementData>>();

                values = part.Select(x => new Tuple<string, CalculatedElementData>($"CalculatedElementData:CostingVersionId:{x.CostingVersionId}", x)).ToList();

                var result = await _redisService.SetAll(values);                               
            }

            Log.Information($"Add costattribute, end");

            return Ok();
        }

        private static List<CalculatedElementData> FillElementList(int amount)
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
                calculatedElementDatas.Add(new CalculatedElementData() { CostingVersionId = index, StartMonth = 1, StartYear = 2020, AppliedFinancialFactors = 1, Attributes = calculatedAttributeDatas });
            }
            return calculatedElementDatas;
        }     
    }
}
