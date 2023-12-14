using System;
using System.ServiceModel;
using System.Windows;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLVictory : IRematchManagerCallback
    {
        public void NotifyRematch(string lobbyCode)
        {
            bool isHost = false;

            NavigationService.Navigate(new XAMLLobby(lobbyCode, isHost));
        }

        public void NotifyHostOfRematch(string lobbyCode)
        {
            bool isHost = true;

            NavigationService.Navigate(new XAMLLobby(lobbyCode, isHost));
        }

        private void SendNotRematch()
        {
            InstanceContext context = new InstanceContext(this);
            RematchManagerClient client = new RematchManagerClient(context);

            //try
            //{
                client.NotRematch(_lobbyCode);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    EmergentWindows.CreateConnectionFailedMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (TimeoutException ex)
            //{
            //    EmergentWindows.CreateTimeOutMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (FaultException)
            //{
            //    EmergentWindows.CreateServerErrorMessageWindow();
            //    NavigationService.Navigate(new XAMLLogin());
            //}
            //catch (CommunicationException ex)
            //{
            //    EmergentWindows.CreateServerErrorMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (Exception ex)
            //{
            //    EmergentWindows.CreateUnexpectedErrorMessageWindow();
            //    HandlerExceptions.HandleFatalException(ex, NavigationService);
            //}
        }

        private void BtnRematch_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            RematchManagerClient client = new RematchManagerClient(context);

            try
            {
                client.Rematch(_lobbyCode, _playerUsername);
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

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            ExitToLobby();
        }

        private void ExitToLobby()
        {
            int idGuestIdPlayer = 0;
            try
            {
                SendNotRematch();

                if (_playerLoggedIn.IdPlayer > idGuestIdPlayer)
                {
                    bool isPlayerBanned = VerifyPlayerIsNotBanned(_playerLoggedIn.IdPlayer);

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
    }
}
