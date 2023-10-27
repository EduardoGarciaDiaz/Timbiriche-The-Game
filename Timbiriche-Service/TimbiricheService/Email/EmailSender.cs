using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TimbiricheService.Email.Templates;

namespace TimbiricheService.Email
{
    public class EmailSender
    {
        private IEmailTemplate _emailTemplate;

        public EmailSender(IEmailTemplate emailTemplate)
        {
            _emailTemplate = emailTemplate;
        }

        public bool SendEmail(string recipient, string message)
        {
            bool emailSent = true;
            string emailContent = _emailTemplate.ComposeEmail(message);
            string emailSender = "timbirichethegame@gmail.com";
            string displayName = "Timbiriche Team";
            string subject = "Timbiriche The Game";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSender, displayName);
            mailMessage.To.Add(recipient);
            mailMessage.Subject = subject;
            mailMessage.Body = emailContent;
            mailMessage.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.Gmail.com", 587);
            client.Credentials = new NetworkCredential(emailSender, "dusb ueav ompt pckq");
            client.EnableSsl = true;

            try
            {
                client.Send(mailMessage);
            } 
            catch (SmtpException ex)
            {
                emailSent = false;
            }

            return emailSent;
        }
    }
}
