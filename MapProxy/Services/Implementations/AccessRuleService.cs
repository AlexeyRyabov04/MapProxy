using MapProxy.Data;
using MapProxy.Models;
using MapProxy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapProxy.Services.Implementations
{
    public class AccessRuleService : IAccessRuleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRedisService _redisService;

        public AccessRuleService(ApplicationDbContext context, IRedisService redisService)
        {
            _context = context;            _redisService = redisService;
        }

        public async Task<AccessRule?> GetAccessRuleAsync(string serviceName)
        {
            return await _context.AccessRules
                .FirstOrDefaultAsync(ar => ar.ServiceName == serviceName);
        }

        public async Task SetAccessRuleAsync(string serviceName, int maxRequests)
        {
            var accessRule = await _context.AccessRules.FirstOrDefaultAsync(ar => ar.ServiceName == serviceName);
            if (accessRule == null)
            {
                accessRule = new AccessRule { ServiceName = serviceName, MaxRequests = maxRequests };
                _context.AccessRules.Add(accessRule);
            }
            else
            {
                accessRule.MaxRequests = maxRequests;
                _context.AccessRules.Update(accessRule);
                await _redisService.UpdateMaxRequestsCountAsync(serviceName, maxRequests);
            }
            await _context.SaveChangesAsync();
        }
    }
}
