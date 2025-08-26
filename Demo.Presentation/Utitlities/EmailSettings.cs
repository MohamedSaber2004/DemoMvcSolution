using System.Net;
using System.Net.Mail;

namespace Demo.Presentation.Utitlities
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com",587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("mmms77990@gmail.com", "qoupqzjxoswqumfv");
            client.Send("mmms77990@gmail.com",email.To,email.Subject,email.Body);
        }
    }
}
