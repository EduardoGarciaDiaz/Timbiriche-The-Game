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

        public void RegisterToTheMatch(string lobbyCode, string username, string hexadecimalColor)
        {
            IMatchManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            Match.Match match = matches[lobbyCode];

            foreach (var player in match.Players)
            {
                if(player.Username == username)
                {
                    player.MatchCallbackChannel = currentUserCallbackChannel;
                    player.StylePath = GetStylePath(player.IdStylePath);
                    player.HexadecimalColor = hexadecimalColor;
                    match.SetConnectedUser(player.Username);
                }
            }

            matches[lobbyCode] = match;

            TryStartMatchIfAllConnected(lobbyCode);
        }

        public void EndTurn(string lobbyCode, Movement movement)
        {
            Match.Match match = matches[lobbyCode];

            foreach (LobbyPlayer player in match.Players)
            {
                if (player != match.GetTurnPlayer())
                {
                        player.MatchCallbackChannel.NotifyMovement(movement);
                }
            }

            int earnedPoints = movement.EarnedPoints;

            if(earnedPoints > 0)
            {
                LobbyPlayer playerScoringPoints = match.GetTurnPlayer();
                match.ScorePointsToPlayer(playerScoringPoints, earnedPoints);
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
            List<KeyValuePair<LobbyPlayer, int>> scoreboard = match.GetScoreboard();

            for(int playerPosition = 0; playerPosition < scoreboard.Count; playerPosition++)
            {
                var player = match.Players.FirstOrDefault(p => p == scoreboard[playerPosition].Key);

                if(player != null)
                {
                    int coinsEarned = CoinsEarn.CalculateExtraCoins(playerPosition, scoreboard[playerPosition].Value);

                    CoinsManagement coinsManagement = new CoinsManagement();
                    coinsManagement.UpdateCoins(player.Username, coinsEarned);

                    try
                    {
                        player.MatchCallbackChannel.NotifyEndOfTheMatch(scoreboard, coinsEarned);
                    } 
                    catch(FaultException ex)
                    {
                        Console.WriteLine($"Fault Exception: {ex.Message}");
                        Console.WriteLine($"Detail: {ex.StackTrace}");
                    }
                }
            }
        }

        public void SendMessageToLobby(string lobbyCode, string senderUsername, string message, int idSenderPlayer)
        {
            Match.Match match = matches[lobbyCode];

            foreach (LobbyPlayer player in match.Players)
            {
                if (player.Username != senderUsername)
                {
                    player.MatchCallbackChannel.NotifyNewMessage(senderUsername, message, idSenderPlayer);
                }
            }
        }

        public void LeftMatch(string lobbyCode, string username)
        {
            Match.Match match = matches[lobbyCode];
            LobbyPlayer playerToDelete = null;
            bool isPlayerOnDuty = (match.GetTurnPlayer().Username == username) ? true : false;

            foreach (LobbyPlayer player in match.Players)
            {
                if (player.Username == username)
                {
                    playerToDelete = player;
                }
            }

            match.DeletePlayerFromMatch(playerToDelete);
            matches[lobbyCode] = match;

            if(match.GetNumberOfPlayerInMatch() == 1)
            {
                LobbyPlayer player = match.Players.FirstOrDefault();
                player.MatchCallbackChannel.NotifyOnlyPlayerInMatch();

                matches.Remove(lobbyCode);
            }
            else
            {
                NotifyPlayerNumberUpdate(match, isPlayerOnDuty);
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

        private void NotifyPlayerNumberUpdate(Match.Match match, bool isPlayerOnDuty)
        {
            LobbyPlayer newTurnPlayer = match.GetTurnPlayer();
            List<KeyValuePair<LobbyPlayer, int>> scoreboard = match.GetScoreboard();

            foreach (LobbyPlayer player in match.Players)
            {
                player.MatchCallbackChannel.NotifyPlayerLeftMatch();
                player.MatchCallbackChannel.NotifyNewScoreboard(scoreboard);

                if (isPlayerOnDuty)
                {
                    player.MatchCallbackChannel.NotifyNewTurn(newTurnPlayer.Username);
                }
            }
        }

        private void TryStartMatchIfAllConnected(string lobbyCode)
        {
            Match.Match match = matches[lobbyCode];
            LobbyInformation lobbyInformation = match.LobbyInformation;

            if (match.AreAllPlayersConnected())
            {
                foreach (var player in match.Players)
                {
                    player.MatchCallbackChannel.NotifyFirstTurn(lobbyInformation.MatchDurationInMinutes, lobbyInformation.TurnDurationInMinutes,
                                                                match.GetTurnPlayer().Username);
                    player.MatchCallbackChannel.NotifyNewScoreboard(match.GetScoreboard());
                }
            }
        }
    }
}
