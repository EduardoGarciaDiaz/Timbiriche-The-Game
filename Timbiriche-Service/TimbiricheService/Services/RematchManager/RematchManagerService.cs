using System;
using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    public partial class UserManagerService : IRematchManager
    {
        public void NotRematch(string lobbyCode)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                match.DisconnectPlayerFromMatch();

                if (match.GetNumberOfPlayerInMatch() == 0)
                {
                    matches.Remove(lobbyCode);
                }
            }
        }

        public void Rematch(string lobbyCode, string username)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                IRematchManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IRematchManagerCallback>();

                match.DisconnectPlayerFromMatch();

                try
                {
                    if (!lobbies.ContainsKey(lobbyCode))
                    {
                        CreateRematchLobby(lobbyCode, username);
                        currentUserCallbackChannel.NotifyHostOfRematch(lobbyCode);
                    }
                    else
                    {
                        currentUserCallbackChannel.NotifyRematch(lobbyCode);
                    }
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                }
                catch (TimeoutException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                }


                TryRemoveMatchFromMatches(lobbyCode);
            }
        }

        private void CreateRematchLobby(string lobbyCode, string username)
        {
            const int DEFAULT_SELECTED_COLOR = 0;

            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                LobbyInformation lobbyInformation = match.LobbyInformation;
            
                List<LobbyPlayer> players = new List<LobbyPlayer>();
                LobbyPlayer hostPlayer = match.GetLobbyPlayerByUsername(username);
                hostPlayer.IdHexadecimalColor = DEFAULT_SELECTED_COLOR;

                players.Add(hostPlayer);
                lobbies.Add(lobbyCode, (lobbyInformation, players));
            }
        }

        private void TryRemoveMatchFromMatches(string lobbyCode)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];

                if (match.GetNumberOfPlayerInMatch() == 0)
                {
                    matches.Remove(lobbyCode);
                }
            }
        }
    }
}
