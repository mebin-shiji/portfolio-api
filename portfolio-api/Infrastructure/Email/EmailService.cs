using portfolio_api.Features.Mail.Send;

namespace portfolio_api.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(List<SendMailCommand> mails)
        {
            throw new NotImplementedException();
        }
    }
}
