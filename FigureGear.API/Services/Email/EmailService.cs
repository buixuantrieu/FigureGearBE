using FigureGear.Service.Interface;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace FigureGear.API.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailService(IOptions<EmailSettings> settings, IWebHostEnvironment webHostEnvironment)
        {
            _settings = settings.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = bodyHtml;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}
