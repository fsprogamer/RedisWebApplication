using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Common;
using RedisWebApplication.Model;
using RedisWebApplication.Services;
using Serilog;
using System;
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
        private readonly RedisListService _redisListService;
        private readonly RedisSetService _redisSetService;
        private readonly RedisHashSetService _redisHashSetService;

        private readonly MongoService _mongoService;
        public ValuesController(RedisService redisService,
                                RedisListService redisListService,
                                RedisSetService redisSetService,
                                RedisHashSetService redisHashSetService,
                                MongoService mongoService)
        {
            _redisService = redisService;
            _redisListService = redisListService;
            _redisSetService = redisSetService;
            _redisHashSetService = redisHashSetService;
            _mongoService = mongoService;
        }

        //------------------------------ELEMENT----------------------------------
        #region ELEMENT
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
        #endregion

        //------------------------------SET------------------------------------------
        #region SET        
        [HttpGet("addcostattribute2")]
        public async Task<ActionResult<int>> AddCostAttribute2()
        {
            const int amount = 50000;
            List<CalculatedElementData> calculatedElementDatas = TestClass.FillElementList(amount);

            int chunk_size = 1000;
            int chunk_count = (int)Math.Floor((decimal)calculatedElementDatas.Count / chunk_size);            

            Log.Information($"Add costattribute, begin");
            for (int i = 0; i < chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (calculatedElementDatas.Count % chunk_size) : chunk_size;
                CalculatedElementData[] part = calculatedElementDatas.GetRange(chunk_size * i, chunk_length).ToArray();

                var result = await _redisSetService.SAdd("collection_key", part);
            }
            //const int amount = 50000;
            //CalculatedElementData[] calculatedElementDatas = TestClass.FillElementList(amount).ToArray();
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

            var length = await _redisSetService.SetLength("collection_key");
            int chunk_count = (int)Math.Floor((decimal)length / chunk_size);

            for (int i = 0; i < chunk_count; i++)
            {
                var chunk_length = (i == chunk_count) ? (length % chunk_size) : chunk_size;
                var result = _redisSetService.SScan("collection_key", i*chunk_size , (int)chunk_length);
            }
            Log.Information($"Get costattribute, end");
            return Ok(/*result*/);
        }
        #endregion
        //----------------------------LIST-------------------------------------------
        #region LIST
        [HttpGet("addcostattribute3")]
        public async Task<ActionResult<int>> AddCostAttribute3()
        {
            const int amount = 200000;
            CalculatedElementData[] calculatedElementDatas = TestClass.FillElementList(amount).ToArray();
            Log.Information($"Add costattribute list, begin");
            var result = await _redisListService.LPush<CalculatedElementData>("list_key", calculatedElementDatas);

            Log.Information($"Add costattribute list: {result}");
            return Ok();
        }
        [HttpGet("getcostattribute3")]
        public async Task<ActionResult<int>> GetCostAttribute3()
        {                       
            var result = _redisListService.LRange<CalculatedElementData>("list_key");

            Log.Information($"Get costattribute list, end");
            return Ok(/*result*/);
        }
        #endregion
        //---------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<int>> GetAsync(int id)
        {
            var definitely = await _redisService.Get<string>("key2");
            Log.Information(definitely);
            return id;
        }

        //[HttpPost("add")]
        //public async Task<ActionResult<int>> Add([FromBody]User user)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest("Not a valid model");

        //    var result = await _redisService.Set($"user:{user.Id}", user);

        //    Log.Information($"Add user: {result}");
        //    return Ok();
        //}
        [HttpGet("addcostattributes")]
        public async Task<ActionResult<int>> AddCostAttributes()
        {
            const int amount = 200000;
            List<CalculatedElementData> calculatedElementDatas = TestClass.FillElementList(amount);

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

                var result = await _redisService.SetAll(values);                               
            }

            Log.Information($"Add costattribute, end");

            return Ok();
        }

        [HttpGet("addcostattribute4")]
        public async Task<ActionResult<int>> AddCostAttribute4()
        {
            const int amount = 200000;
            CalculatedElementData[] calculatedElementDatas = TestClass.FillElementList(amount).ToArray();
            Log.Information($"Add costattribute to list, begin");

            await _redisListService.AddToList("list_key", calculatedElementDatas);

            Log.Information($"Add user collection to list, end");
            return Ok();
        }

        [HttpPost("addcollection")]
        public async Task<ActionResult<int>> AddCollection([FromBody]IList<User> users)
        {
            IList<Tuple<string, User>> values = new List<Tuple<string, User>>();

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            foreach (var user in users)
                values.Add(new Tuple<string, User>($"collection:user.{user.Id}", user));

            var result = await _redisService.SetAll(values);

            Log.Information($"Add user collection: {result}");
            return Ok();
        }

        [HttpPost("addhashset")]
        public async Task<ActionResult<int>> AddHashSet([FromBody]IEnumerable<User> users)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            await _redisHashSetService.SetHashSet(users);

            var result = await _redisHashSetService.HashGetAllAsync("user.1");

            Log.Information($"Add hashset: {result}");
            return Ok();
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Ready" };
        }
     
    }
}
