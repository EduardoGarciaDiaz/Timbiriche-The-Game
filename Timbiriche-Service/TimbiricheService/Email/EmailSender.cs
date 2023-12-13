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
            bool emailSent = true;
            string emailContent = _emailTemplate.ComposeEmail(message);
            const string EMAIL_SENDER = "timbirichethegame@gmail.com";
            const string APPLICATION_PASSWORD = "dusb ueav ompt pckq";
            const string DISPLAY_NAME = "Timbiriche Team";
            const string SUBJECT = "Timbiriche The Game";
            const string SERVER_ADDRESS = "smtp.gmail.com";
            const int PORT = 587;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(EMAIL_SENDER, DISPLAY_NAME);
            mailMessage.To.Add(recipient);
            mailMessage.Subject = SUBJECT;
            mailMessage.Body = emailContent;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient(SERVER_ADDRESS, PORT);
            smtpClient.Credentials = new NetworkCredential(EMAIL_SENDER, APPLICATION_PASSWORD);
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