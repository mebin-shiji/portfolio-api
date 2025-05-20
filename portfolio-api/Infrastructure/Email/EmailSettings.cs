namespace portfolio_api.Infrastructure.Email;

public class EmailSettings
{
    public string SmtpServer { get; set; } = default!;
    public int SmtpPort { get; set; }
    public string DefaultFromEmail { get; set; } = default!;
    public string DefaultFromName { get; set; } = default!;
    public string DefaultToEmail { get; set; } = default!;
    public string DefaultToName { get; set; } = default!;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; } = true;
}