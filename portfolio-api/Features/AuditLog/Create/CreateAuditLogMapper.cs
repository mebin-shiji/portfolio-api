using portfolio_api.Domain.Enums;
using System.Net;

namespace portfolio_api.Features.AuditLog.Create;

public static class CreateAuditLogMapper
{
    public static Domain.Entities.AuditLog ToEntity(this CreateAuditLogCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command), "CreateAuditLogCommand cannot be null.");
        }
        return new Domain.Entities.AuditLog
        {
            EventType = Enum.Parse<EventType>(command.EventType, true),
            Page = command.Page,
            Description = command.Description,
            MetaData = command.MetaData,
            IsSuccess = command.IsSuccess,
            CreatedAt = DateTime.UtcNow
        };
    }
}
