using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public class UserManagement
    {
        public void AddUser(Accounts account, Players player)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var newAccount = context.Accounts.Add(account);
                var newPlayer = context.Players.Add(player);
                context.SaveChanges();
            }
        }
    }
}
