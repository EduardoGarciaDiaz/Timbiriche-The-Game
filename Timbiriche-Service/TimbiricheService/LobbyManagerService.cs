using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    public partial class UserManagerService : ILobbyManager
    {
        private static Dictionary<string, (LobbyInformation, List<LobbyPlayer>)> lobbies = new Dictionary<string, (LobbyInformation, List<LobbyPlayer>)>();

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
            if (lobbies.ContainsKey(lobbyCode))
            {
                LobbyInformation lobbyInformation = lobbies[lobbyCode].Item1 as LobbyInformation;
                List<LobbyPlayer> players = lobbies[lobbyCode].Item2 as List<LobbyPlayer>;

                foreach(var player in players)
                {
                    player.CallbackChannel.NotifyStartOfMatch();
                }
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
            Console.WriteLine(lobbyCode);
            return lobbies.ContainsKey(lobbyCode) ? GenerateLobbyCode() : lobbyCode;
        }
    }
}

