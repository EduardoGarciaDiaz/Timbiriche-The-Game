using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimbiricheViews.Components;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IBanManagerCallback
    {
        private void RegisterToBansNotifications(string lobbyCode)
        {
            InstanceContext context = new InstanceContext(this);
            Server.BanManagerClient banManagerClient = new Server.BanManagerClient(context);

            try
            {
                banManagerClient.RegisterToBansNotifications(lobbyCode, _playerLoggedIn.Username);
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

        private void LbSecondPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (gridOptionsSecondPlayer.Visibility == Visibility.Collapsed)
            {
                string secondPlayerUsername = (string)lbSecondPlayerUsername.Content;
                XAMLOptionsPlayerComponent optionsPlayerComponent = ConfigureOptionsPlayerComponent(secondPlayerUsername);

                gridOptionsSecondPlayer.Children.Add(optionsPlayerComponent);
                gridOptionsSecondPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                gridOptionsSecondPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }
        }

        private void LbThirdPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (gridOptionsThirdPlayer.Visibility == Visibility.Collapsed)
            {
                gridOptionsThirdPlayer.Children.Clear();
                string thirdPlayerUsername = (string)lbThirdPlayerUsername.Content;
                XAMLOptionsPlayerComponent optionsPlayerComponent = ConfigureOptionsPlayerComponent(thirdPlayerUsername);

                gridOptionsThirdPlayer.Children.Add(optionsPlayerComponent);
                gridOptionsThirdPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                gridOptionsThirdPlayer.Children.Clear();
                gridOptionsThirdPlayer.Visibility = Visibility.Collapsed;
            }
        }

        private void LbFourthPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (gridOptionsFourthPlayer.Visibility == Visibility.Collapsed)
            {
                gridOptionsFourthPlayer.Children.Clear();
                string fourthPlayerUsername = (string)lbFourthPlayerUsername.Content;
                XAMLOptionsPlayerComponent optionsPlayerComponent = ConfigureOptionsPlayerComponent(fourthPlayerUsername);

                gridOptionsFourthPlayer.Children.Add(optionsPlayerComponent);
                gridOptionsFourthPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                gridOptionsFourthPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }
        }

        private XAMLOptionsPlayerComponent ConfigureOptionsPlayerComponent(string username)
        {
            bool isHost = true;
            XAMLOptionsPlayerComponent optionsPlayerComponent;
            optionsPlayerComponent = CreateOptionsPlayerComponent(isHost, username);

            return optionsPlayerComponent;
        }

        private XAMLOptionsPlayerComponent CreateOptionsPlayerComponent(bool isHost, string username)
        {
            XAMLOptionsPlayerComponent optionsPlayerComponent = new XAMLOptionsPlayerComponent(isHost, username);
            optionsPlayerComponent.ButtonClicked += BtnOptionPlayer_Click;

            return optionsPlayerComponent;
        }

        private void BtnOptionPlayer_Click(object sender, ButtonClickEventArgs e)
        {
            const string BTN_REPORT = "Report";
            const string BTN_EXPULSE = "Expulse";

            if (e.ButtonName.Equals(BTN_REPORT))
            {
                ReportPlayer(e.Username);
            }
            if (e.ButtonName.Equals(BTN_EXPULSE))
            {
                ExpulsePlayer(e.Username);
            }
        }

        private void ReportPlayer(string username)
        {
            InstanceContext context = new InstanceContext(this);
            BanManagerClient banManagerClient = new BanManagerClient(context);

            int idPlayerReporter = PlayerSingleton.Player.IdPlayer;
            int idPlayerReported = GetIdPlayer(username);

            if (idPlayerReported > 0)
            {
                try
                {
                    string reporterUsername = PlayerSingleton.Player.Username;
                    banManagerClient.ReportPlayer(_lobbyCode, idPlayerReported, idPlayerReporter, reporterUsername);
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
            }
            else
            {
                ShowCantExpulsePlayerMessage();
            }
        }

        private int GetIdPlayer(string username)
        {
            UserManagerClient userManagerClient = new UserManagerClient();
            int idPlayer = -1;

            try
            {
                idPlayer = userManagerClient.GetIdPlayerByUsername(username);
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

            return idPlayer;
        }

        private void ShowCantExpulsePlayerMessage()
        {
            string title = Properties.Resources.lbCantExpulsePlayerTitle;
            string description = Properties.Resources.tbkCantExpulsePlayerDescription;

            EmergentWindows.CreateEmergentWindowNoModal(title, description);
        }

        private void ExpulsePlayer(string username)
        {
            _ = ExpulsePlayerFromLobbyAsync(username);
        }

        public void NotifyReportCompleted()
        {
            EmergentWindows.CreateSuccesfulReportMessageWindow();
        }

        public void NotifyPlayerAlreadyReported()
        {
            EmergentWindows.CreateReportedPlayerMessageWindow();
        }

        public void NotifyPlayerBanned(int idPlayerBanned)
        {
            ExitBanned(idPlayerBanned);
            EmergentWindows.CreateBannedPlayerMessageWindow();
        }

        private void ExitBanned(int idPlayerBanned)
        {
            ExitCurrentLobby(PlayerSingleton.Player.Username);
            ReestablishSelectedColor();
            NavigationService.Navigate(new XAMLBan(idPlayerBanned));
        }
    }
}
