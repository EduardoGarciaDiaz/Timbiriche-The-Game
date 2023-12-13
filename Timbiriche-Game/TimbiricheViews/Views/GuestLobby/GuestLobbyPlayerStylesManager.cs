using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLGuestLobby : Page, IPlayerStylesManagerCallback
    {
        private const int ID_DEFAULT_STYLE = 1;
        private Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private string _lobbyCode;


        private void Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataPlayer();
            PrepareNotificationOfStyleUpdated();
            ConfigureGuestPlayer();
        }

        private void ConfigureGuestPlayer()
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = _playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = ID_DEFAULT_STYLE;

            JoinToLobby(lobbyPlayer);
        }

        private void JoinToLobby(LobbyPlayer lobbyPlayer)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClient = new LobbyManagerClient(context);

            try
            {
                lobbyManagerClient.JoinLobby(_lobbyCode, lobbyPlayer);
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

        private void PrepareNotificationOfStyleUpdated()
        {
            bool isLoaded = true;

            LobbyPlayer lobbyPlayer = CreateLobbyPlayer();
            InformUpdateStyleForPlayers(lobbyPlayer, isLoaded);
        }

        private void InformUpdateStyleForPlayers(LobbyPlayer lobbyPlayer, bool isLoaded)
        {
            InstanceContext context = new InstanceContext(this);
            Server.PlayerStylesManagerClient playerStylesManagerClient = new Server.PlayerStylesManagerClient(context);

            try
            {
                if (!isLoaded)
                {
                    playerStylesManagerClient.AddStyleCallbackToLobbiesList(_lobbyCode, lobbyPlayer);
                }
                else if (_lobbyCode != null)
                {
                    playerStylesManagerClient.ChooseStyle(_lobbyCode, lobbyPlayer);
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
            catch (FaultException<TimbiricheServerExceptions>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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

        public void NotifyStyleSelected(LobbyPlayer lobbyPlayer)
        {
            string username = lobbyPlayer.Username;
            int idStyle = lobbyPlayer.IdStylePath;

            UpdateStyleOfPlayers(username, idStyle);
        }

        private void UpdateStyleOfPlayers(string username, int idStyle)
        {
            if (lbSecondPlayerUsername.Content.Equals(username))
            {
                LoadFaceBox(lbSecondPlayerFaceBox, idStyle, username);
            }
            else if (lbThirdPlayerUsername.Content.Equals(username))
            {
                LoadFaceBox(lbThirdPlayerUsername, idStyle, username);
            }
            else if (lbFourthPlayerUsername.Content.Equals(username))
            {
                LoadFaceBox(lbFourthPlayerUsername, idStyle, username);
            }
        }

        private Image CreateImageByPath(int idStyle)
        {
            string playerStylePath = GetStylePathByIdStyle(idStyle);
            string absolutePath = Utilities.BuildAbsolutePath(playerStylePath);

            Image styleImage = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
            styleImage.Source = bitmapImage;

            return styleImage;
        }

        private string GetStylePathByIdStyle(int idStyle)
        {
            string stylePath = "";
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            try
            {
                stylePath = playerCustomizationManagerClient.GetStylePath(idStyle);
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

            return stylePath;
        }

        private void LoadDataPlayer()
        {
            lbUsername.Content = _playerLoggedIn.Username;
            LoadFaceBox(lbUserFaceBox, _playerLoggedIn.IdStyleSelected, _playerLoggedIn.Username);
        }

        private void LoadFaceBox(Label lbFaceBox, int idStyle, string username)
        {
            const int indexFirstLetter = 0;

            if (idStyle == ID_DEFAULT_STYLE)
            {
                lbFaceBox.Content = username[indexFirstLetter].ToString();
            }
            else
            {
                Image styleImage = CreateImageByPath(idStyle);
                lbFaceBox.Content = styleImage;
            }
        }

        public void BtnCloseWindow_Click()
        {
            PlayerSingleton.Player = null;

            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);

            try
            {
                client.UnregisterUserToOnlineUsers(_playerLoggedIn.Username);
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
    }
}
