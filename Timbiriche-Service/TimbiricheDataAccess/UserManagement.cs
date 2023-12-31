﻿using System;
using System.Linq;
using TimbiricheDataAccess.Utils;
using System.Data.Entity.Validation;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using TimbiricheDataAccess.Exceptions;

namespace TimbiricheDataAccess
{
    public class UserManagement
    {
        public int AddUser(Players player)
        {
            int rowsAffected = -1;
            PasswordHashManager passwordHashManager = new PasswordHashManager();
            player.password = passwordHashManager.HashPassword(player.password);
            player.salt = passwordHashManager.Salt;

            Accounts account = null;
            if (player.Accounts != null)
            {
                account = player.Accounts;

                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        context.Accounts.Add(account);
                        context.Players.Add(player);

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
            
            return rowsAffected;
        }

        public int AddPlayerStyles(PlayerStyles playerStyle)
        {
            int rowsAffected = -1;

            if (playerStyle != null)
            {
                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        context.PlayerStyles.Add(playerStyle);

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

            return rowsAffected;
        }

        public int AddPlayerColors(PlayerColors playerColor)
        {
            int rowsAffected = -1;

            if (playerColor != null)
            {
                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        context.PlayerColors.Add(playerColor);

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

            return rowsAffected;
        }

        public int AddToGlobalScoreboards(GlobalScores globalScore)
        {
            int rowsAffected = -1;

            if (globalScore != null)
            {
                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        context.GlobalScores.Add(globalScore);

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

            return rowsAffected;
        }

        public Players ValidateLoginCredentials(string username, string password)
        {
            Players playerData = null;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    playerData = context.Players.Include("Accounts").SingleOrDefault(player => player.username == username);
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

            if (playerData != null)
            {
                PasswordHashManager passwordHashManager = new PasswordHashManager();
                var playerPassword = playerData.password;

                if (!passwordHashManager.VerifyPassword(password, playerPassword))
                {
                    playerData = null;
                }
            }

            return playerData;
        }

        public Players GetPlayerByIdPlayer(int idPlayer)
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    Players playerFromDataBase = context.Players.Include("Accounts").SingleOrDefault(player => player.idPlayer == idPlayer);

                    return playerFromDataBase;
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

        public bool ExistUserIdenitifier(string identifier)
        {
            bool identifierExist = false;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var players = (from p in context.Players
                                   where p.email == identifier || p.username == identifier
                                   select p).ToList();
                    identifierExist = players.Any();
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

            return identifierExist;
        }

        public int GetIdPlayerByEmail(string email)
        {
            int idPlayer = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var query = from p in context.Players
                                where p.email == email
                                select p;
                    var player = query.SingleOrDefault();

                    if (player != null)
                    {
                        idPlayer = player.idPlayer;
                    }
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

            return idPlayer;
        }

        public int GetIdPlayerByUsername(string username)
        {
            int idPlayer = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players
                        .FirstOrDefault(p => p.username == username);

                    if (player != null)
                    {
                        idPlayer = player.idPlayer;
                    }
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

            return idPlayer;
        }

        public string GetUsernameByIdPlayer(int idPlayer)
        {
            string username = string.Empty;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players
                        .FirstOrDefault(p => p.idPlayer == idPlayer);

                    if (player != null)
                    {
                        username = player.username;
                    }
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

            return username;
        }

        public int UpdateAccount(Accounts editedAccount)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var account = context.Accounts.FirstOrDefault(a => a.idAccount == editedAccount.idAccount);

                    if (account != null)
                    {
                        account.name = editedAccount.name;
                        account.surname = editedAccount.surname;
                        account.lastName = editedAccount.lastName;
                        account.birthdate = editedAccount.birthdate;

                        rowsAffected = context.SaveChanges();
                    }
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

            return rowsAffected;
        }
    }
}
