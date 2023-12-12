using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;

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

            match.DisconnectPlayerFromMatch();

            if (!lobbies.ContainsKey(lobbyCode))
            {
                CreateRematchLobby(lobbyCode, username);
                try
                {
                    currentUserCallbackChannel.NotifyHostOfRematch(lobbyCode);
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    // TODO: Manage channels
                }
            }
            else
            {
                try
                {
                    currentUserCallbackChannel.NotifyRematch(lobbyCode);
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    // TODO: Manage channels
                }
            }

            TryRemoveMatchFromMatches(lobbyCode);
        }

        private void CreateRematchLobby(string lobbyCode, string username)
        {
            const int DEFAULT_SELECTED_COLOR = 0;
            Match.Match match = matches[lobbyCode];
            LobbyInformation lobbyInformation = match.LobbyInformation;
            
            List<LobbyPlayer> players = new List<LobbyPlayer>();
            LobbyPlayer hostPlayer = match.GetLobbyPlayerByUsername(username);
            hostPlayer.IdHexadecimalColor = DEFAULT_SELECTED_COLOR;

            players.Add(hostPlayer);
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
