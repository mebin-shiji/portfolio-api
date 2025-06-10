using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using portfolio_api.Features.Mail.Send;
using portfolio_api.Infrastructure.Services.Storage;

namespace portfolio_api.Infrastructure.Services.Email;

public class EmailService(IOptions<EmailSettings> emailSettingsOptions, ILogger<EmailService> logger, IStorageService storageService) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(emailSettingsOptions), "Email settings options cannot be null.");
    private readonly ILogger<EmailService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IStorageService _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService), "Storage service cannot be null.");

    public async Task SendEmailAsync(List<SendMailCommand> mails, CancellationToken cancellationToken)
    {
        if (mails is null || mails.Count == 0)
        {
            _logger.LogWarning("Attempted to send email with null or empty mail list.");
            throw new ArgumentException("Mails cannot be null or empty.", nameof(mails));
        }

        List<Task> messageTasks = [];
        try
        {
            foreach (var mail in mails)
            {
                _logger.LogInformation("Preparing email to: {Recipient}", mail.To ?? _emailSettings.DefaultToEmail);

                var message = await CreateMimeMessageAsync(mail, cancellationToken);
                messageTasks.Add(SendMessageAsync(message, cancellationToken));
            }

            await Task.WhenAll(messageTasks);
            _logger.LogInformation("Successfully sent {Count} email(s).", mails.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending emails.");
            throw;
        }
    }

    private async Task<MimeMessage> CreateMimeMessageAsync(SendMailCommand mail, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        var senderEmail = mail.From ?? _emailSettings.DefaultFromEmail;
        var senderName = mail.FromName ?? _emailSettings.DefaultFromName;
        message.From.Add(new MailboxAddress(senderName, senderEmail));

        var recipientEmail = mail.To ?? _emailSettings.DefaultToEmail;
        var recipientName = mail.To ?? _emailSettings.DefaultToName;
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));
        message.Subject = mail.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = mail.IsHtml ? mail.Body : null,
            TextBody = !mail.IsHtml ? mail.Body : null
        };

        if (mail.Attachments?.Count > 0)
        {
            foreach (var attachment in mail.Attachments)
            {
                var fileBytes = await _storageService.DownloadBlobAsBytesAsync(attachment.Url, cancellationToken);
                bodyBuilder.Attachments.Add(attachment.Filename, fileBytes);
                _logger.LogDebug("Added attachment: {Filename}", attachment.Filename);
            }
        }

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private async Task SendMessageAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            _logger.LogDebug("Connecting to SMTP server {Server}:{Port}", _emailSettings.SmtpServer, _emailSettings.SmtpPort);

            await client.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                _emailSettings.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_emailSettings.UserName) && !string.IsNullOrWhiteSpace(_emailSettings.Password))
            {
                _logger.LogDebug("Authenticating SMTP user {UserName}", _emailSettings.UserName);
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            _logger.LogInformation("Email sent to {Recipient}", string.Join(", ", message.To.Select(r => r.ToString())));

            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", string.Join(", ", message.To.Select(r => r.ToString())));
            throw;
        }
    }
}
