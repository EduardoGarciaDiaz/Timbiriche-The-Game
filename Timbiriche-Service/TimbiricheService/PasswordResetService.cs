using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheService.Email;
using TimbiricheService.Email.Templates;

namespace TimbiricheService
{
    public partial class UserManagerService : IPasswordReset
    {
        public bool SendResetToken(string email)
        {
            int token = GenerateToken();
            bool isResetTokenSaved = SaveResetToken(email, token);

            if (!isResetTokenSaved)
            {
                return false;
            }

            EmailSender emailSender = new EmailSender(new PasswordResetTemplate());
            bool emailSent = emailSender.SendEmail(email, token.ToString());

            return emailSent;
        }

        public bool ValidateResetToken(string email, int token)
        {
            int playerId = GetPlayerIdByEmail(email);
            if(playerId == 0)
            {
                return false;
            }
            
            PasswordResetTokens passwordResetTokens = PasswordResetManagement
                .GetPasswordResetTokenByIdPlayerAndToken(playerId, token);

            if (passwordResetTokens == null)
            {
                return false;
            }

            if(DateTime.Now > passwordResetTokens.expirationDate)
            {
                return false;
            }

            return true;
        }

        private bool SaveResetToken(string email, int token)
        {
            bool tokenGenerated = false;
            int playerId = GetPlayerIdByEmail(email);
            if (playerId != 0)
            {
                DateTime creationDateTime = DateTime.Now;
                DateTime expirationDateTime = creationDateTime.AddMinutes(5);

                PasswordResetTokens passwordResetTokens = new PasswordResetTokens();
                passwordResetTokens.token = token;
                passwordResetTokens.creationDate = creationDateTime;
                passwordResetTokens.expirationDate = expirationDateTime;
                passwordResetTokens.idPlayer = playerId;

                tokenGenerated = PasswordResetManagement.AddToken(passwordResetTokens);
            }
            return tokenGenerated;
        }

        private int GenerateToken()
        {
            Random random = new Random();
            int token = random.Next(000000, 999999);
            return token;
        }

        private int GetPlayerIdByEmail(string email)
        {
            UserManagement userManagement = new UserManagement();
            return userManagement.GetIdPlayerByEmail(email);
        }

        public int UpdatePassword(string email, string newPassword, string newPasswordConfirmation)
        {
            int idPlayer = GetPlayerIdByEmail(email);
            if (newPassword.Equals(newPasswordConfirmation))
            {
                return PasswordResetManagement.UpdatePasswordPlayer(idPlayer, newPassword);
            }
            return -1;
        }

    }
}
