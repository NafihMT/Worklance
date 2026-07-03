using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.EmailInterface;
using Worklance.Application.Common.Settings;

namespace Worklance.Infrastructure.Services.Email
{
    public class EmailService:IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task SendOtpAsync(string email,string otp)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.From, _settings.DisplayName),
                Subject = "Worklance Email Verification OTP",
                Body = $@"
Hello,
Your OTP From Email Verification Is:
{otp}
Tish OTP is valid for 5 minutes.
Do not share this OTP with anyone.
Regrads,
Worklance Team",
                IsBodyHtml = false
            };

            message.To.Add(email);

            using var smtp=new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials =new NetworkCredential(
                    _settings.UserName,
                    _settings.Password),

                EnableSsl=_settings.EnableSsl
            };
            await smtp.SendMailAsync(message);
        }
    }
}
