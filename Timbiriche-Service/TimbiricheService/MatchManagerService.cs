using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
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
            int matchDurationInMinutes = match.LobbyInformation.MatchDurationInMinutes;
            int turnDurationInMinutes = match.LobbyInformation.TurnDurationInMinutes;

            string firstTurnUsername = match.GetTurnPlayer().Username;

            foreach (var player in match.Players)
            {
                if(player.Username == username)
                {
                    player.MatchCallbackChannel = currentUserCallbackChannel;
                    player.MatchCallbackChannel.NotifyFirstTurn(matchDurationInMinutes, turnDurationInMinutes, firstTurnUsername);
                    player.MatchCallbackChannel.NotifyNewScoreboard(match.GetScoreboard());
                }
            }

            matches[lobbyCode] = match;
        }

        public void EndTurn(string lobbyCode, string typeLine, int row, int column, int points)
        {
            Match.Match match = matches[lobbyCode];

            foreach (LobbyPlayer player in match.Players)
            {
                if (player != match.GetTurnPlayer())
                {
                        player.MatchCallbackChannel.NotifyMovement(typeLine, row, column);
                }
            }

            if(points > 0)
            {
                LobbyPlayer playerScoringPoints = match.GetTurnPlayer();
                match.ScorePointsToPlayer(playerScoringPoints, points);
                matches[lobbyCode] = match;

                foreach (LobbyPlayer player in match.Players)
                {
                    player.MatchCallbackChannel.NotifyNewTurn(playerScoringPoints.Username);
                    player.MatchCallbackChannel.NotifyNewScoreboard(match.GetScoreboard());
                }
            }
            else
            {
                NotifyTurns(lobbyCode);
            }
        }

        public void EndTurnWithoutMovement(string lobbyCode)
        {
            NotifyTurns(lobbyCode);
        }

        public void EndMatch(string lobbyCode)
        {
            Match.Match match = matches[lobbyCode];
            List<KeyValuePair<string, int>> scoreboard = match.GetScoreboard();

            for(int playerPosition = 0; playerPosition < scoreboard.Count; playerPosition++)
            {
                var player = match.Players.FirstOrDefault(p => p.Username == scoreboard[playerPosition].Key);

                if(player != null)
                {
                    int coinsEarned = CoinsEarn.CalculateExtraCoins(playerPosition, scoreboard[playerPosition].Value);

                    CoinsManagement coinsManagement = new CoinsManagement();
                    coinsManagement.UpdateCoins(player.Username, coinsEarned);

                    player.MatchCallbackChannel.NotifyEndOfTheMatch(scoreboard, coinsEarned);
                }
            }
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

        private void NotifyTurns(string lobbyCode)
        {
            Match.Match match = matches[lobbyCode];
            match.NextTurn();

            LobbyPlayer temporalPlayer = match.GetTurnPlayer();

            foreach (LobbyPlayer player in match.Players)
            {
                player.MatchCallbackChannel.NotifyNewTurn(temporalPlayer.Username);
            }

            matches[lobbyCode] = match;
        }
    }
}
