using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, ILobbyManagerCallback
    {
        private string _lobbyCode;
        private int _numberOfPlayersInLobby = 1;

        public void NotifyLobbyCreated(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControl.Visibility = Visibility.Visible;

            ShowSelectPlayerColorGrid();
            ValidateStartOfMatch();
        }

        private void ValidateStartOfMatch()
        {
            // TODO: VALIDATE ALL PLAYERS HAVE SELECTED COLOR
            const int INITIAL_NUMBER_OF_PLAYERS = 1;
            if (_numberOfPlayersInLobby > INITIAL_NUMBER_OF_PLAYERS)
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
                return;
            }
        }

        public void NotifyPlayerLeftLobby(string username)
        {
            string secondPlayerUsername = (string)lbSecondPlayerUsername.Content;
            string thirdPlayerUsername = (string)lbThirdPlayerUsername.Content;
            string fourthPlayerUsername = (string)lbFourthPlayerUsername.Content;

            if (username.Equals(secondPlayerUsername))
            {
                gridSecondPlayer.Visibility = Visibility.Collapsed;
                gridOptionsSecondPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(thirdPlayerUsername))
            {
                gridThirdPlayer.Visibility = Visibility.Collapsed;
                gridOptionsThirdPlayer.Children.Clear();
                gridOptionsThirdPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(fourthPlayerUsername))
            {
                gridFourthPlayer.Visibility = Visibility.Collapsed;
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

            _lobbyCode = lobbyCode;
            int numPlayersInLobby = lobbyPlayers.Length;
            const int SECOND_PLAYER_ID = 0;
            const int THIRD_PLAYER_ID = 1;
            const int FOURTH_PLAYER_ID = 2;

            if (numPlayersInLobby > SECOND_PLAYER_ID)
            {
                lbSecondPlayerUsername.Content = lobbyPlayers[SECOND_PLAYER_ID].Username;
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayers[SECOND_PLAYER_ID].IdStylePath, lobbyPlayers[SECOND_PLAYER_ID].Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > THIRD_PLAYER_ID)
            {
                lbThirdPlayerUsername.Content = lobbyPlayers[THIRD_PLAYER_ID].Username;
                LoadFaceBox(lbThirdPlayerFaceBox, lobbyPlayers[THIRD_PLAYER_ID].IdStylePath, lobbyPlayers[THIRD_PLAYER_ID].Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > FOURTH_PLAYER_ID)
            {
                lbFourthPlayerUsername.Content = lobbyPlayers[FOURTH_PLAYER_ID].Username;
                LoadFaceBox(lbFourthPlayerFaceBox, lobbyPlayers[FOURTH_PLAYER_ID].IdStylePath, lobbyPlayers[FOURTH_PLAYER_ID].Username);
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
            NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
        }

        private void JoinLobbyByLobbyCode(String lobbyCode)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = _playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = _playerLoggedIn.IdStyleSelected;

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);

            try
            {
                client.JoinLobby(lobbyCode, lobbyPlayer);
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
            catch (FaultException ex)
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

        public void NotifyExpulsedFromLobby()
        {
            string title = Properties.Resources.lbExpulsedTilte; ;
            string message = Properties.Resources.tbkExpulsedDescription; ;
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
            catch (FaultException ex)
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

            (string, string) playerCustomization = GetPlayerCustomization();
            NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
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
            ExitCurrentLobby(PlayerSingleton.Player.Username);
            ReestablishSelectedColor();

            NavigationService.Navigate(new XAMLLobby());
        }

        private void ExitCurrentLobby(string username)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClient = new LobbyManagerClient(context);
            try
            {
                lobbyManagerClient.ExitLobby(_lobbyCode, username);
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
            catch (FaultException ex)
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

        private void ReestablishSelectedColor()
        {
            int defaultColor = 0;

            InstanceContext context = new InstanceContext(this);
            PlayerColorsManagerClient playerColorsManagerClient = new PlayerColorsManagerClient(context);

            try
            {
                playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, CreateLobbyPlayer());
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
            catch (FaultException ex)
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

            PlayerSingleton.Player.IdColorSelected = defaultColor;
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
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException ex)
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

        private (string, string) GetPlayerCustomization()
        {
            string playerHexadecimalColor = null;
            string playerStylePath = null;
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            try
            {
                playerHexadecimalColor = GetHexadecimalColor(_playerLoggedIn.IdColorSelected);
                playerStylePath = playerCustomizationManagerClient.GetStylePath(_playerLoggedIn.IdStyleSelected);
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
            catch (FaultException<TimbiricheServerException> ex)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException ex)
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

            return (playerHexadecimalColor, playerStylePath);
        }
    }
}
