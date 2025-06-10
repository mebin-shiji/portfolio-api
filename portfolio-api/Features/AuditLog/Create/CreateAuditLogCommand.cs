namespace portfolio_api.Features.AuditLog.Create;

public sealed record CreateAuditLogCommand
(
    string EventType,
    string? Page,
    string? Description,
    object? MetaData,
    bool IsSuccess
);
