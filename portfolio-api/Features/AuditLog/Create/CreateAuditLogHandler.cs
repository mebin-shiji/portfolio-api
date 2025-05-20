using portfolio_api.Infrastructure.Persistance;

namespace portfolio_api.Features.AuditLog.Create;

public interface ICreateAuditLogHandler
{
    Task Handle(CreateAuditLogCommand request, CancellationToken ct);
}

public class CreateAuditLogHandler(AppDbContext dbContext, ILogger<CreateAuditLogHandler> logger) : ICreateAuditLogHandler
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<CreateAuditLogHandler> _logger = logger;

    public async Task Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received audit log request for action: {Action} from IP: {IpAddress}", request.Action, request.IpAddress);

        var auditLog = new AuditLog
        {
            Action = Enum.Parse<ActionType>(request.Action),
            Description = request.Description,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
            Data = request.Data,
            IpAddress = request.IpAddress
        };

        _dbContext.AuditLog.Add(auditLog);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created audit log for action: {Action} at {Timestamp}", auditLog.Action, auditLog.CreatedAt);
    }
}