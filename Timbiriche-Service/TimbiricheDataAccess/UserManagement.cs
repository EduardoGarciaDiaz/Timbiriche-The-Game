﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;
using System.Data.Entity.Validation;
using System.Data.Entity.Core;

namespace TimbiricheDataAccess
{
    public class UserManagement
    {
        public int AddUser(Players player)
        {
            PasswordHashManager passwordHashManager = new PasswordHashManager();
            player.password = passwordHashManager.HashPassword(player.password);
            player.salt = passwordHashManager.Salt;

            Accounts account = null;
            if (player != null)
            {
                account = player.Accounts;

            }
            if (account != null)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var newAccount = context.Accounts.Add(account);
                    var newPlayer = context.Players.Add(player);
                    try
                    {
                        return context.SaveChanges();
                    } 
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Entity: { entityValidationErrors.Entry.Entity.GetType().Name}, Field: {validationError.PropertyName}, Error: { validationError.ErrorMessage}" );
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public int AddPlayerStyles(PlayerStyles playerStyle)
        {
            if (playerStyle != null)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var newPlayerStyle = context.PlayerStyles.Add(playerStyle);
                    try
                    {
                        return context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Entity: {entityValidationErrors.Entry.Entity.GetType().Name}, Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public int AddPlayerColors(PlayerColors playerColor)
        {
            if (playerColor != null)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var newPlayerColor = context.PlayerColors.Add(playerColor);
                    try
                    {
                        return context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Entity: {entityValidationErrors.Entry.Entity.GetType().Name}, Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public Players ValidateLoginCredentials(string username, string password)
        {
            using (var context = new TimbiricheDBEntities())
            {
            var playerData = context.Players.Include("Accounts").SingleOrDefault(player => player.username == username);
            
                if (playerData != null)
                {
                    PasswordHashManager passwordHashManager = new PasswordHashManager();
                    var playerPassword = playerData.password;
                    if (passwordHashManager.VerifyPassword(password, playerPassword))
                    {
                        return playerData;
                    }
                }
                 return null;
             }
        }

        public bool ExistUserIdenitifier(string identifier)
        {
            bool identifierExist = false;
            using(var context = new TimbiricheDBEntities())
            {
                var players = (from p in context.Players
                               where p.email == identifier || p.username == identifier
                               select p).ToList();
                identifierExist = players.Any();
            }
            return identifierExist;
        }

        public int GetIdPlayerByEmail(string email)
        {
            int idPlayer = 0;
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
            return idPlayer;
        }

        public int GetIdPlayerByUsername(string username)
        {
            int idPlayer = 0;
            using (var context = new TimbiricheDBEntities())
            {
                var player = context.Players
                    .FirstOrDefault(p => p.username == username);
                if (player != null) {
                    idPlayer = player.idPlayer;
                }
            }
            return idPlayer;
        }

        public string GetUsernameByIdPlayer(int idPlayer)
        {
            string username = string.Empty;
            using (var context = new TimbiricheDBEntities())
            {
                var player = context.Players
                    .FirstOrDefault(p => p.idPlayer == idPlayer);

                if (player != null)
                {
                    username = player.username;
                }
            }

            return username;
        }
    }
}
