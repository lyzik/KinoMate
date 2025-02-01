using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Http;

public class EmailServices
{
    private const string SmtpServer = "live.smtp.mailtrap.io";
    private const int SmtpPort = 587;
    private const string SenderPassword = "9d4ed744687a075aa1b1c92c96a48cee";

    public async Task SendEmailAsync(string recipientEmail, string subject, string? templateName, IFormFile? attachment)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("KinoMate", "info@kinomate.pl"));
            email.To.Add(new MailboxAddress("", recipientEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();

            if (!string.IsNullOrEmpty(templateName))
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", templateName + ".html");
                if (File.Exists(templatePath))
                {
                    bodyBuilder.HtmlBody = await File.ReadAllTextAsync(templatePath);
                }
                else
                {
                    throw new FileNotFoundException("Template not found.");
                }
            }
            else
            {
                bodyBuilder.TextBody = "Default email content.";
            }

            if (attachment != null)
            {
                using (var stream = attachment.OpenReadStream())
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, stream);
                }
            }

            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(SmtpServer, SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("api", SenderPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas wysyłania e-maila: {ex.Message}");
            throw;
        }
    }
}
