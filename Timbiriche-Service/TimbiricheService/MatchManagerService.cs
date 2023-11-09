using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheService.Match;

namespace TimbiricheService
{
    public partial class UserManagerService : IMatchManager
    {
        private static Dictionary<string, Match.Match> matches = new Dictionary<string, Match.Match>();

        public void RegisterToTheMatch(string lobbyCode, string username)
        {
            IMatchManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            Match.Match match = matches[lobbyCode];
               
            foreach(var player in match.Players)
            {
                if(player.Username == username)
                {
                    player.MatchCallbackChannel = currentUserCallbackChannel;
                }
            }

            matches[lobbyCode] = match;
        }

        public void EndTurn(string lobbyCode, string typeLine, int row, int column)
        {
            Match.Match match = matches[lobbyCode];

            foreach (LobbyPlayer player in match.Players)
            {
                if (player != match.GetTurnPlayer())
                {
                    player.MatchCallbackChannel.NotifyMovement(typeLine, row, column);
                }
            }

            match.NextTurn();
            LobbyPlayer temporalPlayer = match.GetTurnPlayer();

            foreach (LobbyPlayer player in match.Players)
            {
                player.MatchCallbackChannel.NotifyNewTurn(temporalPlayer.Username);
            }

            matches[lobbyCode] = match;
        }

        public void SendMessageToLobby(string lobbyCode, string senderUsername, string message)
        {
            Match.Match match = matches[lobbyCode];

            foreach (var player in match.Players)
            {
                if (player.Username != senderUsername)
                {
                    player.MatchCallbackChannel.NotifyNewMessage(senderUsername, message);
                }
            }
        }
    }
}
