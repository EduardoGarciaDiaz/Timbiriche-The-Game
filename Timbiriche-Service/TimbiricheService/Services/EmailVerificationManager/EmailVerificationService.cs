using System;
using System.Net;
using System.Security;
using System.Text;
using TimbiricheService.Email;
using TimbiricheService.Email.Templates;

namespace TimbiricheService
{
    public partial class UserManagerService : IEmailVerificationManager
    {
        private SecureString _secureToken;

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

        private void SaveOnSecureString(string token)
        {
            _secureToken = new SecureString();
            foreach (char c in token.ToString())
            {
                _secureToken.AppendChar(c);
            }
        }

        public bool VerifyEmailToken(string token)
        {
            bool isTokenValid = false;
            string tokenNetworkCredential = new NetworkCredential(string.Empty, _secureToken).Password;

            if (token.Equals(tokenNetworkCredential))
            {
                _secureToken.Dispose();
                isTokenValid = true;
            }

            return isTokenValid;
        }
    }
}