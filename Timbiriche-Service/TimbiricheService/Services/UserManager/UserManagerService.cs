using System;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IUserManager
    {
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                    TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };

                    throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public Player ValidateLoginCredentials(string username, string password)
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public bool ValidateUniqueIdentifierUser(string identifier)
        {
            UserManagement dataAccess = new UserManagement();

            try
            {
                return dataAccess.ExistUserIdenitifier(identifier);
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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