using System.Net.Mail;
using Microsoft.Extensions.Options;
using WebAppRazorPages.Settings;

namespace WebAppRazorPages.Services;

public class EmailService(IOptions<SmtpSettings> smtpSettings) : IEmailService
{
    public async Task SendAsync(string from, string to, string subject, string body)
    {
        var message = new MailMessage(from, to, subject, body);

        using (var emailClient = new SmtpClient(smtpSettings.Value.Host, smtpSettings.Value.Port))
        {
            emailClient.Credentials =
                new System.Net.NetworkCredential(smtpSettings.Value.UserName, smtpSettings.Value.Password);
            await emailClient.SendMailAsync(message);
        }
    }
}