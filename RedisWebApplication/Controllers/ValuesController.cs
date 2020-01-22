using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Model;
using RedisWebApplication.Services;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly MongoService _mongoService;
        public ValuesController(RedisService redisService, MongoService mongoService)
        {
            _redisService = redisService;
            _mongoService = mongoService;
        }

        
        [HttpPost("addcostattribute")]
        public async Task<ActionResult<CalculatedElementData>> AddCostAttribute([FromBody]CalculatedElementData calculatedElementData)
        {
            var keyValue = $"CalculatedElementData:CostingVersionId:{calculatedElementData.CostingVersionId}";

            var result = await _redisService.Set(keyValue, calculatedElementData);            

            Log.Information($"Add user: {result}");
            return Ok();
        }
        [HttpGet("getcostattribute_redis/{id}")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributeRedis(int id)
        {
            var keyValue = $"CalculatedElementData:CostingVersionId:{id}";

            var ret = await _redisService.Get<CalculatedElementData>(keyValue);

            Log.Information($"Get user: Ok");
            return Ok(ret);
        }
        [HttpGet("getcostattribute_mongo/{id}")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributeMongo(int id)
        {
            var keyValue = $"CalculatedElementData:CostingVersionId:{id}";

            var ret = await _mongoService.Get(id);

            Log.Information($"Get user: Ok");
            return Ok(ret);
        }
        //------------------------------SET----------------------------------

        [HttpGet("addcostattribute2")]
        public async Task<ActionResult<int>> AddCostAttribute2()
        {
            const int amount = 50000;
            List<CalculatedElementData> calculatedElementDatas = FillElementList(amount);

            int chunk_size = 1000;
            int chunk_count = (int)Math.Floor((decimal)calculatedElementDatas.Count / chunk_size);            

            Log.Information($"Add costattribute, begin");
            for (int i = 0; i < chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (calculatedElementDatas.Count % chunk_size) : chunk_size;
                CalculatedElementData[] part = calculatedElementDatas.GetRange(chunk_size * i, chunk_length).ToArray();

                var result = await _redisService.SAdd("collection_key", part);
            }
            //const int amount = 50000;
            //CalculatedElementData[] calculatedElementDatas = FillElementList(amount).ToArray();
            //Log.Information($"Add costattribute, begin");
            //var result = await _redisService.SAdd("collection_key", calculatedElementDatas);

            //Log.Information($"Add user collection: {result}");
            return Ok();
        }

        [HttpGet("getcostattribute2")]
        public async Task<ActionResult<int>> GetCostAttribute2()
        {
            int chunk_size = 10;
            
            Log.Information($"Get costattribute, begin");

            var length = await _redisService.SetLength("collection_key");
            int chunk_count = (int)Math.Floor((decimal)length / chunk_size);

            for (int i = 0; i < chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (length % chunk_size) : chunk_size;
                var result = _redisService.SScan("collection_key", i*chunk_size , (int)chunk_length);
            }
            Log.Information($"Get costattribute, end");
            return Ok(/*result*/);
        }
        //----------------------------LIST----------------------------------------------
        [HttpGet("addcostattribute3")]
        public async Task<ActionResult<int>> AddCostAttribute3()
        {
            const int amount = 200000;
            CalculatedElementData[] calculatedElementDatas = FillElementList(amount).ToArray();
            Log.Information($"Add costattribute list, begin");
            var result = await _redisService.LPush<CalculatedElementData>("list_key", calculatedElementDatas);

            Log.Information($"Add costattribute list: {result}");
            return Ok();
        }
        [HttpGet("getcostattribute3")]
        public async Task<ActionResult<int>> GetCostAttribute3()
        {
            int chunk_size = 1000;
            object syncLock = new object();
            
            Log.Information($"Get costattribute list, begin");

            var length = await _redisService.LLen("list_key");
            int chunk_count = (int)Math.Floor((decimal)length / chunk_size);

            var partitoner = Partitioner.Create(0, length, chunk_size);
            RedisValue[] bag = new RedisValue[length];

            Parallel.ForEach(partitoner, new ParallelOptions() { MaxDegreeOfParallelism = 3 },
            value =>
            {
                  //lock (syncLock) {
    
                    _redisService.LRange<CalculatedElementData>("list_key", value.Item1, value.Item2).CopyTo(bag,value.Item1);
                  //  }
            });

            //for (int i = 0; i < chunk_count; i++)
            //{
            //    var chunk_length = (i == chunk_count) ? (length % chunk_size) : chunk_size;
            //    var result = await _redisService.LRangeAsync<CalculatedElementData>("list_key", i * chunk_size, (i + 1) * chunk_size - 1);
            //}


            Log.Information($"Get costattribute list, end");
            return Ok(/*result*/);
        }
        //---------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<int>> GetAsync(int id)
        {
            var definitely = await _redisService.Get<string>("key2");
            Log.Information(definitely);
            return id;
        }

        [HttpPost("add")]
        public async Task<ActionResult<int>> Add([FromBody]User user)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            var result = await _redisService.Set($"user:{user.Id}", user);

            Log.Information($"Add user: {result}");
            return Ok();
        }
        [HttpGet("addcostattributes")]
        public async Task<ActionResult<int>> AddCostAttributes()
        {
            const int amount = 200000;
            List<CalculatedElementData> calculatedElementDatas = FillElementList(amount);

            int chunk_size = 1000;
            int chunk_count = (int)Math.Floor((decimal)calculatedElementDatas.Count / chunk_size);
            List<CalculatedElementData> part;

            Log.Information($"Add costattribute, begin");
            for (int i = 0; i <= chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (calculatedElementDatas.Count % chunk_size) : chunk_size;
                part = calculatedElementDatas.GetRange(chunk_size * i, chunk_length);
                
                IList<Tuple<string, CalculatedElementData>> values = new List<Tuple<string, CalculatedElementData>>();

                values = part.Select(x => new Tuple<string, CalculatedElementData>($"CalculatedElementData:CostingVersionId:{x.CostingVersionId}", x)).ToList();

                var result = await _redisService.SetCollection(values);                               
            }

            Log.Information($"Add costattribute, end");

            return Ok();
        }



        [HttpGet("addcostattribute4")]
        public async Task<ActionResult<int>> AddCostAttribute4()
        {
            const int amount = 200000;
            CalculatedElementData[] calculatedElementDatas = FillElementList(amount).ToArray();
            Log.Information($"Add costattribute to list, begin");

            await _redisService.AddToList("list_key", calculatedElementDatas);

            Log.Information($"Add user collection to list, end");
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

        [HttpPost("addcollection")]
        public async Task<ActionResult<int>> AddCollection([FromBody]IList<User> users)
        {
            IList<Tuple<string, User>> values = new List<Tuple<string, User>>();

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            foreach (var user in users)
                values.Add(new Tuple<string, User>($"collection:user.{user.Id}", user));

            var result = await _redisService.SetCollection(values);

            Log.Information($"Add user collection: {result}");
            return Ok();
        }

        [HttpPost("addhashset")]
        public async Task<ActionResult<int>> AddHashSet([FromBody]IEnumerable<User> users)
        {

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            await _redisService.SetHashSet(users);

            var result = await _redisService.HashGetAllAsync("user.1");

            Log.Information($"Add hashset: {result}");
            return Ok();
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }


        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
