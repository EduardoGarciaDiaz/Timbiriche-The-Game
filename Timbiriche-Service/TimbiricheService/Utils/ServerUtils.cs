using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    public partial class UserManagerService 
    {
        private void DeletePlayerFromOnlineFriendshipDictionary(string username)
        {
            onlineFriendship.Remove(username);
        }

        private void DeletePlayerFromOnlineUsersDictionary(string username)
        {
            onlineUsers.Remove(username);
            UnregisterUserToOnlineUsers(username);
        }

        private void DeletePlayerFromGlobalScoreRealTimeDictionary(string username)
        {
            globalScoreRealTime.Remove(username);
        }

        // TODO
        private void DeletePlayerFromMatchesDictionary()
        {

        }

        // TODO
        private void DeletePlayerFromLobbiesDictionary(string lobbyCode, string username)
        {
        }



    }
}
