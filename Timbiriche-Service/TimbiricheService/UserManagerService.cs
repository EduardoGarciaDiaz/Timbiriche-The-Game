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
        public int addUser(Account account, Player player)
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

            UserManagement DAO = new UserManagement();
            return DAO.addUser(newAccount, newPlayer);
        }

        public bool validateCredentials(String username, String password)
        {
            //TODO: Pasword hash
            UserManagement DAO = new UserManagement();
            return DAO.validateCredentials(username, password);
        }


        //TODO: Validate unique email and username
        public bool ValidateUniqueIdentifierUser(String identifier)
        {
            UserManagement DAO = new UserManagement();
            return DAO.ExistUserIdenitifier(identifier);
        }
    }

}
