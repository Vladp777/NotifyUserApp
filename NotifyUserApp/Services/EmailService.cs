using NotifyUserApp.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace NotifyUserApp.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _log;

    public EmailService(ILogger<EmailService> log)
    {
        _log = log;
    }
    public void SendEmail(EmailModel emailModel)
    {
		try
		{
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("alexxnaggetss@gmail.com"));

            email.To.Add(MailboxAddress.Parse(emailModel.EmailTo));
            email.Subject = emailModel.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = emailModel.Message
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("alexxnaggetss@gmail.com", "mykl mqui wrvs essu ");

            smtp.Send(email);
            smtp.Disconnect(true);

            _log.LogInformation($"User '{emailModel.EmailTo}' is notified");
        }
		catch (System.Exception)
		{
            _log.LogError($"User '{emailModel.EmailTo}' is not notified");
		}
        
    }
}
