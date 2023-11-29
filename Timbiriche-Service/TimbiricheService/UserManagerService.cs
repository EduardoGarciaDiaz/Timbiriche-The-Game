using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Exceptions;
using TimbiricheService.Utils;

namespace TimbiricheService
{
    public partial class UserManagerService : IUserManager
    {
        private ILogger _logger = LoggerManager.GetLogger();
        private const int DEFAULT_ID_COLOR_SELECTED = 0;

        public int AddUser(Player player)
        {
            Account auxiliarAccount = player.AccountFK;

            Accounts newAccount = new Accounts();
            newAccount.name = auxiliarAccount.Name;
            newAccount.lastName = auxiliarAccount.LastName;
            newAccount.surname = auxiliarAccount.Surname;
            newAccount.birthdate = auxiliarAccount.Birthdate;

            Players newPlayer = new Players();
            newPlayer.username = player.Username;
            newPlayer.email = player.Email;
            newPlayer.password = player.Password;
            newPlayer.coins = player.Coins;
            newPlayer.status = player.Status;
            newPlayer.salt = player.Salt;
            newPlayer.idColorSelected = player.IdColorSelected;
            newPlayer.idStyleSelected = player.IdStyleSelected;
            newPlayer.Accounts = newAccount;

            UserManagement dataAccess = new UserManagement();
            int rowsAffected = dataAccess.AddUser(newPlayer);

            if (rowsAffected > 0)
            {
                int rowsAffectedPlayerStyles = SetDefaultStyle(newPlayer);
                if  (rowsAffectedPlayerStyles > 0)
                {
                    SetDefaultColors(newPlayer);
                    AddNewPlayerToGlobalScores(newPlayer.idPlayer);
                }
            }
            return rowsAffected;
        }

        private int SetDefaultStyle(Players newPlayer)
        {
            PlayerStyles playerStyle = new PlayerStyles();
            playerStyle.idPlayer = newPlayer.idPlayer;
            playerStyle.idStyle = 1;
            UserManagement dataAccess = new UserManagement();
            return dataAccess.AddPlayerStyles(playerStyle);
        }

        private void SetDefaultColors(Players newPlayer)
        {
            PlayerColors playerColor = new PlayerColors();
            playerColor.idPlayer = newPlayer.idPlayer;
            for (int i = 1 ; i < 5 ; i++)
            {
                playerColor.idColor = i;
                UserManagement dataAccess = new UserManagement();
                dataAccess.AddPlayerColors(playerColor);
            }
        }

        private void AddNewPlayerToGlobalScores(int idPlayer)
        {
            const int DEFAULT_NUMBER_OF_WINS = 0;
            UserManagement dataAccess = new UserManagement();
            GlobalScores newScore = new GlobalScores();
            newScore.idPlayer = idPlayer;
            newScore.winsNumber = DEFAULT_NUMBER_OF_WINS;
            dataAccess.AddToGlobalScoreboards(newScore);
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
                    Account account = new Account
                    {
                        IdAcccount = accountValidated.idAccount,
                        Name = accountValidated.name,
                        LastName = accountValidated.lastName,
                        Surname = accountValidated.surname,
                        Birthdate = accountValidated.birthdate
                    };

                    player = new Player
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

        public Player GetPlayerByIdPlayer(int idPlayer)
        {
            UserManagement dataAccess = new UserManagement();
            Players playerFromDataBase = dataAccess.GetPlayerByIdPlayer(idPlayer);

            Player player = null;

            if (playerFromDataBase != null)
            {
                Accounts accountValidated = playerFromDataBase.Accounts;
                Account account = new Account
                {
                    Name = accountValidated.name,
                    LastName = accountValidated.lastName,
                    Surname = accountValidated.surname,
                    Birthdate = accountValidated.birthdate
                };

                player = new Player
                {
                    IdPlayer = playerFromDataBase.idPlayer,
                    Username = playerFromDataBase.username,
                    Email = playerFromDataBase.email,
                    Password = playerFromDataBase.password,
                    Coins = (int)playerFromDataBase.coins,
                    Status = playerFromDataBase .status,
                    Salt = playerFromDataBase.salt,
                    IdColorSelected = DEFAULT_ID_COLOR_SELECTED,
                    IdStyleSelected = (int)playerFromDataBase.idStyleSelected,
                    AccountFK = account,
                };
            }

            return player;
        }

        public bool ValidateUniqueIdentifierUser(String identifier)
        {
            UserManagement dataAccess = new UserManagement();
            return dataAccess.ExistUserIdenitifier(identifier);
        }

        public int UpdateAccount(Account account)
        {
            Accounts editedAccount = new Accounts();
            editedAccount.idAccount = account.IdAcccount;
            editedAccount.name = account.Name;
            editedAccount.lastName = account.LastName;
            editedAccount.surname = account.Surname;
            editedAccount.birthdate = account.Birthdate;

            UserManagement dataAccess = new UserManagement();
            int rowsAffected = dataAccess.UpdateAccount(editedAccount);

            return rowsAffected;
        }

        public string GetUsernameByIdPlayer(int idPlayer)
        {
            UserManagement dataAccess = new UserManagement();
            string username = dataAccess.GetUsernameByIdPlayer(idPlayer);
            return username;
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
                
                currentUserCallbackChannel.NotifyOnlineFriends(onlineFriends);

                onlineUsers.Add(username, currentUserCallbackChannel);

                foreach (var user in onlineUsers)
                {
                    if (user.Key != username && IsFriend(idPlayer, user.Key))
                    {
                        user.Value.NotifyUserLoggedIn(username); 
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

                foreach (var user in onlineUsers)
                {
                    user.Value.NotifyUserLoggedOut(username);
                }
            }
        }
    }
}