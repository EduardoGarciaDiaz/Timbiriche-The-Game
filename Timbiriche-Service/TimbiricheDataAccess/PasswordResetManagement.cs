using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;

namespace TimbiricheDataAccess
{
    public static class PasswordResetManagement
    {
        public static bool AddToken(PasswordResetTokens passwordResetToken)
        {
            int rowsAffected = -1;

            if (passwordResetToken != null)
            {
                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        context.PasswordResetTokens.Add(passwordResetToken);

                        rowsAffected = context.SaveChanges();
                    }
                }
                catch (EntityException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    throw new DataAccessException(ex.Message);
                }
                catch (SqlException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    throw new DataAccessException(ex.Message);
                }
                catch (Exception ex)
                {
                    HandlerExceptions.HandleFatalException(ex);
                    throw new DataAccessException(ex.Message);
                }
            }

            return rowsAffected > 0;
        }

        public static PasswordResetTokens GetPasswordResetTokenByIdPlayerAndToken(int playerId, int token)
        {
            try
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
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public static bool ChangePasswordById(int idPlayer, string password)
        {
            int rowsAffected = -1;
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var query = from player in context.Players
                                where player.idPlayer == idPlayer
                                select player;

                    var playerFound = query.FirstOrDefault();

                    if (playerFound != null)
                    {
                        Utils.PasswordHashManager passwordHashManager = new Utils.PasswordHashManager();
                        playerFound.password = passwordHashManager.HashPassword(password);
                        playerFound.salt = passwordHashManager.Salt;
                        rowsAffected = context.SaveChanges();
                    }

                    return rowsAffected > 0;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }
    }
}