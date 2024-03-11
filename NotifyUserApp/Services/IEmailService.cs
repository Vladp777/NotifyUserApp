using NotifyUserApp.Models;

namespace NotifyUserApp.Services
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}