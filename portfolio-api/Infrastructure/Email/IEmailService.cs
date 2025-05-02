using portfolio_api.Features.Mail.Send;

namespace portfolio_api.Infrastructure.Email
{
    public interface IEmailService
    {
        public Task SendEmailAsync(List<SendMailCommand> mails);
    }
}
