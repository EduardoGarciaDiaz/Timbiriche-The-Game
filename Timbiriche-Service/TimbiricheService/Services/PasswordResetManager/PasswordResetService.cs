using System;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheService.Email;
using TimbiricheService.Email.Templates;
using TimbiricheService.Exceptions;

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
            int invalidPlayerId = 0;

            if(playerId == invalidPlayerId)
            {
                return false;
            }

            try
            {
                PasswordResetTokens passwordResetTokens = PasswordResetManagement
                        .GetPasswordResetTokenByIdPlayerAndToken(playerId, token);

                if (passwordResetTokens == null)
                {
                    return false;
                }

                if (DateTime.Now > passwordResetTokens.expirationDate)
                {
                    return false;
                }

                return true;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        private bool SaveResetToken(string email, int token)
        {
            bool tokenGenerated = false;
            int playerId = GetPlayerIdByEmail(email);
            
            if (playerId != 0)
            {
                DateTime creationDateTime = DateTime.Now;
                DateTime expirationDateTime = creationDateTime.AddMinutes(5);

                PasswordResetTokens passwordResetTokens = new PasswordResetTokens
                {
                    token = token,
                    creationDate = creationDateTime,
                    expirationDate = expirationDateTime,
                    idPlayer = playerId
                };

                try
                {
                    tokenGenerated = PasswordResetManagement.AddToken(passwordResetTokens);
                }
                catch (DataAccessException ex)
                {
                    TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };

                    throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
                }
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
            try
            {
                return userManagement.GetIdPlayerByEmail(email);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public bool ChangePassword(string newPassword, string email)
        {            
            int idPlayer = GetPlayerIdByEmail(email);
            try
            {
                return PasswordResetManagement.ChangePasswordById(idPlayer, newPassword);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }
}
