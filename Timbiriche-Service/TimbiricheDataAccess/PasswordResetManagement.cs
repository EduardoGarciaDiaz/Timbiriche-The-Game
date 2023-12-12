using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheDataAccess
{
    public static class PasswordResetManagement
    {
        public static bool AddToken(PasswordResetTokens passwordResetToken)
        {
            int rowsAffected = 0;

            if (passwordResetToken != null)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    context.PasswordResetTokens.Add(passwordResetToken);

                    rowsAffected = context.SaveChanges();
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

        public static bool ChangePasswordById(int idPlayer, string password)
        {
            using(var context = new TimbiricheDBEntities())
            {
                var query = from player in context.Players
                            where player.idPlayer == idPlayer
                            select player;

                var playerFound = query.FirstOrDefault();
                int rowsAffected = 0;

                if(playerFound != null)
                {
                    Utils.PasswordHashManager passwordHashManager = new Utils.PasswordHashManager();
                    playerFound.password = passwordHashManager.HashPassword(password);
                    playerFound.salt = passwordHashManager.Salt;
                    rowsAffected = context.SaveChanges();
                }

                return rowsAffected > 0;
            }
        }
    }
}