using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    public partial class UserManagerService : IRematchManager
    {
        public void NotRematch(string lobbyCode)
        {
            Match.Match match = matches[lobbyCode];
            match.DisconnectPlayerFromMatch();

            if (match.GetNumberOfPlayerInMatch() == 0)
            {
                matches.Remove(lobbyCode);
            }
        }

        public void Rematch(string lobbyCode, string username)
        {
            Match.Match match = matches[lobbyCode];
            IRematchManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IRematchManagerCallback>();

            LobbyPlayer playerToRematch = match.GetLobbyPlayerByUsername(username);

            match.DisconnectPlayerFromMatch();

            if (!lobbies.ContainsKey(lobbyCode))
            {
                CreateRematchLobby(lobbyCode, username);   
                currentUserCallbackChannel.NotifyHostOfRematch(lobbyCode);
            }
            else
            {
                currentUserCallbackChannel.NotifyRematch(lobbyCode);
            }

            TryRemoveMatchFromMatches(lobbyCode);
        }

        private void CreateRematchLobby(string lobbyCode, string username)
        {
            Match.Match match = matches[lobbyCode];
            LobbyInformation lobbyInformation = match.LobbyInformation;
            
            List<LobbyPlayer> players = new List<LobbyPlayer>();
            players.Add(match.GetLobbyPlayerByUsername(username));

            lobbies.Add(lobbyCode, (lobbyInformation, players));
        }

        private void TryRemoveMatchFromMatches(string lobbyCode)
        {
            Match.Match match = matches[lobbyCode];
            
            if(match.GetNumberOfPlayerInMatch() == 0)
            {
                matches.Remove(lobbyCode);
            }
        }
    }
}
