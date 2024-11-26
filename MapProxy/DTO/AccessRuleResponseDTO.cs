namespace MapProxy.DTO
{
    public class AccessRuleResponseDTO
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ClientIp { get; set; } = string.Empty;
        public int RemainingRequests { get; set; }
        public int ProcessedRequests { get; set; }

    }
}
