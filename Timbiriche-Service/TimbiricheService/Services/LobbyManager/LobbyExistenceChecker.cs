using System.Collections.Generic;
using System.Linq;

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

        public bool ExistUsernameInLobby(string lobbyCode, string username)
        {
            bool existsUsernameInLobby = false;

            if (lobbies.ContainsKey(lobbyCode))
            {
                List<LobbyPlayer> players = lobbies[lobbyCode].Item2;

                LobbyPlayer player = players.FirstOrDefault(p => p.Username == username);

                existsUsernameInLobby = (player != null);
            }

            return existsUsernameInLobby;
        }
    }
}
