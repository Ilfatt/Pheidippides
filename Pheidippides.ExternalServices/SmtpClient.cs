using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Pheidippides.ExternalServices;

public class SmtpClient(IConfiguration configuration)
{
    private async Task SendMessage(string subject, string body, string emailTo)
    {
        var from = new MailAddress(configuration["EmailConfig:Sender"]!,
            configuration["EmailConfig:SenderName"]!);
        var to = new MailAddress(emailTo);
        var message = new MailMessage(from, to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        var smtp = new System.Net.Mail.SmtpClient(
            configuration["SmtpSettings:SmtpAddress"],
            int.Parse(configuration["SmtpSettings:Port"]!, CultureInfo.InvariantCulture))
        {
            Credentials = new NetworkCredential(
                configuration["EmailConfig:Sender"]!,
                configuration["EmailConfig:SenderPassword"]!),
            EnableSsl = true,
            UseDefaultCredentials = false,
        };

        await smtp.SendMailAsync(message);
    }
}