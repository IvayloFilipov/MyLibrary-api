namespace Services.EmailSender
{
    public interface IMailSender
    {
        Task SendEmailAsync(string emailTo, string subject, string content, string htmlContent);
    }
}
