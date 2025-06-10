using portfolio_api.Features.Mail.Send;

namespace portfolio_api.Infrastructure.Services.Email;
public interface IEmailService
{
    public Task SendEmailAsync(List<SendMailCommand> mails);
}
