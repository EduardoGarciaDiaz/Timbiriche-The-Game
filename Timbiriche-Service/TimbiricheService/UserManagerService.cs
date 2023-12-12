﻿using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IUserManager
    {
        private ILogger _logger = LoggerManager.GetLogger();
        private const int DEFAULT_ID_COLOR_SELECTED = 0;

        public int AddUser(Player player)
        {
            Account auxiliarAccount = player.AccountFK;

            Accounts newAccount = new Accounts
            {
                name = auxiliarAccount.Name,
                lastName = auxiliarAccount.LastName,
                surname = auxiliarAccount.Surname,
                birthdate = auxiliarAccount.Birthdate
            };

            Players newPlayer = new Players
            {
                username = player.Username,
                email = player.Email,
                password = player.Password,
                coins = player.Coins,
                status = player.Status,
                salt = player.Salt,
                idColorSelected = player.IdColorSelected,
                idStyleSelected = player.IdStyleSelected,
                Accounts = newAccount
            };

            UserManagement dataAccess = new UserManagement();
            try
            {
                int rowsAffected = dataAccess.AddUser(newPlayer);

                if (rowsAffected > 0)
                {
                    int rowsAffectedPlayerStyles = SetDefaultStyle(newPlayer);

                    if (rowsAffectedPlayerStyles > 0)
                    {
                        SetDefaultColors(newPlayer);
                        AddNewPlayerToGlobalScores(newPlayer.idPlayer);
                    }
                }

                return rowsAffected;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        private int SetDefaultStyle(Players newPlayer)
        {            
            UserManagement dataAccess = new UserManagement();
            PlayerStyles playerStyle = new PlayerStyles
            {
                idPlayer = newPlayer.idPlayer,
                idStyle = 1
            };

            try
            {
                return dataAccess.AddPlayerStyles(playerStyle);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        private void SetDefaultColors(Players newPlayer)
        {
            PlayerColors playerColor = new PlayerColors();
            playerColor.idPlayer = newPlayer.idPlayer;

            for (int i = 1 ; i < 5 ; i++)
            {
                playerColor.idColor = i;
                UserManagement dataAccess = new UserManagement();

                try
                {
                    dataAccess.AddPlayerColors(playerColor);

                }
                catch (DataAccessException ex)
                {
                    TimbiricheServerException exceptionResponse = new TimbiricheServerException
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };

                    throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
                }
            }
        }

        private void AddNewPlayerToGlobalScores(int idPlayer)
        {
            const int DEFAULT_NUMBER_OF_WINS = 0;
            UserManagement dataAccess = new UserManagement();
            GlobalScores newScore = new GlobalScores();
            newScore.idPlayer = idPlayer;
            newScore.winsNumber = DEFAULT_NUMBER_OF_WINS;

            try
            {
                dataAccess.AddToGlobalScoreboards(newScore);

            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public Player ValidateLoginCredentials(String username, String password)
        {
            Player player = null;
            UserManagement dataAccess = new UserManagement();

            try
            {
                Players playerValidated = dataAccess.ValidateLoginCredentials(username, password);

                if (playerValidated != null)
                {
                    Accounts accountValidated = playerValidated.Accounts;
                    Account account = CreateAccount(accountValidated);
                    player = CreatePlayer(playerValidated, account);                    
                }
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }

            return player;
        }

        private Account CreateAccount(Accounts accountValidated)
        {
            Account account = new Account
            {
                IdAccount = accountValidated.idAccount,
                Name = accountValidated.name,
                LastName = accountValidated.lastName,
                Surname = accountValidated.surname,
                Birthdate = accountValidated.birthdate
            };

            return account;
        }

        private Player CreatePlayer(Players playerValidated, Account account)
        {
            Player player = new Player
            {
                IdPlayer = playerValidated.idPlayer,
                Username = playerValidated.username,
                Email = playerValidated.email,
                Password = playerValidated.password,
                Coins = (int)playerValidated.coins,
                Status = playerValidated.status,
                Salt = playerValidated.salt,
                IdColorSelected = DEFAULT_ID_COLOR_SELECTED,
                IdStyleSelected = (int)playerValidated.idStyleSelected,
                AccountFK = account,
            };

            return player;
        }

        public Player GetPlayerByIdPlayer(int idPlayer)
        {
            UserManagement dataAccess = new UserManagement();
            Player player = null;

            try
            {
                Players playerFromDataBase = dataAccess.GetPlayerByIdPlayer(idPlayer);

                if (playerFromDataBase != null)
                {
                    Accounts accountValidated = playerFromDataBase.Accounts;
                    Account account = CreateAccount(accountValidated);

                    player = CreatePlayer(playerFromDataBase, account);
                }

                return player;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public bool ValidateUniqueIdentifierUser(String identifier)
        {
            UserManagement dataAccess = new UserManagement();

            try
            {
                return dataAccess.ExistUserIdenitifier(identifier);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public int UpdateAccount(Account account)
        {            
            UserManagement dataAccess = new UserManagement();
            Accounts editedAccount = new Accounts
            {
                idAccount = account.IdAccount,
                name = account.Name,
                lastName = account.LastName,
                surname = account.Surname,
                birthdate = account.Birthdate
            };

            try
            {
                return dataAccess.UpdateAccount(editedAccount);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }

            
        }

        public string GetUsernameByIdPlayer(int idPlayer)
        {
            UserManagement dataAccess = new UserManagement();

            try
            {
                return dataAccess.GetUsernameByIdPlayer(idPlayer);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            } 
        }

        public bool ValidateIsUserAlreadyOnline(string username)
        {
            bool isAlreadyOnline = true;

            if (!onlineUsers.ContainsKey(username))
            {
                isAlreadyOnline = false;
            }

            return isAlreadyOnline;
        }

        public int GetIdPlayerByUsername(string username)
        {
            UserManagement dataAccess = new UserManagement();
            try
            {
                return dataAccess.GetIdPlayerByUsername(username);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class UserManagerService : IOnlineUsersManager
    {
        private static Dictionary<string, IUserManagerCallback> onlineUsers = new Dictionary<string, IUserManagerCallback>();

        public void RegisterUserToOnlineUsers(int idPlayer, string username)
        {
            if (!onlineUsers.ContainsKey(username))
            {
                IUserManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IUserManagerCallback>();

                List<string> onlineUsernames = onlineUsers.Keys.ToList();
                List<string> onlineFriends = new List<string>();

                foreach(string onlineUsername in onlineUsernames)
                {
                    if (IsFriend(idPlayer, onlineUsername))
                    {
                        onlineFriends.Add(onlineUsername);
                    }
                }

                try
                {
                    currentUserCallbackChannel.NotifyOnlineFriends(onlineFriends);
                    onlineUsers.Add(username, currentUserCallbackChannel);
                }   
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    UnregisterUserToOnlineUsers(username);
                }

                foreach (var user in onlineUsers.ToList())
                {
                    if (user.Key != username && IsFriend(idPlayer, user.Key))
                    {
                        try
                        {
                            user.Value.NotifyUserLoggedIn(username);
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerException.HandleErrorException(ex);
                            UnregisterUserToOnlineUsers(username);
                        }
                    }
                }
            }
        }

        private bool IsFriend(int currentIdPlayer, string onlineUsername)
        {
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            UserManagement userDataAccess = new UserManagement();
            int idOnlinePlayer = userDataAccess.GetIdPlayerByUsername(onlineUsername);

            bool isFriend = friendRequestDataAccess.IsFriend(currentIdPlayer, idOnlinePlayer);

            return isFriend;
        }

        public void UnregisterUserToOnlineUsers(string username)
        {
            if (onlineUsers.ContainsKey(username))
            {
                onlineUsers.Remove(username);
                onlineFriendship.Remove(username);

                foreach (var user in onlineUsers.ToList())
                {
                    try
                    {
                        user.Value.NotifyUserLoggedOut(username);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerException.HandleErrorException(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                }
            }
        }
    }
}