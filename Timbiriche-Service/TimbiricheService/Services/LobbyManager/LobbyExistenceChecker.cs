using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    public partial class UserManagerService : ILobbyExistenceChecker
    {
        public bool ExistLobbyCode(string lobbyCode)
        {
            bool existLobby = false;

            if (lobbies.ContainsKey(lobbyCode))
            {
                existLobby = true;
            }

            return existLobby;
        }
    }
}
