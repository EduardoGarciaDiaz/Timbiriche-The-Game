using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService.Match
{
    public class Match
    {
        LobbyInformation _lobbyInformation;
        List<LobbyPlayer> _players;
        Queue<LobbyPlayer> _turns;
        Dictionary<LobbyPlayer, int> _scoreboard;
        Dictionary<string, bool> _connectedPlayers;

        public LobbyInformation LobbyInformation { get; set; }
        public List<LobbyPlayer> Players { get; set; }

        public Match(LobbyInformation lobbyInformation, List<LobbyPlayer> players)
        {
            _lobbyInformation = lobbyInformation;
            _players = players;
            InitializeRandomTurns();
            InitializeScoreboard();
            InitializePlayersConnected();
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
    }
}
