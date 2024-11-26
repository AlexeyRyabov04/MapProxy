using MapProxy.DTO;
using MapProxy.Models;
using MapProxy.Services.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MapProxy.Services.Implementations
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _ttl = TimeSpan.FromHours(1);

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task<List<AccessRuleResponseDTO>> GetAllAccessRulesAsync()
        {
            var db = _redis.GetDatabase();
            var cursor = 0;
            var accessRules = new List<AccessRuleResponseDTO>();

            do
            {
                var result = await db.ExecuteAsync("SCAN", cursor);
                cursor = (int)result[0];
                var keys = (RedisKey[])result[1];
                foreach (var key in keys)
                {
                    var existingValue = await db.StringGetAsync(key);
                    if (!existingValue.IsNullOrEmpty)
                    {
                        var stats = JsonConvert.DeserializeObject<RequestStats>(existingValue);
                        var keyParts = key.ToString().Split('_', 2);
                        if (keyParts.Length == 2)
                        {
                            var clientIp = keyParts[0];
                            var serviceName = keyParts[1];
                            var dto = new AccessRuleResponseDTO
                            {
                                ClientIp = clientIp,
                                ServiceName = serviceName,
                                RemainingRequests = stats.RemainingRequests,
                                ProcessedRequests = stats.ProcessedRequests
                            };

                            accessRules.Add(dto);
                        }
                    }
                }
            } while (cursor != 0);

            return accessRules;
        }

        public async Task<RequestStats> GetRemainingRequestsAsync(string clientIp, string serviceName, int maxRequests)
        {
            var db = _redis.GetDatabase();
            var key = $"{clientIp}_{serviceName}";
            var data = await db.StringGetAsync(key);
            if (data.IsNullOrEmpty)
            {
                return new RequestStats { RemainingRequests = maxRequests, ProcessedRequests = 0 };
            }
            return JsonConvert.DeserializeObject<RequestStats>(data);
        }

        public async Task UpdateMaxRequestsCountAsync(string serviceName, int maxRequests)
        {
            var db = _redis.GetDatabase();
            var pattern = $"*_{serviceName}"; 
            var cursor = 0;

            do
            {
                var result = await db.ExecuteAsync("SCAN", cursor, "MATCH", pattern);
                cursor = (int)result[0]; 
                var keys = ((RedisKey[])result[1]).ToList();
                foreach (var key in keys)
                {
                    var existingValue = await db.StringGetAsync(key);
                    if (!existingValue.IsNullOrEmpty)
                    {
                        var stats = JsonConvert.DeserializeObject<RequestStats>(existingValue);
                        stats.RemainingRequests = maxRequests - stats.ProcessedRequests;
                        var updatedStatsJson = JsonConvert.SerializeObject(stats);
                        var currentTtl = await db.KeyTimeToLiveAsync(key);
                        await db.StringSetAsync(key, updatedStatsJson, currentTtl);
                    }
                }
            } while (cursor != 0);
        }

        public async Task UpdateRequestStatsAsync(string clientIp, string serviceName, int processedRequests, int remainingRequests)
        {
            var db = _redis.GetDatabase();
            var key = $"{clientIp}_{serviceName}";
            var stats = new RequestStats { ProcessedRequests = processedRequests, RemainingRequests = remainingRequests };
            var requestStats = JsonConvert.SerializeObject(stats);
            var isSet = await db.StringSetAsync(key, requestStats, _ttl, When.NotExists);
            if (!isSet)
            {
                var currentTtl = await db.KeyTimeToLiveAsync(key);
                await db.StringSetAsync(key, requestStats, currentTtl);
            }
        }
    }
}
