using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Player;

namespace TimbiricheViews.Views
{
    public partial class XAMLVictory : Page
    {
        KeyValuePair<string, int>[] _scoreboard;
        string _playerUsername;
        int _coinsEarned;

        public XAMLVictory(KeyValuePair<string, int>[] scoreboard, int coinsEarned)
        {
            InitializeComponent();
            _scoreboard = scoreboard;
            _coinsEarned = coinsEarned;
            _playerUsername = PlayerSingleton.Player.Username;
            InitializeVictoryPage();
        }

        private void InitializeVictoryPage()
        {
            int numPlayers = _scoreboard.Count();

            tbxFirstPlaceUsername.Text = _scoreboard[0].Key;
            tbxFirstPlacePoints.Text = _scoreboard[0].Value.ToString();

            tbxSecondPlaceUsername.Text = _scoreboard[1].Key;
            tbxSecondPlacePoints.Text = _scoreboard[1].Value.ToString();


            if (numPlayers > 2)
            {
                gridThirdPlace.Visibility = Visibility.Visible;
                tbxThirdPlaceUsername.Text = _scoreboard[2].Key;
                tbxThirdPlacePoints.Text = _scoreboard[2].Value.ToString();
            }

            if (numPlayers > 3)
            {
                gridFourthPlace.Visibility = Visibility.Visible;
                tbxFourthPlaceUsername.Text = _scoreboard[3].Key;
                tbxFourthPlacePoints.Text = _scoreboard[3].Value.ToString();
            }

            lbEarnedCoins.Content = _coinsEarned.ToString();

            if (IsPlayerAWinner())
            {
                lbYouWon.Visibility = Visibility.Visible;
                lbYouLost.Visibility = Visibility.Collapsed;
            }

            switch(GetPlayerPositionInScoreboard())
            {
                case 0:
                    lbFirstPlace.Visibility = Visibility.Visible;
                    break;
                case 1:
                    lbSecondPlace.Visibility = Visibility.Visible;
                    break;
                case 2:
                    lbThirdPlace.Visibility = Visibility.Visible;
                    break;
                case 4:
                    lbFourthPlace.Visibility = Visibility.Visible;
                    break;
            }

            lbUsername.Content = _playerUsername;
        }

        private bool IsPlayerAWinner()
        {
            string winnerUsername = _scoreboard[0].Key;

            return PlayerSingleton.Player.Username == winnerUsername;
        }

        private int GetPlayerPositionInScoreboard()
        {
            for(int i = 0; i < _scoreboard.Length; i++)
            {
                if(_playerUsername == _scoreboard[i].Key)
                {
                    return i;
                }
            }

            return -1;
        }

    }
}
