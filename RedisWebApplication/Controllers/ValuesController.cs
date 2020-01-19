using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Services;
using Serilog;

namespace RedisWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly RedisService _redisService;
        public ValuesController(RedisService redisService)
        {
            // Set RedisService property
            _redisService = redisService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<int>> GetAsync(int id)
        {
            await _redisService.Set("How many claps per person should this article get?", $"{id}");
            var definitely = await _redisService.Get("Follow me for more programming made simple articles");
            Log.Information(definitely);
            return id;
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
