using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string smtpServer = "smtp.gmail.com";
    private readonly int smtpPort = 587;
    private readonly string smtpUsername = "emailtesting0237@gmail.com";
    private readonly string smtpPassword = "awxa ygok ulul tdmj";
    private readonly string senderEmail = "emailtesting0237@gmail.com";

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var client = new SmtpClient(smtpServer))
        {
            client.Port = smtpPort;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            client.EnableSsl = true;

            var message = new MailMessage(senderEmail, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}
