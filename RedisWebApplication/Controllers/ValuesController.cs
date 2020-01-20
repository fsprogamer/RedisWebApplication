using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Services;
using Serilog;
using StackExchange.Redis;

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
            //await _redisService.Set("key1", $"{id}");
            var definitely = await _redisService.Get("key2");
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
        [HttpPost("addcollection")]
        public async Task<ActionResult<int>> AddCollection([FromBody]IList<User> users)
        {
            IList<Tuple<string, User>> values = new List<Tuple<string, User>>();

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            foreach (var user in users)
                values.Add(new Tuple<string, User>($"collection:user.{user.Id}", user));

            var result = await _redisService.SetCollection(values);

            Log.Information($"Add user: {result}");
            return Ok();
        }

        [HttpPost("addhashset")]
        public async Task<ActionResult<int>> AddHashSet([FromBody]IList<User> users)
        {
            //RedisKey hashKey = "hashKey";

            //HashEntry[] redisBookHash = {
            //    new HashEntry("title", "Redis for .NET Developers"),
            //    new HashEntry("year", 2016),
            //    new HashEntry("author", "Taswar Bhatti")
            //  };

            //await _redisService.SetHashSet(hashKey, redisBookHash);
            //var result1 = await _redisService.HashGetAllAsync(hashKey);


            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            foreach (var user in users)
            {
                RedisKey userHashKey = $"user.{user.Id}";
                HashEntry[] redisUser = { new HashEntry("Firstname", user.Firstname),
                                          new HashEntry("Lastname", user.Lastname),
                                          new HashEntry("Twitter", user.Twitter),
                                          new HashEntry("Blog", user.Blog)
                                        };
                await _redisService.SetHashSet(userHashKey, redisUser);
            }

            var result2 = await _redisService.HashGetAllAsync("user.1");

            //Log.Information($"Add user: {result}");
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
