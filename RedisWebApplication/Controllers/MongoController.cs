using Microsoft.AspNetCore.Mvc;
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
    public class MongoController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly MongoService _mongoService;
        public MongoController(RedisService redisService,
                               MongoService mongoService)
        {
            _redisService = redisService;
            _mongoService = mongoService;
        }

        [HttpGet("getcostattribute/{id}")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributeMongo(int id)
        {
            var keyValue = $"CalculatedElementData:CostingVersionId:{id}";

            var ret = await _mongoService.Get(id);

            Log.Information($"Get user: Ok");
            return Ok(ret);
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Ready" };
        }
     
    }
}
