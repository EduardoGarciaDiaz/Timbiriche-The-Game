using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IPlayerStylesManagerCallback
    {
        private void RestartSelectedColor(bool isRematch)
        {
            PlayerSingleton.Player.IdColorSelected = DEFAULT_SELECTED_COLOR;

            if (isRematch)
            {
                InstanceContext context = new InstanceContext(this);
                PlayerColorsManagerClient client = new PlayerColorsManagerClient(context);

                client.UnsubscribeColorToColorsSelected(_lobbyCode, CreateLobbyPlayer());
            }
        }

        private void Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataPlayer();
                PrepareNotificationOfStyleUpdated();
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
            PlayerStylesManagerClient playerStylesManagerClient = new PlayerStylesManagerClient(context);

            if (!isLoaded)
            {
                playerStylesManagerClient.AddStyleCallbackToLobbiesList(_lobbyCode, lobbyPlayer);
            }
            else if (_lobbyCode != null)
            {
                playerStylesManagerClient.ChooseStyle(_lobbyCode, lobbyPlayer);
            }
        }

        public void NotifyStyleSelected(LobbyPlayer lobbyPlayer)
        {
            string username = lobbyPlayer.Username;
            int idStyle = lobbyPlayer.IdStylePath;

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

        private void LoadDataPlayer()
        {
            lbUsername.Content = _playerLoggedIn.Username;
            lbCoins.Content = _playerLoggedIn.Coins;

            LoadFaceBox(lbUserFaceBox, _playerLoggedIn.IdStyleSelected, _playerLoggedIn.Username);
        }

        private void LoadFaceBox(Label lbFaceBox, int idStyle, string username)
        {
            int idDefaultStyle = 1;
            int indexFirstLetter = 0;

            if (idStyle == idDefaultStyle)
            {
                lbFaceBox.Content = username[indexFirstLetter].ToString();
            }
            else
            {
                try
                {
                    string stylePath = GetPathByIdStyle(idStyle);
                    Image styleImage = Utilities.CreateImageByPath(stylePath);
                    lbFaceBox.Content = styleImage;
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
                    NavigationService?.Navigate(new XAMLLogin());
                }
                catch (FaultException)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                    NavigationService?.Navigate(new XAMLLogin());
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

        private string GetPathByIdStyle(int idStyle)
        {
            PlayerCustomizationManagerClient playerCustomizationManagerClient = new PlayerCustomizationManagerClient();
            string playerStylePath = null;

            playerStylePath = playerCustomizationManagerClient.GetStylePath(idStyle);
            
            return playerStylePath;
        }
    }
}
