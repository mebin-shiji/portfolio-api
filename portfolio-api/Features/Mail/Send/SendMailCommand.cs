namespace portfolio_api.Features.Mail.Send;

public sealed record SendMailCommand
{
    public string? From { get; init; } = null;
    public string? FromName { get; init; } = null;
    public string? To { get; init; } = null;
    public string? ToName { get; init; } = null;
    public string Subject { get; init; } = default!;
    public string Body { get; init; } = default!;
    public bool IsHtml { get; init; } = false;
    public List<Attachment> Attachments { get; init; } = [];
}

public sealed record Attachment
(
    string Filename,
    string Url
);