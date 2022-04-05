using System.Net.Mail;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Common;

namespace Services.EmailSender
{
    public class EmailSender : IMailSender
    {
        private readonly ISendGridClient client;
        private readonly IOptions<EmailSenderOptions> opt;
        
        public EmailSender(ISendGridClient client, IOptions<EmailSenderOptions> opt)
        {
            this.client = client;
            this.opt = opt;
        }

        public async Task SendEmailAsync(string emailTo, string subject, string content, string htmlContent)
        {
            EmailAddress fromEmail = new EmailAddress(this.opt.Value.SenderEmail);
            EmailAddress receiversEmail = new EmailAddress(emailTo);

            SendGridMessage message = MailHelper.CreateSingleEmail(fromEmail, receiversEmail, subject, content, htmlContent);

            Response result = await client.SendEmailAsync(message);

            if (!result.IsSuccessStatusCode)
            {
                throw new SmtpException(ExceptionMessages.SMTP_EXCEPTION);
            }
        }
    }
}
