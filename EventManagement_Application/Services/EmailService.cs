using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public async Task SendEmailAsync(string userEmail, string subject, string message)
    {
        try
        {
            var smtpServer = _config["EmailSettings:SMTPServer"];
            var smtpPort = int.Parse(_config["EmailSettings:SMTPPort"]);
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];
            var adminEmail = "EventifyApplicationTeam@gmail.com"; // Admin email address

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "Event Contact Form"),
                    Subject = subject,
                    Body = $"Sender: {userEmail}\n\nMessage:\n{message}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(adminEmail); // Send email to admin
                mailMessage.ReplyToList.Add(new MailAddress(userEmail)); // Set user email as reply-to

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
