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

            currentUserCallbackChannel.NotifyLobbyCreated();
        }

        public void JoinLobby(String lobbyCode, LobbyPlayer lobbyPlayer)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
                lobbyPlayer.CallbackChannel = currentUserCallbackChannel;

                List<LobbyPlayer> playersInLobby = lobbies[lobbyCode].Item2;

                if(playersInLobby.Count < 4)
                {
                    playersInLobby.Add(lobbyPlayer);
                    lobbyPlayer.CallbackChannel.NotifyPlayersInLobby(playersInLobby);

                    foreach(var player in playersInLobby)
                    {
                        player.CallbackChannel.NotifyPlayerJoinToLobby(lobbyPlayer);
                    }
                }
            }
        }

        public void StartMatch()
        {
            throw new NotImplementedException();
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

