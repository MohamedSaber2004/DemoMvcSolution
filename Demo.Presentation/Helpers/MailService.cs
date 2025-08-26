using Demo.Presentation.Settings;
using Demo.Presentation.Utitlities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Demo.Presentation.Helpers
{
    public class MailService(IOptions<MailSettings> options) : IMailService
    {
        public void Send(Email email)
        {
            var mail = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(options.Value.Email),
                Subject = email.Subject,

            };

            mail.To.Add(MailboxAddress.Parse(email.To));
            mail.From.Add(new MailboxAddress(options.Value.DisplayName,options.Value.Email));

            var builder = new BodyBuilder();
            builder.TextBody = email.Body;

            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(options.Value.Host,options.Value.Port,SecureSocketOptions.StartTls);

            smtp.Authenticate(options.Value.Email, options.Value.Password);
            smtp.Send(mail);

            smtp.Disconnect(true);
        }
    }
}
