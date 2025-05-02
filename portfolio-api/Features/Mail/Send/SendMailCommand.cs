namespace portfolio_api.Features.Mail.Send
{
    public class SendMailCommand
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public List<Attachment> Attachments { get; set; } = [];
    }

    public class Attachment
    {
        public string Filename { get; set; }
        public string Url { get; set; }
    }
}
