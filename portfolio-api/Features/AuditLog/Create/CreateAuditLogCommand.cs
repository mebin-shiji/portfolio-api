namespace portfolio_api.Features.AuditLog.Create;

public sealed record CreateAuditLogCommand
(
    string Action,
    string? Description,
    DateTime? CreatedAt,
    object? Data,
    string IpAddress
);
