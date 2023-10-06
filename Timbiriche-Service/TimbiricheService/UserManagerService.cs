using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    public class UserManagerService : IUserManager
    {
        public int AddUser(Account account, Player player)
        {
            Accounts newAccount = new Accounts();
            newAccount.name = account.name;
            newAccount.lastName = account.lastName;
            newAccount.surname = account.surname;
            newAccount.birthdate = account.birthdate;

            //TODO: Password hash

            Players newPlayer = new Players();
            newPlayer.username = player.username;
            newPlayer.email = player.email;
            newPlayer.password = player.password;

            UserManagement dataAccess = new UserManagement();
            return dataAccess.AddUser(newAccount, newPlayer);
        }

        public bool ValidateLoginCredentials(String username, String password)
        {
            //TODO: Pasword hash
            UserManagement dataAccess = new UserManagement();
            return dataAccess.ValidateLoginCredentials(username, password);
        }


        //TODO: Validate unique email and username
        public bool ValidateUniqueIdentifierUser(String identifier)
        {
            UserManagement dataAccess = new UserManagement();
            return dataAccess.ExistUserIdenitifier(identifier);
        }
    }

}
