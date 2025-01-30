using MailKit.Net.Smtp;
using MimeKit;


namespace KinoMate.server.Services
{
    public class EmailServices
    {
        public async Task SendEmailAsync(string recipientEmail, string subject, string? message, IFormFile? attachment)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Wheat", "mailtrap@demomailtrap.com"));
            email.To.Add(new MailboxAddress("Mk", recipientEmail));

            email.Subject = subject;
            var builder = new BodyBuilder()
            {
                TextBody = message
            };

            if (attachment != null && attachment.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await attachment.CopyToAsync(memoryStream);
                    builder.Attachments.Add(attachment.FileName, memoryStream.ToArray());
                }
            }

            email.Body = builder.ToMessageBody();

            using (var smpt = new SmtpClient())
            {
                await smpt.ConnectAsync("live.smtp.mailtrap.io", 587, false);
                await smpt.AuthenticateAsync("api", "2bcf4537d93ac2a1006ce299cd7dd819");
                await smpt.SendAsync(email);
                await smpt.DisconnectAsync(true);
            }
        }
    }
}
