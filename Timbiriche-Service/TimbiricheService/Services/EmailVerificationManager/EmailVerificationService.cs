using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using TimbiricheService.Email;
using TimbiricheService.Email.Templates;

namespace TimbiricheService
{
    public partial class UserManagerService : IEmailVerificationManager
    {
        private static Dictionary<string, string> _secureTokens = new Dictionary<string, string>();

        public bool SendEmailToken(string email, string username)
        {
            string token = GenerateTokenNumbersAndLetters();
            
            if (!_secureTokens.ContainsKey(username))
            {
                _secureTokens.Add(username, token);
            }

            EmailSender emailSender = new EmailSender(new EmailVerificationTemplate());
            bool isEmailSend = emailSender.SendEmail(email, token);

            return isEmailSend;
        }

        private string GenerateTokenNumbersAndLetters()
        {
            const int CODE_LENGTH = 8;
            const string VALID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            StringBuilder confirmationCodeBuilder = new StringBuilder();
            string confirmationCode = string.Empty;

            for (int i = 0; i < CODE_LENGTH; i++)
            {
                int index = random.Next(VALID_CHARACTERS.Length);
                confirmationCodeBuilder.Append(VALID_CHARACTERS[index]);
            }

            confirmationCode = confirmationCodeBuilder.ToString();

            return confirmationCode;
        }

        public bool VerifyEmailToken(string token, string username)
        {
            bool isTokenValid = false;
            string emailToken = null;

            if (_secureTokens.ContainsKey(username))
            {
                emailToken = _secureTokens[username];
                _secureTokens.Remove(username);
            }

            if (token == emailToken)
            {
                isTokenValid = true;
            }

            return isTokenValid;
        }
    }
}