namespace portfolio_api.Features.Mail.Send;

public class SendMailCommand
{
    public string? From { get; set; } = null;
    public string? FromName { get; set; } = null;
    public string? To { get; set; } = null;
    public string? ToName { get; set; } = null;
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsHtml { get; set; } = false;
    public List<Attachment> Attachments { get; set; } = [];
}

public class Attachment
{
    public required string Filename { get; set; }
    public required string Url { get; set; }
}