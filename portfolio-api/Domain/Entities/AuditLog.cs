using portfolio_api.Domain.Enums;
using System.Net;

namespace portfolio_api.Domain.Entities;
public class AuditLog
{
    public int Id { get; init; }
    public required EventType EventType { get; set; }
    public string? Page { get; set; }
    public string? Description { get; set; }
    public object? MetaData { get; set; }
    public IPAddress? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime CreatedAt { get; set; }
}
