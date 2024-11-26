using MapProxy.Models;
using MapProxy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MapProxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessRulesController : ControllerBase
    {
        private readonly IAccessRuleService _accessRuleService;

        public AccessRulesController(IAccessRuleService accessRuleService)
        {
            _accessRuleService = accessRuleService;
        }

        [HttpPost]
        public async Task<IActionResult> SetAccessRule([FromBody] AccessRule rule)
        {
            await _accessRuleService.SetAccessRuleAsync(rule.ServiceName, rule.MaxRequests);
            return Ok();
        }

        [HttpGet("{serviceName}")]
        public async Task<IActionResult> GetAccessRule(string serviceName)
        {
            var rule = await _accessRuleService.GetAccessRuleAsync(serviceName);
            if (rule == null) return NotFound();
            return Ok(rule);
        }
    }
}
