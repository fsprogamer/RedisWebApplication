using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Common;
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
            const string logMessage = "Get costattribute, ";
            var keyValue = $"CalculatedElementData:CostingVersionId:{id}";
            try
            {
                Log.Information($"{logMessage}begin");
                var ret = await _redisService.Get<CalculatedElementData>(keyValue);
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }

        [HttpGet("getcostattributes")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributes()
        {
            const string logMessage = "Get costattributes, ";
            const int amount = 200000;
            IEnumerable<string> keyValues = Enumerable.Range(0, amount).Select(x => $"CalculatedElementData:CostingVersionId:{x}");
            try
            {
                Log.Information($"{logMessage}begin");
                var ret = await _redisService.GetAll<CalculatedElementData>(keyValues);
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }

        [HttpGet("getcostattributes/{id}")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributes(int id)
        {
            const string logMessage = "Get costattributes, ";
            var keyValue = $"CalculatedElementData:CostingVersionId:{id}";
            try
            {
                Log.Information($"{logMessage}begin");
                var ret = await _redisService.Get<List<CalculatedElementData>>(keyValue);
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }

        [HttpPost("getcostattributes")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributesWithParam([FromBody] CostAttributesRequest request)
        {
            const string logMessage = "Get costattributes, ";
            var keyValue = $"CalculatedElementData:CostingVersionId:*.ff:*.allocId:{request.allocId}";
            try
            {
                Log.Information($"{logMessage}begin");
                var ret = await _redisService.GetAllByPatern<List<CalculatedElementData>>(keyValue);
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }

        [HttpPost("addcostattribute")]
        public async Task<ActionResult<CalculatedElementData>> AddCostAttribute([FromBody]CalculatedElementData calculatedElementData)
        {
            const string logMessage = "Add costattribute, ";
            var keyValue = $"CalculatedElementData:CostingVersionId:{calculatedElementData.BidId}";
            try
            {
                Log.Information($"{logMessage}begin");
                var result = await _redisService.Set(keyValue, calculatedElementData);
                Log.Information($"{logMessage}end");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }            
        }

        [HttpGet("addcostattributes")]
        public async Task<ActionResult<int>> AddCostAttributes()
        {
            const int amount = 200000;
            const string logMessage = "Add costattributes, ";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<CalculatedElementData> calculatedElementDatas = TestClass.FillElementList(amount);
            sw.Stop();
            Log.Information("FillElementList {0} ms", sw.ElapsedMilliseconds);

            try
            {
                int chunk_size = 1000;
                int chunk_count = (int)Math.Floor((decimal)calculatedElementDatas.Count / chunk_size);
                List<CalculatedElementData> part;
                Log.Information($"{logMessage}begin");
                for (int i = 0; i < chunk_count; i++)
                {
                    var chunk_length = (i == chunk_count) ? (calculatedElementDatas.Count % chunk_size) : chunk_size;
                    part = calculatedElementDatas.GetRange(chunk_size * i, chunk_length);

                    IList<Tuple<string, CalculatedElementData>> values = new List<Tuple<string, CalculatedElementData>>();

                    values = part.Select(x => new Tuple<string, CalculatedElementData>($"CalculatedElementData:CostingVersionId:{x.BidId}", x)).ToList();

                    var result = await _redisService.SetAll(values);
                }
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }
        [HttpGet("addcostattributes/{id}")]
        public async Task<ActionResult<int>> AddCostAttributes(int id)
        {
            const int amount = 200000;
            const string logMessage = "Add costattributes, ";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<CalculatedElementData> calculatedElementDatas = TestClass.FillElement(id, amount);
            sw.Stop();
            Log.Information("FillElementList {0} ms", sw.ElapsedMilliseconds);

            try
            {
                Log.Information($"{logMessage}begin");

                var result = await _redisService.Set($"CalculatedElementData:CostingVersionId:{id}", calculatedElementDatas);

                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }
        [HttpPost("addcostattributes")]
        public async Task<ActionResult<int>> AddCostAttributesWithParam([FromBody]CostAttributesRequest request)
        {
            const int amount = 50000;
            const string logMessage = "Add costattributes, ";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<CalculatedElementData> calculatedElementDatas = TestClass.FillElement(request.id, amount);
            sw.Stop();
            Log.Information("FillElementList {0} ms", sw.ElapsedMilliseconds);

            try
            {
                Log.Information($"{logMessage}begin");

                var result = await _redisService.Set($"CalculatedElementData:CostingVersionId:{request.id}.ff:{request.ff}.allocId:{request.allocId}", calculatedElementDatas);

                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }
    }
}
