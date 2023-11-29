using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;
using System.Data.Entity.Validation;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.ServiceModel;
using TimbiricheDataAccess.Exceptions;

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

        public int AddToGlobalScoreboards(GlobalScores globalScore)
        {
            int response = -1;
            if (globalScore != null)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var newGlobalScore = context.GlobalScores.Add(globalScore);
                    try
                    {
                        response = context.SaveChanges();
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
            return response;
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
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex.Message);
            }

            if (playerData != null)
            {
                PasswordHashManager passwordHashManager = new PasswordHashManager();
                var playerPassword = playerData.password;

                if (playerData.Accounts != null)
                {
                    var accountEntry = playerData.Accounts;
                }

                if (!passwordHashManager.VerifyPassword(password, playerPassword))
                {
                    playerData = null;
                }
            }

            return playerData;
        }

        public Players GetPlayerByIdPlayer(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                Players playerFromDataBase = context.Players.Include("Accounts").SingleOrDefault(player => player.idPlayer == idPlayer);

                return playerFromDataBase;
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

        public int UpdateAccount(Accounts editedAccount)
        {
            int rowsAffected = -1;

            using (var context = new TimbiricheDBEntities()){
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

            return rowsAffected;
        }
    }
}
