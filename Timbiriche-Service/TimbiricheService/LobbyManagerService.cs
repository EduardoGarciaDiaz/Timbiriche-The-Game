using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheService.Match;

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

    public partial class UserManagerService : ILobbyManager
    {
        private static readonly Dictionary<string, (LobbyInformation, List<LobbyPlayer>)> lobbies = new Dictionary<string, (LobbyInformation, List<LobbyPlayer>)>();

        public void CreateLobby(LobbyInformation lobbyInformation, LobbyPlayer lobbyPlayer)
        {
            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            lobbyPlayer.CallbackChannel = currentUserCallbackChannel;

            List<LobbyPlayer> players = new List<LobbyPlayer>();
            players.Add(lobbyPlayer);

            string lobbyCode = GenerateLobbyCode(); 

            lobbies.Add(lobbyCode, (lobbyInformation, players));

            currentUserCallbackChannel.NotifyLobbyCreated(lobbyCode);
        }

        public void JoinLobby(String lobbyCode, LobbyPlayer lobbyPlayer)
        {
            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            lobbyPlayer.CallbackChannel = currentUserCallbackChannel;

            if (lobbies.ContainsKey(lobbyCode))
            {
                List<LobbyPlayer> playersInLobby = lobbies[lobbyCode].Item2;

                if(playersInLobby.Count < 5)
                {                          
                    int numOfPlayersInLobby = playersInLobby.Count;

                    foreach(var player in playersInLobby)
                    {
                        player.CallbackChannel.NotifyPlayerJoinToLobby(lobbyPlayer, numOfPlayersInLobby);
                    }

                    lobbyPlayer.CallbackChannel.NotifyPlayersInLobby(lobbyCode, playersInLobby);
                    playersInLobby.Add(lobbyPlayer);
                }
                else
                {
                    lobbyPlayer.CallbackChannel.NotifyLobbyIsFull();
                }
            }
            else
            {
                lobbyPlayer.CallbackChannel.NotifyLobbyDoesNotExist();
            }
        }

        public void StartMatch(string lobbyCode)
        {
            LobbyInformation lobbyInformation = lobbies[lobbyCode].Item1;
            List<LobbyPlayer> players = lobbies[lobbyCode].Item2;

            matches.Add(lobbyCode, new Match.Match(lobbyInformation, players));

            foreach(var player in players)
            {
                player.CallbackChannel.NotifyStartOfMatch();
            }

            lobbies.Remove(lobbyCode);
        }

        public void ExitLobby(String lobbyCode, String username)
        {
            List<LobbyPlayer> players = lobbies[lobbyCode].Item2;
            int hostIndex = 0;
            int eliminatedPlayerIndex = hostIndex;

            LobbyPlayer playerToEliminate = null;

            foreach (LobbyPlayer player in players)
            {
                if (player.Username.Equals(username))
                {
                    playerToEliminate = player;
                    break;
                }
                else
                {
                    eliminatedPlayerIndex++;
                }
            }

            players.Remove(playerToEliminate);
            lobbies[lobbyCode] = (lobbies[lobbyCode].Item1, players);

            foreach (var player in players)
            {
                if (eliminatedPlayerIndex != hostIndex)
                {
                    player.CallbackChannel.NotifyPlayerLeftLobby(username);
                }
                else
                {
                    player.CallbackChannel.NotifyHostPlayerLeftLobby();
                }
            }

            if(eliminatedPlayerIndex == hostIndex)
            {
                lobbies.Remove(lobbyCode);
            }
        }

        private string GenerateLobbyCode()
        {
            int length = 6;
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();

            char[] code = new char[length];

            for(int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            string lobbyCode = new string(code);

            return lobbies.ContainsKey(lobbyCode) ? GenerateLobbyCode() : lobbyCode;
        }
    }
}

