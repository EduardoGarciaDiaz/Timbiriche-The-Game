﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    public partial class UserManagerService : ILobbyManager
    {
        private static readonly Dictionary<string, (LobbyInformation, List<LobbyPlayer>)> lobbies = 
            new Dictionary<string, (LobbyInformation, List<LobbyPlayer>)>();

        public void CreateLobby(LobbyInformation lobbyInformation, LobbyPlayer lobbyPlayer)
        {
            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            lobbyPlayer.CallbackChannel = currentUserCallbackChannel;

            List<LobbyPlayer> players = new List<LobbyPlayer>
            {
                lobbyPlayer
            };

            string lobbyCode = GenerateLobbyCode(); 

            lobbies.Add(lobbyCode, (lobbyInformation, players));

            try
            {
                currentUserCallbackChannel.NotifyLobbyCreated(lobbyCode);
            }
            catch (CommunicationException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                PerformExitLobby(lobbyCode, lobbyPlayer.Username, false);
            }
            catch (TimeoutException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                PerformExitLobby(lobbyCode, lobbyPlayer.Username, false);
            }
        }

        public void JoinLobbyAsHost(string lobbyCode)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
                List<LobbyPlayer> players = lobbies[lobbyCode].Item2;
                LobbyPlayer hostPlayer = players[0];

                hostPlayer.CallbackChannel = currentUserCallbackChannel;

                try
                {
                    currentUserCallbackChannel.NotifyLobbyCreated(lobbyCode);
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    PerformExitLobby(lobbyCode, hostPlayer.Username, false);
                }
            }
        }

        public void JoinLobby(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            lobbyPlayer.CallbackChannel = currentUserCallbackChannel;
            int maxSizePlayers = 4;

            try
            {
                if (lobbies.ContainsKey(lobbyCode))
                {
                    List<LobbyPlayer> playersInLobby = lobbies[lobbyCode].Item2;
                    int numOfPlayersInLobby = playersInLobby.Count;

                    if (numOfPlayersInLobby < maxSizePlayers)
                    {
                        lobbyPlayer.CallbackChannel.NotifyPlayersInLobby(lobbyCode, playersInLobby);
                        NotifyPlayerJoinToLobby(playersInLobby, lobbyPlayer, numOfPlayersInLobby, lobbyCode);
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
            catch (CommunicationException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
            }
        }

        private void NotifyPlayerJoinToLobby(List<LobbyPlayer> playersInLobby, LobbyPlayer playerEntering, int numOfPlayersInLobby, string lobbyCode)
        {
            foreach (var player in playersInLobby.ToList())
            {
                try
                {
                    player.CallbackChannel.NotifyPlayerJoinToLobby(playerEntering, numOfPlayersInLobby);
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    PerformExitLobby(lobbyCode, player.Username, false);
                }
            }
        }

        public void StartMatch(string lobbyCode)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                LobbyInformation lobbyInformation = lobbies[lobbyCode].Item1;
                List<LobbyPlayer> players = lobbies[lobbyCode].Item2;

                matches.Add(lobbyCode, new Match.Match(lobbyInformation, players));

                foreach (var player in players.ToList())
                {
                    try
                    {
                        player.CallbackChannel.NotifyStartOfMatch();
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        PerformExitLobby(lobbyCode, player.Username, false);
                        DeletePlayerFromMatch(lobbyCode, player.Username);
                    }
                }

                lobbies.Remove(lobbyCode);
            }
        }

        public void ExitLobby(string lobbyCode, string username)
        {
            PerformExitLobby(lobbyCode, username, false);
        }

        public void ExpulsePlayerFromLobby(string lobbyCode, string username)
        {
            PerformExitLobby(lobbyCode, username, true);
        }

        private void DeletePlayerFromMatch(string lobbyCode, string username)
        {
            Match.Match match = matches[lobbyCode];
            LobbyPlayer player = match.GetLobbyPlayerByUsername(username);

            if (player != null)
            {
                match.DeletePlayerFromMatch(player);
                matches[lobbyCode] = match;
            }
        }

        private void PerformExitLobby(String lobbyCode, String username, bool isExpulsed)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                List<LobbyPlayer> players = lobbies[lobbyCode].Item2;
                LobbyPlayer playerToEliminate = null;

                int hostIndex = 0;
                int eliminatedPlayerIndex = hostIndex;


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

                if (isExpulsed)
                {
                    ManagePlayerExpulsed(playerToEliminate);
                }

                players.Remove(playerToEliminate);
                lobbies[lobbyCode] = (lobbies[lobbyCode].Item1, players);

                NotifyPlayerLeftLobby(players, username, eliminatedPlayerIndex, lobbyCode, isExpulsed);

                if (eliminatedPlayerIndex == hostIndex)
                {
                    lobbies.Remove(lobbyCode);
                }
            }
        }

        private void ManagePlayerExpulsed(LobbyPlayer playerToEliminate)
        {
            try
            {
                playerToEliminate.CallbackChannel.NotifyExpulsedFromLobby();
            }
            catch (CommunicationException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
            }
        }

        private void NotifyPlayerLeftLobby(List<LobbyPlayer> players, string username, int eliminatedPlayerIndex, string lobbyCode, bool isExpulsed)
        {
            int hostIndex = 0;

            foreach (var callbackChannel in players.Select(p => p.CallbackChannel).ToList())
            {
                try
                {
                    if (eliminatedPlayerIndex != hostIndex)
                    {
                        callbackChannel.NotifyPlayerLeftLobby(username);
                    }
                    else
                    {
                        callbackChannel.NotifyHostPlayerLeftLobby();
                    }
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    PerformExitLobby(lobbyCode, username, isExpulsed);
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

            return lobbies.ContainsKey(lobbyCode) ? GenerateLobbyCode() : lobbyCode;
        }
    }
}

