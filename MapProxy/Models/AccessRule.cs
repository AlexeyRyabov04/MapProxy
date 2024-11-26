namespace MapProxy.Models
{
    public class AccessRule
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int MaxRequests { get; set; }
    }
}
