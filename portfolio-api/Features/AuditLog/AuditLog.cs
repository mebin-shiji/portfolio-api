namespace portfolio_api.Features.AuditLog
{
    public class AuditLog
    {
        public int Id { get; init; }
        public required ActionType Action { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public object? Data { get; set; }
        public required string IpAddress { get; set; }
    }

    public enum ActionType
    {
        CREATE,
        DELETE,
        UPDATE,
        READ
    }
}
