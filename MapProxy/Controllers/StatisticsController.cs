using MapProxy.Models;
using MapProxy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MapProxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public StatisticsController(IRedisService redisService)
        {
            _redisService = redisService;
        }


        [HttpGet()]
        public async Task<IActionResult> GetStatisticsAccessRule()
        {
            try
            {
                var accessRules = await _redisService.GetAllAccessRulesAsync();
                return Ok(accessRules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
