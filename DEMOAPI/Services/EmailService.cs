using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmployeeApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("EMS System",
                _config["EmailSettings:SmtpUser"]));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();

            smtp.Connect(
                _config["EmailSettings:SmtpServer"],
                int.Parse(_config["EmailSettings:SmtpPort"]),
                SecureSocketOptions.StartTls
            );

            smtp.Authenticate(
                _config["EmailSettings:SmtpUser"],
                _config["EmailSettings:SmtpPass"]
            );

            smtp.Send(message);
            smtp.Disconnect(true);
        }
    }
}