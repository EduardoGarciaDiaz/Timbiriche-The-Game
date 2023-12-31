﻿using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, ILobbyManagerCallback
    {
        private string _lobbyCode;
        private int _numberOfPlayersInLobby = 1;
        private bool _allHasColor = false;

        public void NotifyLobbyCreated(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControl.Visibility = Visibility.Visible;
            btnSignOff.Visibility = Visibility.Collapsed;
            btnShop.IsEnabled = false;

            ShowSelectPlayerColorGrid();
            ValidateStartOfMatch();
        }

        private void ValidateStartOfMatch()
        {
            int initialNumberOfPlayers = 1;

            if (_numberOfPlayersInLobby > initialNumberOfPlayers && _allHasColor)
            {
                btnStartMatch.IsEnabled = true;
            }
            else
            {
                btnStartMatch.IsEnabled = false;
            }
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
            _numberOfPlayersInLobby = ++numOfPlayersInLobby;
            _allHasColor = false;
            ValidateStartOfMatch();

            if (gridSecondPlayer.Visibility == Visibility.Collapsed)
            {
                lbSecondPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
                return;
            }

            if (gridThirdPlayer.Visibility == Visibility.Collapsed)
            {
                lbThirdPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbThirdPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
                return;
            }

            if (gridFourthPlayer.Visibility == Visibility.Collapsed)
            {
                lbFourthPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbFourthPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridFourthPlayer.Visibility = Visibility.Visible;
            }
        }

        public void NotifyPlayerLeftLobby(string username)
        {
            string secondPlayerUsername = (string)lbSecondPlayerUsername.Content;
            string thirdPlayerUsername = (string)lbThirdPlayerUsername.Content;
            string fourthPlayerUsername = (string)lbFourthPlayerUsername.Content;
            string defaultHexadecimalRectangle = "#FF6C6868";

            SolidColorBrush defaultRectangleColor = Utilities.CreateColorFromHexadecimal(defaultHexadecimalRectangle);

            if (username.Equals(secondPlayerUsername))
            {
                gridSecondPlayer.Visibility = Visibility.Collapsed;
                rectangleSecondPlayerColor.Fill = defaultRectangleColor;
                rectangleSecondPlayerUsernameColor.Fill = defaultRectangleColor;
                gridOptionsSecondPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(thirdPlayerUsername))
            {
                gridThirdPlayer.Visibility = Visibility.Collapsed;
                rectangleThirdPlayerColor.Fill = defaultRectangleColor;
                rectangleThirdPlayerUsernameColor.Fill = defaultRectangleColor;
                gridOptionsThirdPlayer.Children.Clear();
                gridOptionsThirdPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(fourthPlayerUsername))
            {
                gridFourthPlayer.Visibility = Visibility.Collapsed;
                rectangleFourthPlayerColor.Fill = defaultRectangleColor;
                rectangleFourthPlayerUsernameColor.Fill = defaultRectangleColor;
                gridOptionsFourthPlayer.Children.Clear();
                gridOptionsFourthPlayer.Visibility = Visibility.Collapsed;
            }

            _numberOfPlayersInLobby--;
            ValidateStartOfMatch();
        }

        public void NotifyHostPlayerLeftLobby()
        {
            EmergentWindows.CreateHostLeftLobbyMessageWindow();
            NavigationService.Navigate(new XAMLLobby());
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControlNotLeadPlayer.Visibility = Visibility.Visible;
            btnSignOff.Visibility = Visibility.Collapsed;
            btnShop.IsEnabled = false;

            _lobbyCode = lobbyCode;
            int numPlayersInLobby = lobbyPlayers.Length;
            const int secondPlayerId = 0;
            const int thirdPlayerId = 1;
            const int fourthPlayerId = 2;

            if (numPlayersInLobby > secondPlayerId)
            {
                lbSecondPlayerUsername.Content = lobbyPlayers[secondPlayerId].Username;
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayers[secondPlayerId].IdStylePath, lobbyPlayers[secondPlayerId].Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > thirdPlayerId)
            {
                lbThirdPlayerUsername.Content = lobbyPlayers[thirdPlayerId].Username;
                LoadFaceBox(lbThirdPlayerFaceBox, lobbyPlayers[thirdPlayerId].IdStylePath, lobbyPlayers[thirdPlayerId].Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > fourthPlayerId)
            {
                lbFourthPlayerUsername.Content = lobbyPlayers[fourthPlayerId].Username;
                LoadFaceBox(lbFourthPlayerFaceBox, lobbyPlayers[fourthPlayerId].IdStylePath, lobbyPlayers[fourthPlayerId].Username);
                gridFourthPlayer.Visibility = Visibility.Visible;
            }

            gridCodeDialog.Visibility = Visibility.Collapsed;

            ShowSelectPlayerColorGrid();
        }

        public void NotifyLobbyIsFull()
        {
            EmergentWindows.CreateLobbyIsFullMessageWindow();
        }

        public void NotifyLobbyDoesNotExist()
        {
            EmergentWindows.CreateLobbyNotFoundMessageWindow();
        }

        public void NotifyStartOfMatch()
        {
            (string, string) playerCustomization = GetPlayerCustomization();

            if (playerCustomization != (null, null))
            {
                NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
            }
        }

        private void JoinLobbyByLobbyCode(string lobbyCode)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer
            {
                Username = _playerLoggedIn.Username,
                IdStylePath = _playerLoggedIn.IdStyleSelected
            };

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);

            try
            {
                client.JoinLobby(lobbyCode, lobbyPlayer);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }
        }

        public void NotifyExpulsedFromLobby()
        {
            string title = Properties.Resources.lbExpulsedTilte;
            string message = Properties.Resources.tbkExpulsedDescription;
            EmergentWindows.CreateEmergentWindowNoModal(title, message);

            NavigationService.Navigate(new XAMLLobby());
        }

        private void BtnCreateMatch_Click(object sender, RoutedEventArgs e)
        {
            ConfigureMatch();
        }

        private void BtnJoinByCode_Click(object sender, RoutedEventArgs e)
        {
            gridCodeDialog.Visibility = Visibility.Visible;
        }

        private void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            string lobbyCode = tbxJoinByCode.Text.Trim();

            JoinLobbyByLobbyCode(lobbyCode);
        }

        private void BtnStartMatch_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);

            try
            {
                client.StartMatch(_lobbyCode);
                (string, string) playerCustomization = GetPlayerCustomization();

                if (playerCustomization != (null, null))
                {
                    NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
                }
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }
        }

        private void BtnInviteToLobby_Click(object sender, RoutedEventArgs e)
        {
            EmergentWindows.CreateLobbyInvitationWindow(_lobbyCode);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            ExitToLobby();
        }

        private void ExitToLobby()
        {
            try
            {
                ExitCurrentLobby(PlayerSingleton.Player.Username);
                ReestablishSelectedColor();

                NavigationService.Navigate(new XAMLLobby());
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }
        }

        private void ExitCurrentLobby(string username)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClient = new LobbyManagerClient(context);

            lobbyManagerClient.ExitLobby(_lobbyCode, username);
        }

        private void ReestablishSelectedColor()
        {
            InstanceContext context = new InstanceContext(this);
            PlayerColorsManagerClient playerColorsManagerClient = new PlayerColorsManagerClient(context);

            playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, CreateLobbyPlayer());
           
            PlayerSingleton.Player.IdColorSelected = DEFAULT_SELECTED_COLOR;
        }

        public async Task ExpulsePlayerFromLobbyAsync(string username)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClientExpulse = new LobbyManagerClient(context);

            try
            {
                await lobbyManagerClientExpulse.ExpulsePlayerFromLobbyAsync(_lobbyCode, username);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }
        }

        private (string, string) GetPlayerCustomization()
        {
            string playerHexadecimalColor = null;
            string playerStylePath = null;
            PlayerCustomizationManagerClient playerCustomizationManagerClient = new PlayerCustomizationManagerClient();

            try
            {
                playerHexadecimalColor = GetHexadecimalColor(_playerLoggedIn.IdColorSelected);
                playerStylePath = playerCustomizationManagerClient.GetStylePath(_playerLoggedIn.IdStyleSelected);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerExceptions>)
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
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }

            return (playerHexadecimalColor, playerStylePath);
        }
    }
}
