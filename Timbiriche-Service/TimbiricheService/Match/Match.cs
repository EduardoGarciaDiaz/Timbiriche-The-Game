using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TimbiricheService.Match
{
    public class Match
    {
        private LobbyInformation _lobbyInformation;
        private List<LobbyPlayer> _players;
        private Queue<LobbyPlayer> _turns;
        private Dictionary<LobbyPlayer, int> _scoreboard;
        private Dictionary<string, bool> _connectedPlayers;
        private int _numberOfPlayers;

        public LobbyInformation LobbyInformation { get { return _lobbyInformation; } set { _lobbyInformation = value; } }
        public List<LobbyPlayer> Players { get { return _players; } set { _players = value; } }

        public Match(LobbyInformation lobbyInformation, List<LobbyPlayer> players)
        {
            _lobbyInformation = lobbyInformation;
            _players = players;
            InitializeRandomTurns();
            InitializeScoreboard();
            InitializePlayersConnected();
            InitializeNumberOfPlayers();
        }

        public LobbyPlayer GetTurnPlayer()
        {
            return _turns.Peek();
        }

        public List<KeyValuePair<LobbyPlayer, int>> GetScoreboard()
        {
            Dictionary<LobbyPlayer, int> scoreboard = new Dictionary<LobbyPlayer, int>();

            foreach(var entry in _scoreboard)
            {
                scoreboard.Add(entry.Key, entry.Value);
            }

            List<KeyValuePair<LobbyPlayer, int>> sortedScoreboard = scoreboard.ToList();
            sortedScoreboard = sortedScoreboard.OrderByDescending(points => points.Value).ToList();

            return sortedScoreboard;
        }

        public void NextTurn()
        {
            LobbyPlayer player = _turns.Dequeue();
            _turns.Enqueue(player);
        }

        public void ScorePointsToPlayer(LobbyPlayer player, int points)
        {
            if(_scoreboard.ContainsKey(player))
            {
                _scoreboard[player] += points;
            }
        }

        private void InitializeNumberOfPlayers()
        {
            _numberOfPlayers = _players.Count;
        }

        private void InitializeRandomTurns()
        {
            List<LobbyPlayer> RandomTurns = new List<LobbyPlayer>(_players);
            Random random = new Random();
            int size = RandomTurns.Count;
            
            while(size > 1)
            {
                size--;
                int randomPosition = random.Next(size + 1);
                LobbyPlayer temporalLobbyPlayer = RandomTurns[randomPosition];
                RandomTurns[randomPosition] = RandomTurns[size];
                RandomTurns[size] = temporalLobbyPlayer;
            }

            _turns = new Queue<LobbyPlayer>(RandomTurns);
        }

        private void InitializeScoreboard()
        {
            int initialPoints = 0;
            _scoreboard = new Dictionary<LobbyPlayer, int>();

            foreach(LobbyPlayer player in _players)
            {
                _scoreboard.Add(player, initialPoints);
            }
        }

        private void InitializePlayersConnected()
        {
            _connectedPlayers = new Dictionary<string, bool>();

            foreach (LobbyPlayer player in _players)
            {
                _connectedPlayers.Add(player.Username, false);
            }
        }

        public bool AreAllPlayersConnected()
        {
            bool areAllPlayersConnected = true;
            
            foreach(var entry in _connectedPlayers)
            {
                if(entry.Value == false)
                {
                    return false;
                }
            }

            return areAllPlayersConnected;
        }

        public void SetConnectedUser(string username)
        {
            if (_connectedPlayers.ContainsKey(username))
            {
                _connectedPlayers[username] = true;
            }
        }

        public void DeletePlayerFromMatch(LobbyPlayer player)
        {
            _numberOfPlayers--;
            _players.Remove(player);
            _scoreboard.Remove(player);

            DeletePlayerFromTurns(player);
        }

        private void DeletePlayerFromTurns(LobbyPlayer player)
        {
            Queue<LobbyPlayer> newTurns = new Queue<LobbyPlayer>();

            while(_turns.Count > 0)
            {
                LobbyPlayer actualPlayer = _turns.Dequeue();

                if(actualPlayer != player)
                {
                    newTurns.Enqueue(actualPlayer);
                }
            }

            _turns = newTurns;
        }

        public int GetNumberOfPlayerInMatch()
        {
            return _numberOfPlayers;
        }

        public void DisconnectPlayerFromMatch()
        {
            _numberOfPlayers--;
        }

        public LobbyPlayer GetLobbyPlayerByUsername(String username)
        {
            LobbyPlayer lobbyPlayer = null;

            foreach(LobbyPlayer player in _players)
            {
                if(player.Username == username)
                {
                    lobbyPlayer = player;
                }
            }

            return lobbyPlayer; 
        }
    }
}
