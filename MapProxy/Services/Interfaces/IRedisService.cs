using MapProxy.DTO;
using MapProxy.Models;

namespace MapProxy.Services.Interfaces
{
    public interface IRedisService
    {
        Task<RequestStats> GetRemainingRequestsAsync(string clientIp, string serviceName, int maxRequests);
        Task UpdateRequestStatsAsync(string clientIp, string serviceName, int processedRequests, int remainingRequests);
        Task UpdateMaxRequestsCountAsync(string serviceName, int maxRequests);
        Task<List<AccessRuleResponseDTO>> GetAllAccessRulesAsync();
    }
}
