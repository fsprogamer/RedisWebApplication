﻿using Microsoft.AspNetCore.Mvc;
using RedisWebApplication.Common;
using RedisWebApplication.Model;
using RedisWebApplication.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
                               MongoService mongoService
                               )
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

        [HttpGet("getcostattribute/{id}")]
        public async Task<ActionResult<CalculatedElementData>> GetCostAttributeMongo(int bidId)
        {
            const string logMessage = "Get costattribute, ";
            var keyValue = $"CalculatedElementData:CostingVersionId:{bidId}";
            Expression<Func<CalculatedElementData, bool>> expression = x => x.BidId == bidId;

            try
            {
                Log.Information($"{logMessage}begin");
                var ret = await _mongoService.Get(expression);
                Log.Information($"{logMessage}end");
                return Ok(/*ret*/);
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
            const string logMessage = "Add costattributes, ";
            const int amount = 200;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<CalculatedElementData> calculatedElementDatas = TestClass.FillElementList(amount);
            sw.Stop();
            Log.Information("FillElementList {0} ms", sw.ElapsedMilliseconds);
            
            try
            {
                Log.Information($"{logMessage}begin");
                await _mongoService.Add(calculatedElementDatas);
                Log.Information($"{logMessage}end");

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }       
        [HttpGet("deletecostattribute/{bidId}")]
        public async Task<ActionResult<int>> DeleteCostAttribute(int bidId)
        {
            const string logMessage = "Delete costattribute, ";
            try
            {
                Log.Information($"{logMessage}begin");
                await _mongoService.Delete(bidId);
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }
        [HttpGet("copycostattributesfromredis")]
        public async Task<ActionResult<int>> CopyCostAttributesFromRedis()
        {
            const int amount = 200000;
            const string logMessage = "Copy costattributes, ";
            IEnumerable<string> keyValues = Enumerable.Range(0, amount).Select(x => $"CalculatedElementData:CostingVersionId:{x}");
            List<CalculatedElementData> calculatedElementDatas = new List<CalculatedElementData>();
            try
            {
                Log.Information($"{logMessage}begin");
                var result = await _redisService.GetAll<CalculatedElementData>(keyValues);

                Log.Information($"{logMessage}step 2");

                await _mongoService.Add(elements:result.Values.ToList());
                Log.Information($"{logMessage}end");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{logMessage}error, {ex.Message}");
                return BadRequest();
            }
        }
        [HttpGet("copycostattributefromredis/{bidId}")]
        public async Task<ActionResult<int>> CopyCostAttributesFromRedis(int bidId)
        {
            const string logMessage = "Copy costattribute, ";
            try
            {
                Log.Information($"{logMessage}begin");
                var result = await _redisService.Get<List<CalculatedElementData>>($"CalculatedElementData:CostingVersionId:{bidId}");
                Log.Information($"{logMessage}step 2");
                await _mongoService.Add(elements:result);
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
