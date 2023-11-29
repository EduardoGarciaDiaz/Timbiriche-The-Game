using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLVictory : Page
    {
        private Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private KeyValuePair<Server.LobbyPlayer, int>[] _scoreboard;
        private string _playerUsername;
        private int _coinsEarned;
        private int _idPlayer;

        public XAMLVictory(KeyValuePair<Server.LobbyPlayer, int>[] scoreboard, int coinsEarned)
        {
            InitializeComponent();
            _scoreboard = scoreboard;
            _coinsEarned = coinsEarned;
            _playerUsername = _playerLoggedIn.Username;
            _idPlayer = _playerLoggedIn.IdPlayer;
            InitializeVictoryPage();
        }

        private void InitializeVictoryPage()
        {
            int numPlayers = _scoreboard.Count();

            SolidColorBrush brushFirstPlace = Utilities.CreateColorFromHexadecimal(_scoreboard[0].Key.HexadecimalColor);
            tbxFirstPlaceUsername.Text = _scoreboard[0].Key.Username;
            tbxFirstPlacePoints.Text = _scoreboard[0].Value.ToString();
            borderFirstPlace.Background = brushFirstPlace;

            SolidColorBrush brushSecondPlace = Utilities.CreateColorFromHexadecimal(_scoreboard[1].Key.HexadecimalColor);
            tbxSecondPlaceUsername.Text = _scoreboard[1].Key.Username;
            tbxSecondPlacePoints.Text = _scoreboard[1].Value.ToString();
            borderSecondPlace.Background = brushSecondPlace;


            if (numPlayers > 2)
            {
                SolidColorBrush brushThirdPlace = Utilities.CreateColorFromHexadecimal(_scoreboard[2].Key.HexadecimalColor);
                gridThirdPlace.Visibility = Visibility.Visible;
                tbxThirdPlaceUsername.Text = _scoreboard[2].Key.Username;
                tbxThirdPlacePoints.Text = _scoreboard[2].Value.ToString();
                borderThirdPlace.Background = brushThirdPlace;
            }

            if (numPlayers > 3)
            {
                SolidColorBrush brushFourthPlace = Utilities.CreateColorFromHexadecimal(_scoreboard[3].Key.HexadecimalColor);
                gridFourthPlace.Visibility = Visibility.Visible;
                tbxFourthPlaceUsername.Text = _scoreboard[3].Key.Username;
                tbxFourthPlacePoints.Text = _scoreboard[3].Value.ToString();
                borderFourthPlace.Background = brushFourthPlace;
            }

            lbEarnedCoins.Content = _coinsEarned.ToString();

            if (IsPlayerAWinner())
            {
                lbYouWon.Visibility = Visibility.Visible;
                lbYouLost.Visibility = Visibility.Collapsed;
                UpdateWinsNumber();
            }

            switch (GetPlayerPositionInScoreboard())
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
            string winnerUsername = _scoreboard[0].Key.Username;
            return _playerLoggedIn.Username == winnerUsername;
        }

        private void UpdateWinsNumber()
        {
            if (_idPlayer > 0)
            {
                Server.ScoreboardManagerClient scoreboardManagerClient = new Server.ScoreboardManagerClient();
                scoreboardManagerClient.UpdateWins(_idPlayer);
            }
        }

        private int GetPlayerPositionInScoreboard() //TODO: Check dictionary
        {
            for(int i = 0; i < _scoreboard.Length; i++)
            {
                if(_playerUsername == _scoreboard[i].Key.Username)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool VerifyPlayerIsNotBanned(int idPlayer)
        {
            Server.BanVerifierManagerClient banVerifierManagerClient = new Server.BanVerifierManagerClient();
            bool isPlayerBanned = banVerifierManagerClient.VerifyPlayerIsBanned(idPlayer);

            return isPlayerBanned;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Exit lobby
            bool isPlayerBanned = true;
            if (_idPlayer != 0)
            {
                isPlayerBanned = VerifyPlayerIsNotBanned(_playerLoggedIn.IdPlayer);
            }

            if (isPlayerBanned)
            {
                NavigationService.Navigate(new XAMLLogin());
            }
            else
            {
                PlayerSingleton.UpdatePlayerFromDataBase();
                NavigationService.Navigate(new XAMLLobby());
            }
        }

        
    }
}
