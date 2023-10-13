using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    public class UserManagerService : IUserManager
    {
        public int AddUser(Player player)
        {
            Account auxiliarAccount = player.accountFK;

            Accounts newAccount = new Accounts();
            newAccount.name = auxiliarAccount.name;
            newAccount.lastName = auxiliarAccount.lastName;
            newAccount.surname = auxiliarAccount.surname;
            newAccount.birthdate = auxiliarAccount.birthdate;

            Players newPlayer = new Players();
            newPlayer.username = player.username;
            newPlayer.email = player.email;
            newPlayer.password = player.password;
            newPlayer.coins = player.coins;
            newPlayer.status = player.status;
            newPlayer.salt = player.salt;
            newPlayer.Accounts = newAccount;

            UserManagement dataAccess = new UserManagement();
            return dataAccess.AddUser(newPlayer);
        }

        public Player ValidateLoginCredentials(String username, String password)
        {
            UserManagement dataAccess = new UserManagement();
            Players playerValidated = dataAccess.ValidateLoginCredentials(username, password);
            if (playerValidated != null)
            {
                Accounts accountValidated = playerValidated.Accounts;
                Account account = new Account
                {
                    name = accountValidated.name,
                    lastName = accountValidated.lastName,
                    surname = accountValidated.surname,
                    birthdate = accountValidated.birthdate
                };

                Player player = new Player
                {
                    idPlayer = playerValidated.idPlayer,
                    username = playerValidated.username,
                    email = playerValidated.email,
                    password = playerValidated.password,
                    coins = playerValidated.coins,
                    status = playerValidated.status,
                    salt = playerValidated.salt,
                    accountFK = account
                };

                return player;
            }

            return null;
        }

        public bool ValidateUniqueIdentifierUser(String identifier)
        {
            UserManagement dataAccess = new UserManagement();
            return dataAccess.ExistUserIdenitifier(identifier);
        }
    }

}
