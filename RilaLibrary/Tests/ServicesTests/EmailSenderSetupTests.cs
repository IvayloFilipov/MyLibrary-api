using Moq;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

using Services.EmailSender;
using Microsoft.Extensions.Options;

namespace Tests.ServicesTests
{
    [TestFixture]
    public class EmailSenderSetupTests
    {
        readonly Mock<IOptions<EmailSenderOptions>> opt = new Mock<IOptions<EmailSenderOptions>>();

        private Mock<ISendGridClient> mockSendGridClient = null!;
        private IMailSender emailSender = null!;

        [SetUp]
        public void Setup()
        {
            EmailSenderOptions emailSenderOptions = new EmailSenderOptions()
            {
                ApiKey = "",
                SenderEmail = "",
                SenderName = ""
            };

            opt.Setup(x => x.Value).Returns(emailSenderOptions);

            mockSendGridClient = new Mock<ISendGridClient>();
            emailSender = new EmailSender(mockSendGridClient.Object, this.opt.Object);
        }

        [Test]
        public void SendEmailAsyncSendEmailSuccessfully()
        {
            mockSendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
               .ReturnsAsync(new Response(HttpStatusCode.Accepted, new StringContent("contejkjlnt"), new HttpResponseMessage().Headers));

            Assert.DoesNotThrowAsync(async Task () => await emailSender.SendEmailAsync("rila@rila.com", "subject", "content", "htmlContent"));
        }

        [Test]
        public void SendEmailAsyncThrowsException()
        {
            mockSendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
               .ReturnsAsync(new Response(HttpStatusCode.Forbidden, new StringContent("contejkjlnt"), new HttpResponseMessage().Headers));

            Assert.ThrowsAsync<SmtpException>(async Task () => await emailSender
                                                                    .SendEmailAsync("rila@rila.com", "subject", "content", "htmlContent"),
                                                                    "Email sending failed");
        }

        [Test]
        public async Task SendEmailAsyncSendTheCorrectMessage()
        {
            mockSendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
               .ReturnsAsync(new Response(HttpStatusCode.Accepted, new StringContent("contejkjlnt"), new HttpResponseMessage().Headers));

            await emailSender.SendEmailAsync("rila@rila.com", "subject", "content", "htmlContent");

            mockSendGridClient.Verify(x => x.SendEmailAsync(It.Is<SendGridMessage>(x => 
                x.Personalizations[0].Subject == "subject" &&
                x.From.Email == opt.Object.Value.SenderEmail &&
                x.Contents[0].Value == "content" &&
                x.Contents[1].Value == "htmlContent"
            ), default), Times.Once);
        }
    }
}
