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

        public Match(LobbyInformation lobbyInformation, List<LobbyPlayer> players)
        {
            _lobbyInformation = lobbyInformation;
            _players = players;
            InitializeRandomTurns();
            InitializeScoreboard();
        }

        public LobbyInformation LobbyInformation { get { return _lobbyInformation; } set { _lobbyInformation = value; } }
        public List<LobbyPlayer> Players { get { return _players; } set { _players = value; } }

        public LobbyPlayer GetTurnPlayer()
        {
            return _turns.Peek();
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
            foreach(LobbyPlayer player in _players)
            {
                _scoreboard.Add(player, initialPoints);
            }
        }

    }
}
