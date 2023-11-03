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
        Dictionary<string, Match.Match> matches = new Dictionary<string, Match.Match> ();

        public void MatchSetup(String lobbyCode, LobbyInformation lobbyInformation, List<LobbyPlayer> players)
        {
            foreach(LobbyPlayer player in players)
            {
                IMatchManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();
                player.MatchCallbackChannel = currentUserCallbackChannel;
            }

            Match.Match match = new Match.Match(lobbyInformation, players);
            matches.Add(lobbyCode, match);
        }

        public void EndTurn(string lobbyCode, string typeLine, int row, int column)
        {
            if(matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];

                foreach(LobbyPlayer player in match.Players)
                {
                    if(player != match.GetTurnPlayer())
                    {
                        player.MatchCallbackChannel.NotifyMovement(typeLine, row, column);
                    }
                }

                match.NextTurn();
                LobbyPlayer temporalPlayer = match.GetTurnPlayer();

                foreach(LobbyPlayer player in match.Players)
                {
                    player.MatchCallbackChannel.NotifyNewTurn(temporalPlayer.Username);
                }
            }
        }
    }
}
