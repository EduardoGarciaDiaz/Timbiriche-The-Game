using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public class UserManagement
    {
        public int addUser(Accounts account, Players player)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var newAccount = context.Accounts.Add(account);
                var newPlayer = context.Players.Add(player);
                return context.SaveChanges();
            }
        }

        public bool validateCredentials(String username, String password)
        {
            using(var context = new TimbiricheDBEntities())
            {
                var query = from player in context.Players
                            where player.username == username && player.password == password
                            select player;
                bool userExists = query.Any();

                return userExists;
            }
        }

        public bool ExistUserIdenitifier(String identifier)
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

    }
}
