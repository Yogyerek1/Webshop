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
    private async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{templateName}.html");
        if (!File.Exists(path)) throw new FileNotFoundException("Template not found.");

        string html = await File.ReadAllTextAsync(path);

        foreach (var item in placeholders)
        {
            html = html.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return html;
    }

    public async Task SendMailWithTemplateAsync(string to, string subject, string templateName, Dictionary<string, string> data)
    {
        var htmlBody = await GetTemplateAsync(templateName, data);

        await SendMailAsync(to, subject, htmlBody);
    }

    public async Task SendMailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();

        var MAIL_HOST = config["MAIL_HOST"] ?? throw new InvalidOperationException("MAIL_HOST configuratuion is missing.");
        var MAIL_DISPLAY_NAME = config["MAIL_DISPLAY_NAME"] ?? throw new InvalidOperationException("MAIL_DISPLAY_NAME configuration is missing.");
        var MAIL_MAIL = config["MAIL_EMAIL"] ?? throw new InvalidOperationException("MAIL_EMAIL configuration is missing.");
        var MAIL_PORT = config["MAIL_PORT"] ?? "587";
        var MAIL_PASSWORD = config["MAIL_PASSWORD"] ?? throw new InvalidOperationException("MAIL_PASSWORD configuration is missing.");

        email.From.Add(new MailboxAddress(
            MAIL_DISPLAY_NAME,
            MAIL_MAIL
        ));

        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(
                MAIL_HOST,
                int.Parse(MAIL_PORT),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(MAIL_MAIL, MAIL_PASSWORD);

            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
