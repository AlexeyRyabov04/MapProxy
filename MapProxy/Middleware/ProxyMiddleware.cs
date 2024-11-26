using MapProxy.Services.Interfaces;
using System.Net.Http;

namespace MapProxy.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;

        public ProxyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAccessRuleService accessRuleService, IHttpClientFactory httpClientFactory, IRedisService redisService)
        {
            
            var targetUrl = $"https://portaltest.gismap.by/arcservertest/rest/services{context.Request.Path}{context.Request.QueryString}";
            var clientIp = context.Connection.RemoteIpAddress!.MapToIPv4().ToString();
            var serviceName = context.Request.Path.ToString().Remove(0, 1).Split('/')[0];

            var accessRule = await accessRuleService.GetAccessRuleAsync(serviceName);
            if (accessRule == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Service not found.");
                return;
            }
            var stat = await redisService.GetRemainingRequestsAsync(clientIp, serviceName, accessRule.MaxRequests);
            if (stat.RemainingRequests > 0)
            {
                await redisService.UpdateRequestStatsAsync(clientIp, serviceName, stat.ProcessedRequests + 1, stat.RemainingRequests - 1);
                context.Response.Redirect(targetUrl, permanent: false);
                return;
            }
            else
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Request limit exceeded.");
            }
            await _next(context);
        }

    }
}
