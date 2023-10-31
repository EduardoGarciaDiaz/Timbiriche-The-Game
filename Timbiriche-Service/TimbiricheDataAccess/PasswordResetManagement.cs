using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;

namespace TimbiricheDataAccess
{
    public static class PasswordResetManagement
    {
        public static bool AddToken(PasswordResetTokens passwordResetToken)
        {
            int rowsAffected = 0;

            if (passwordResetToken == null)
            {
                return false;
            }

            using (var context = new TimbiricheDBEntities())
            {
                var newPasswordResetToken = context.PasswordResetTokens.Add(passwordResetToken);
                try
                {
                    rowsAffected = context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Entity: {entityValidationErrors.Entry.Entity.GetType().Name}," +
                                $" Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }
                }
            }
            return rowsAffected > 0;
        }

        public static PasswordResetTokens GetPasswordResetTokenByIdPlayerAndToken(int playerId, int token)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var query = from p in context.PasswordResetTokens
                            where p.token == token && p.idPlayer == playerId
                            select p;
                PasswordResetTokens passwordResetToken = query.SingleOrDefault();
                return passwordResetToken;
            }
        }

        public static int UpdatePasswordPlayer(int idPlayer, string password)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var playerToUpdate = context.Players.Find(idPlayer);
                if (playerToUpdate != null)
                {
                    PasswordHashManager passwordHashManager = new PasswordHashManager();
                    playerToUpdate.password = passwordHashManager.HashPassword(password);
                    playerToUpdate.salt = passwordHashManager.Salt;
                    
                    return context.SaveChanges();
                }
            }
            return -1;
        }
    }
}