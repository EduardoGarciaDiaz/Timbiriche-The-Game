using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheService.Email.Templates;
using TimbiricheService.Email;
using TimbiricheDataAccess;
using System.Security;
using System.Net;

namespace TimbiricheService
{
    public partial class UserManagerService : IEmailVerificationManager
    {
        private static SecureString _secureToken;

        public bool SendEmailToken(string email)
        {
            string token = GenerateTokenNumbersAndLetters();
            SaveOnSecureString(token);
            EmailSender emailSender = new EmailSender(new EmailVerificationTemplate());
            bool isEmailSend = emailSender.SendEmail(email, token);

            return isEmailSend;
        }

        private string GenerateTokenNumbersAndLetters()
        {
            string confirmationCode = string.Empty;
            const int CODE_LENGTH = 8;
            const string VALID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            for (int i = 0; i < CODE_LENGTH; i++)
            {
                int index = random.Next(VALID_CHARACTERS.Length);
                confirmationCode += VALID_CHARACTERS[index];
            }
            return confirmationCode;
        }

        private void SaveOnSecureString(string token)
        {
            _secureToken = new SecureString();
            foreach (char c in token.ToString())
            {
                _secureToken.AppendChar(c);
            }
        }

        public bool VerifyEmailToken(string tokenReceived)
        {
            bool isTokenValid = false;
            string token = new NetworkCredential(string.Empty, _secureToken).Password;

            if (tokenReceived.Equals(token))
            {
                _secureToken.Dispose();
                isTokenValid = true;
            }

            return isTokenValid;
        }
    }
}