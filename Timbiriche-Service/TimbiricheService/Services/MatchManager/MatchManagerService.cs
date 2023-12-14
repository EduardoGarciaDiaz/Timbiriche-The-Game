using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Exceptions;
using TimbiricheService.Match;

namespace TimbiricheService
{
    public partial class UserManagerService : IMatchManager
    {
        private static Dictionary<string, Match.Match> matches = new Dictionary<string, Match.Match>();

        public void RegisterToTheMatch(string lobbyCode, string username, string hexadecimalColor)
        {
            IMatchManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];

                foreach (var player in match.Players)
                {
                    if (player.Username == username)
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
        }

        public void EndTurn(string lobbyCode, Movement movement)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];

                NotifyMovement(lobbyCode, match, movement);

                int earnedPoints = movement.EarnedPoints;

                if (earnedPoints > 0)
                {
                    LobbyPlayer playerScoringPoints = match.GetTurnPlayer();

                    if (playerScoringPoints != null)
                    {
                        match.ScorePointsToPlayer(playerScoringPoints, earnedPoints);

                        foreach (LobbyPlayer player in match.Players.ToList())
                        {
                            try
                            {
                                player.MatchCallbackChannel.NotifyNewTurn(playerScoringPoints.Username);
                                player.MatchCallbackChannel.NotifyNewScoreboard(match.GetScoreboard());
                            }
                            catch (CommunicationException ex)
                            {
                                HandlerExceptions.HandleErrorException(ex);
                                LeftMatch(lobbyCode, player.Username);
                            }
                            catch (TimeoutException ex)
                            {
                                HandlerExceptions.HandleErrorException(ex);
                                LeftMatch(lobbyCode, player.Username);
                            }
                        } 
                    }
                }
                else
                {
                    NotifyTurns(lobbyCode);
                }
            }
        }

        private void NotifyMovement(string lobbyCode, Match.Match match, Movement movement)
        {
            foreach (LobbyPlayer player in match.Players.ToList())
            {
                LobbyPlayer currentTurnPlayer = match.GetTurnPlayer();

                if (currentTurnPlayer != null && player != currentTurnPlayer)
                {
                    try
                    {
                        player.MatchCallbackChannel.NotifyMovement(movement);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        LeftMatch(lobbyCode, player.Username);
                    }
                    catch (TimeoutException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        LeftMatch(lobbyCode, player.Username);
                    }
                }
            }
        }

        public void EndTurnWithoutMovement(string lobbyCode)
        {
            NotifyTurns(lobbyCode);
        }

        public void EndMatch(string lobbyCode)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                List<KeyValuePair<LobbyPlayer, int>> scoreboard = match.GetScoreboard();

                for (int playerPosition = 0; playerPosition < scoreboard.Count; playerPosition++)
                {
                    var player = match.Players.Find(p => p == scoreboard[playerPosition].Key);

                    if (player != null)
                    {
                        int coinsEarned = CoinsEarn.CalculateExtraCoins(playerPosition, scoreboard[playerPosition].Value);

                        UpdateCoins(player.Username, coinsEarned);

                        try
                        {
                            player.MatchCallbackChannel.NotifyEndOfTheMatch(scoreboard, coinsEarned);
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            match?.DeletePlayerFromMatch(player);
                        }
                        catch (TimeoutException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                    }
                }
            }
        }

        private void UpdateCoins(string username, int coinsEarned)
        {
            CoinsManagement coinsManagement = new CoinsManagement();

            try
            {
                coinsManagement.UpdateCoins(username, coinsEarned);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public void SendMessageToLobby(string lobbyCode, string senderUsername, string message, int idSenderPlayer)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];

                foreach (LobbyPlayer player in match.Players.ToList())
                {
                    if (player.Username != senderUsername)
                    {
                        try
                        {
                            player.MatchCallbackChannel.NotifyNewMessage(senderUsername, message, idSenderPlayer);
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                        catch (TimeoutException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                    }
                }
            }           
        }

        public void LeftMatch(string lobbyCode, string username)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                LobbyPlayer playerToDelete = null;
                LobbyPlayer lobbyPlayer = match.GetTurnPlayer();

                if (lobbyPlayer != null)
                {
                    bool isPlayerOnDuty = (lobbyPlayer.Username == username);

                    foreach (LobbyPlayer player in match.Players)
                    {
                        if (player.Username == username)
                        {
                            playerToDelete = player;
                        }
                    }

                    match?.DeletePlayerFromMatch(playerToDelete);
                    NotifyPlayerLeftLobby(lobbyCode, match, isPlayerOnDuty);
                }
            } 
        }

        private void NotifyPlayerLeftLobby(string lobbyCode, Match.Match match, bool isPlayerOnDuty)
        {
            if (match.GetNumberOfPlayerInMatch() == 1)
            {
                LobbyPlayer player = match.Players.FirstOrDefault();

                if(player != null)
                {
                    try
                    {
                        player.MatchCallbackChannel.NotifyOnlyPlayerInMatch();
                        matches.Remove(lobbyCode);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        LeftMatch(lobbyCode, player.Username);
                    }
                    catch (TimeoutException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        LeftMatch(lobbyCode, player.Username);
                    }
                }
            }
            else
            {
                NotifyPlayerNumberUpdate(lobbyCode, match, isPlayerOnDuty);
            }
        }

        private void NotifyTurns(string lobbyCode)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                match.NextTurn();

                LobbyPlayer temporalPlayer = match.GetTurnPlayer();

                if (temporalPlayer != null)
                {
                    foreach (LobbyPlayer player in match.Players.ToList())
                    {
                        try
                        {
                            player.MatchCallbackChannel.NotifyNewTurn(temporalPlayer.Username);
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                        catch (TimeoutException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                    }

                    matches[lobbyCode] = match; 
                }
            }
        }

        private void NotifyPlayerNumberUpdate(string lobbyCode, Match.Match match, bool isPlayerOnDuty)
        {
            LobbyPlayer newTurnPlayer = match.GetTurnPlayer();
            List<KeyValuePair<LobbyPlayer, int>> scoreboard = match.GetScoreboard();

            if (newTurnPlayer != null && scoreboard != null)
            {
                foreach (LobbyPlayer player in match.Players.ToList())
                {
                    try
                    {
                        player.MatchCallbackChannel.NotifyPlayerLeftMatch();
                        player.MatchCallbackChannel.NotifyNewScoreboard(scoreboard);

                        if (isPlayerOnDuty)
                        {
                            player.MatchCallbackChannel.NotifyNewTurn(newTurnPlayer.Username);
                        }
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        LeftMatch(lobbyCode, player.Username);
                    }
                    catch (TimeoutException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        LeftMatch(lobbyCode, player.Username);
                    }

                } 
            }
        }

        private void TryStartMatchIfAllConnected(string lobbyCode)
        {
            if (matches.ContainsKey(lobbyCode))
            {
                Match.Match match = matches[lobbyCode];
                LobbyInformation lobbyInformation = match.LobbyInformation;

                if (match.AreAllPlayersConnected())
                {
                    foreach (var player in match.Players.ToList())
                    {
                        try
                        {
                            LobbyPlayer lobbyPlayer = match.GetTurnPlayer();
                            List<KeyValuePair<LobbyPlayer, int>> scoreboard = match.GetScoreboard();

                            if (lobbyPlayer != null && scoreboard != null)
                            {
                                player.MatchCallbackChannel.NotifyFirstTurn(lobbyInformation.MatchDurationInMinutes,
                                lobbyInformation.TurnDurationInMinutes, lobbyPlayer.Username);
                                player.MatchCallbackChannel.NotifyNewScoreboard(scoreboard);
                            }
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                        catch (TimeoutException ex)
                        {
                            HandlerExceptions.HandleErrorException(ex);
                            LeftMatch(lobbyCode, player.Username);
                        }
                    }
                }
            }
        }
    }
}
