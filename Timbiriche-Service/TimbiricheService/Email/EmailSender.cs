using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Email.Templates;

namespace TimbiricheService.Email
{
    public class EmailSender
    {
        private readonly IEmailTemplate _emailTemplate;

        public EmailSender(IEmailTemplate emailTemplate)
        {
            _emailTemplate = emailTemplate;
        }

        public bool SendEmail(string recipient, string message)
        {
            string emailContent = _emailTemplate.ComposeEmail(message);
            string emailSender = "timbirichethegame@gmail.com";
            string displayName = "Timbiriche Team";
            string subject = "Timbiriche The Game";
            string serverAddress = "smtp.gmail.com";
            string applicationPassword = Environment.GetEnvironmentVariable("APPLICATION_PASSWORD");
            int PORT = 587;
            bool emailSent = true;


            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSender, displayName);
            mailMessage.To.Add(recipient);
            mailMessage.Subject = subject;
            mailMessage.Body = emailContent;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient(serverAddress, PORT);
            smtpClient.Credentials = new NetworkCredential(emailSender, applicationPassword);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                emailSent = false;
            }

            return emailSent;
        }
    }
}