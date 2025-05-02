namespace portfolio_api.Infrastructure.Email
{
    public class EmailSettings
    {
        public required string SmtpServer { get; init; }
        public int SmtpPort { get; init; }
        public required string FromEmail { get; init; }
        public required string DefaultToEmail { get; init; }
        public bool IsHtml { get; init; }
    }
}
