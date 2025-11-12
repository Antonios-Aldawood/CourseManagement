using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CourseManagement.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ErrorOr<Success>> SendVerificationEmailAsync(string toEmail, string alias, string password, string verificationCode)
        {
            try
            {
                var smtpClient = new SmtpClient(_config["Email:SmtpHost"])
                {
                    Port = int.Parse(_config["Email:SmtpPort"]!),
                    Credentials = new NetworkCredential(_config["Email:Email"], _config["Email:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["Email:From"]!),
                    Subject = "Course Management - Account Verification",
                    Body = $"Hi {alias},\n\nPlease use this verification code on your first login: {verificationCode}\n\nHere is your current password: {password}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return Result.Success;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
