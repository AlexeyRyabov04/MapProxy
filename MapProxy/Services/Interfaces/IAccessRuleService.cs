using MapProxy.Models;

namespace MapProxy.Services.Interfaces
{
    public interface IAccessRuleService
    {
        Task<AccessRule?> GetAccessRuleAsync(string serviceName);
        Task SetAccessRuleAsync(string serviceName, int maxRequests);
    }
}
