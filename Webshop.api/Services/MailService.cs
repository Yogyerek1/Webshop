using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Webshop.api.Services;

public interface IMailService
{
    Task SendMailAsync(string to, string subject, string body);
}

public class MailService(IConfiguration config) : IMailService
{
    public async Task SendMailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(
            config["MAIL_DISPLAY_NAME"] ?? "Webshop",
            config["MAIL_EMAIL"] ?? throw new InvalidOperationException("MAIL_EMAIL configuration is missing!")
        ));

        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(
                config["MAIL_HOST"],
                int.Parse(config["MAIL_PORT"] ?? "587"),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(config["MAIL_EMAIL"], config["MAIL_PASSWORD"]);

            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
