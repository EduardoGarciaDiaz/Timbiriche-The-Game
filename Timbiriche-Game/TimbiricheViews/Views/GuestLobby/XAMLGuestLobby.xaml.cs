using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TimbiricheViews.Components;
using TimbiricheViews.Components.Lobby;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLGuestLobby : Page, ILobbyManagerCallback
    {
        public XAMLGuestLobby()
        {
            InitializeComponent();
            LoadDataPlayer();
            this.Loaded += Lobby_Loaded;
        }

        public XAMLGuestLobby(string lobbyCode)
        {
            InitializeComponent();
            _lobbyCode = lobbyCode;
            LoadDataPlayer();
            this.Loaded += Lobby_Loaded;
        }

        public void NotifyLobbyCreated(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            ShowSelectPlayerColorGrid();
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
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

        public void NotifyPlayerLeftLobby(String username)
        {
            String secondPlayerUsername = (String)lbSecondPlayerUsername.Content;
            String thirdPlayerUsername = (String)lbThirdPlayerUsername.Content;
            String fourthPlayerUsername = (String)lbFourthPlayerUsername.Content;

            if (username.Equals(secondPlayerUsername))
            {
                gridSecondPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(thirdPlayerUsername))
            {
                gridThirdPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(fourthPlayerUsername))
            {
                gridFourthPlayer.Visibility = Visibility.Collapsed;
            }
        }

        public void NotifyHostPlayerLeftLobby()
        {
            Utils.EmergentWindows.CreateEmergentWindow(Properties.Resources.lbHostLeftLobbyTitle ,
                Properties.Resources.tbkHostLeftLobbyDescription);
            NavigationService.Navigate(new XAMLLogin());
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
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
            ShowSelectPlayerColorGrid();
        }

        public void NotifyLobbyIsFull()
        {
            EmergentWindows.CreateLobbyIsFullMessageWindow();

            NavigationService.Navigate(new XAMLLogin());
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

        private void BtnInviteToLobby_Click(object sender, RoutedEventArgs e)
        {
            EmergentWindows.CreateLobbyInvitationWindow(_lobbyCode);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);

            try
            {
                client.ExitLobby(_lobbyCode, PlayerSingleton.Player.Username);
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

            NavigationService.Navigate(new XAMLLogin());
        }

        private (string, string) GetPlayerCustomization()
        {
            string playerHexadecimalColor = "";
            string playerStylePath = "";

            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            try
            {
                playerHexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(_playerLoggedIn.IdColorSelected);
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
            }
            catch (FaultException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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

        public void NotifyExpulsedFromLobby()
        {
            string title = Properties.Resources.lbExpulsedTilte;
            string message = Properties.Resources.tbkExpulsedDescription;
            EmergentWindows.CreateEmergentWindowNoModal(title, message);

            NavigationService.Navigate(new XAMLLogin());
        }
    }

    
}