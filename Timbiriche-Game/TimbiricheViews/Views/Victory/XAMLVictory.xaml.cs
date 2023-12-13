﻿using System;
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
        private string _lobbyCode;
        private string _playerUsername;
        private int _coinsEarned;
        private int _idPlayer;

        public XAMLVictory(string lobbyCode, KeyValuePair<Server.LobbyPlayer, int>[] scoreboard, int coinsEarned)
        {
            InitializeComponent();
            _scoreboard = scoreboard;
            _coinsEarned = coinsEarned;
            _playerUsername = _playerLoggedIn.Username;
            _lobbyCode = lobbyCode;
            _idPlayer = _playerLoggedIn.IdPlayer;

            InitializeVictoryPage();
        }

        private void InitializeVictoryPage()
        {
            int numPlayers = _scoreboard.Count();
            int indexFirstPlayer = 0;
            int indexSecondPlayer = 1;
            int indexThirdPlayer = 2;
            int indexFourthPlayer = 3;

            SolidColorBrush brushFirstPlace = Utilities.CreateColorFromHexadecimal(_scoreboard[indexFirstPlayer].Key.HexadecimalColor);
            tbxFirstPlaceUsername.Text = _scoreboard[indexFirstPlayer].Key.Username;
            tbxFirstPlacePoints.Text = _scoreboard[indexFirstPlayer].Value.ToString();
            borderFirstPlace.Background = brushFirstPlace;

            SolidColorBrush brushSecondPlace = Utilities.CreateColorFromHexadecimal(_scoreboard[indexSecondPlayer].Key.HexadecimalColor);
            tbxSecondPlaceUsername.Text = _scoreboard[indexSecondPlayer].Key.Username;
            tbxSecondPlacePoints.Text = _scoreboard[indexSecondPlayer].Value.ToString();
            borderSecondPlace.Background = brushSecondPlace;


            if (numPlayers > indexThirdPlayer)
            {
                SolidColorBrush brushThirdPlace = Utilities
                    .CreateColorFromHexadecimal(_scoreboard[indexThirdPlayer].Key.HexadecimalColor);
                gridThirdPlace.Visibility = Visibility.Visible;
                tbxThirdPlaceUsername.Text = _scoreboard[indexThirdPlayer].Key.Username;
                tbxThirdPlacePoints.Text = _scoreboard[indexThirdPlayer].Value.ToString();
                borderThirdPlace.Background = brushThirdPlace;
            }

            if (numPlayers > indexFourthPlayer)
            {
                SolidColorBrush brushFourthPlace = Utilities
                    .CreateColorFromHexadecimal(_scoreboard[indexFourthPlayer].Key.HexadecimalColor);
                gridFourthPlace.Visibility = Visibility.Visible;
                tbxFourthPlaceUsername.Text = _scoreboard[indexFourthPlayer].Key.Username;
                tbxFourthPlacePoints.Text = _scoreboard[indexFourthPlayer].Value.ToString();
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
            int idPlayerGuest = 0;

            if (_idPlayer > idPlayerGuest)
            {
                ScoreboardManagerClient scoreboardManagerClient = new ScoreboardManagerClient();

                try
                {
                    scoreboardManagerClient.UpdateWins(_idPlayer);
                }
                catch (EndpointNotFoundException ex)
                {
                    EmergentWindows.CreateConnectionFailedMessageWindow();
                    HandlerException.HandleErrorException(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    EmergentWindows.CreateTimeOutMessageWindow();
                    HandlerException.HandleErrorException(ex, NavigationService);
                }
                catch (FaultException<TimbiricheServerException>)
                {
                    EmergentWindows.CreateDataBaseErrorMessageWindow();
                    NavigationService.Navigate(new XAMLLogin());
                }
                catch (FaultException)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                    NavigationService.Navigate(new XAMLLogin());
                }
                catch (CommunicationException ex)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                    HandlerException.HandleErrorException(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    EmergentWindows.CreateUnexpectedErrorMessageWindow();
                    HandlerException.HandleFatalException(ex, NavigationService);
                }
            }
        }

        private int GetPlayerPositionInScoreboard()
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
            bool isPlayerBanned = false;
            BanVerifierManagerClient banVerifierManagerClient = new BanVerifierManagerClient();

            try
            {
                isPlayerBanned = banVerifierManagerClient.VerifyPlayerIsBanned(idPlayer);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

            return isPlayerBanned;
        }
    }
}