using portfolio_api.Infrastructure.Persistance;

namespace portfolio_api.Features.AuditLog.Create;

public interface ICreateAuditLogHandler
{
    Task Handle(CreateAuditLogCommand request, HttpContext context, CancellationToken ct);
}

public class CreateAuditLogHandler(AppDbContext dbContext, ILogger<CreateAuditLogHandler> logger) : ICreateAuditLogHandler
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<CreateAuditLogHandler> _logger = logger;

    public async Task Handle(CreateAuditLogCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received audit log request for event: {EventType} with description: {Description}", request.EventType, request.Description);

        var auditLog = request.ToEntity();
        auditLog.IpAddress = context.Connection.RemoteIpAddress;
        auditLog.UserAgent = context.Request.Headers.UserAgent.ToString();

        _dbContext.AuditLog.Add(auditLog);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created audit log for event: {EventType} at {Timestamp}", auditLog.EventType, auditLog.CreatedAt);
    }
}