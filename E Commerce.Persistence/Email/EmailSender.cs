using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace E_Commerce.Persistence.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration config;
        public EmailSender(IConfiguration _config)
        {
            config = _config;
        }

        public  async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailsender = config["EmailSetting:Host"];
            var password = config["EmailSetting:Password"];

            var message = new MailMessage();
            message.From = new MailAddress(emailsender);
            message.To.Add(email);
            message.Subject = subject;
            message.Body = $"<html><body>{htmlMessage}</body></html>";
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(emailsender, password);
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587; 
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
        }
    }
}
