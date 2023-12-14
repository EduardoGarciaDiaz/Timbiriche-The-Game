using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using TimbiricheViews.Components.Match;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLGameBoard : Page, IMatchManagerCallback
    {
        private Match.Timer _matchTimer;
        private Match.Timer _turnTimer;
        private DispatcherTimer _dispatchTimer;

        public void NotifyMovement(Movement movement)
        {
            Button btnLine = FindButtonByName(movement.TypeLine + "_" + movement.Row + "_" + movement.Column);
            string stylePlayer = ChooseCorrectStyle(movement.StylePath, movement.Username);

            SetMovement(btnLine, movement.HexadecimalColor, stylePlayer, movement.Row, movement.Column, movement.TypeLine);
        }

        public void NotifyFirstTurn(float matchDurationInMinutes, float turnDurationInMinutes, string username)
        {
            _matchTimer = new Match.Timer((int)(matchDurationInMinutes * 60));
            _turnTimer = new Match.Timer((int)(turnDurationInMinutes * 60));

            _dispatchTimer = new DispatcherTimer();
            TimeSpan.FromSeconds(1);

            _dispatchTimer.Tick += UpdateTimeLabels;
            _dispatchTimer.Start();

            _matchTimer.CountDownFinished += OnCountDownMatchFinished;
            _turnTimer.CountDownFinished += OnCountDownTurnFinished;

            _matchTimer.Start();
            _turnTimer.Start();

            UpdateTurn(username);
        }

        public void NotifyNewTurn(string username)
        {
            UpdateTurn(username);
        }

        public void NotifyNewScoreboard(KeyValuePair<Server.LobbyPlayer, int>[] scoreboard)
        {
            string animationNewScoreboard = "fadeAnimation";
            int indexFirstPlayer = 0;
            int indexSecondPlayer = 1;
            int indexThirdPlayer = 2;
            int indexFourthPlayer = 3;

            if (stackPanelScoreboard.Visibility == Visibility.Collapsed)
            {
                stackPanelScoreboard.Visibility = Visibility.Visible;
            }

            Storyboard animationFadeIn = (Storyboard)FindResource(animationNewScoreboard);
            animationFadeIn.Begin();

            int numPlayers = scoreboard.Count();

            SolidColorBrush brushFirstPlace = Utilities.CreateColorFromHexadecimal(scoreboard[indexFirstPlayer].Key.HexadecimalColor);
            tbxFirstPlaceUsername.Text = scoreboard[indexFirstPlayer].Key.Username;
            tbxFirstPlacePoints.Text = scoreboard[indexFirstPlayer].Value.ToString();
            borderFirstPlace.Background = brushFirstPlace;

            SolidColorBrush brushSecondPlace = Utilities.CreateColorFromHexadecimal(scoreboard[indexSecondPlayer].Key.HexadecimalColor);
            tbxSecondPlaceUsername.Text = scoreboard[indexSecondPlayer].Key.Username;
            tbxSecondPlacePoints.Text = scoreboard[indexSecondPlayer].Value.ToString();
            borderSecondPlace.Background = brushSecondPlace;

            if (numPlayers > indexThirdPlayer)
            {
                SolidColorBrush brushThirdPlace = Utilities.CreateColorFromHexadecimal(scoreboard[indexThirdPlayer].Key.HexadecimalColor);
                gridThirdPlace.Visibility = Visibility.Visible;
                tbxThirdPlaceUsername.Text = scoreboard[indexThirdPlayer].Key.Username;
                tbxThirdPlacePoints.Text = scoreboard[indexThirdPlayer].Value.ToString();
                borderThirdPlace.Background = brushThirdPlace;
            }

            if (numPlayers > indexFourthPlayer)
            {
                SolidColorBrush brushFourthPlace = Utilities.CreateColorFromHexadecimal(scoreboard[indexFourthPlayer].Key.HexadecimalColor);
                gridFourthPlace.Visibility = Visibility.Visible;
                tbxFourthPlaceUsername.Text = scoreboard[indexFourthPlayer].Key.Username;
                tbxFourthPlacePoints.Text = scoreboard[indexFourthPlayer].Value.ToString();
                borderFourthPlace.Background = brushFourthPlace;
            }
        }

        public void NotifyEndOfTheMatch(KeyValuePair<Server.LobbyPlayer, int>[] scoreboard, int coinsEarned)
        {
            _turnTimer.Stop();
            _matchTimer.Stop();
            _dispatchTimer.Stop();

            XAMLMainWindow parentWindow = Window.GetWindow(this) as XAMLMainWindow;

            if (parentWindow != null)
            {
                parentWindow.frameNavigation.NavigationService.Navigate(new XAMLVictory(_lobbyCode, scoreboard, coinsEarned));
            }
        }

        public void NotifyNewMessage(string senderUsername, string message, int idSenderPlayer)
        {
            bool isMessageReceived = true;

            XAMLMessageItemComponent messageComponent = new XAMLMessageItemComponent(senderUsername, message,
                isMessageReceived, idSenderPlayer, _lobbyCode);
            messageComponent.HorizontalAlignment = HorizontalAlignment.Left;

            stackPanelMessages.Children.Add(messageComponent);
        }

        public void NotifyPlayerLeftMatch()
        {
            gridThirdPlace.Visibility = Visibility.Collapsed;
            gridFourthPlace.Visibility = Visibility.Collapsed;
        }

        public void NotifyOnlyPlayerInMatch()
        {
            EmergentWindows.CreateEmergentWindow(Properties.Resources.lbUniquePlayerTitle,
                Properties.Resources.tbkUniquePlayerDescription);

            LeftMatch();
        }

        private Button FindButtonByName(string name)
        {
            return (Button)LogicalTreeHelper.FindLogicalNode(this, name);
        }

        private void UpdateTimeLabels(object sender, EventArgs e)
        {
            lbMatchTime.Content = _matchTimer.GetTime();
            lbTurnTime.Content = _turnTimer.GetTime();
        }

        private void UpdateTurn(string username)
        {
            lbTurnOfUsername.Content = username;
            _itsMyTurn = (_username == username);

            _turnTimer.Reset();
        }

        private void OnCountDownTurnFinished(object sender, EventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.MatchManagerClient client = new Server.MatchManagerClient(context);

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    client.EndTurnWithoutMovement(_lobbyCode);
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
            });
        }

        private void OnCountDownMatchFinished(object sender, EventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            MatchManagerClient client = new MatchManagerClient(context);

            _itsMyTurn = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    client.EndMatch(_lobbyCode);
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
            });
        }

        private void LeftMatch()
        {
            _turnTimer.Stop();
            _matchTimer.Stop();
            _dispatchTimer.Stop();

            XAMLMainWindow parentWindow = Window.GetWindow(this) as XAMLMainWindow;

            if (parentWindow != null)
            {
                parentWindow.frameNavigation.NavigationService.Navigate(new XAMLLobby());
            }
        }

        private void SendMessageToLobby(string senderUsername, string message, int idSenderPlayer)
        {
            InstanceContext context = new InstanceContext(this);
            MatchManagerClient client = new MatchManagerClient(context);

            try
            {
                client.SendMessageToLobby(_lobbyCode, senderUsername, message, idSenderPlayer);
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

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            int emptySizeText = 0;
            string defaultMessageText = "";

            if (tbxMessage.Text != null && tbxMessage.Text.Length != emptySizeText)
            {
                string senderUsername = _username;
                string message = tbxMessage.Text;
                bool isMessageReceived = false;

                int idSenderPlayer = PlayerSingleton.Player.IdPlayer;

                XAMLMessageItemComponent messageComponent = new XAMLMessageItemComponent(senderUsername, message,
                    isMessageReceived, idSenderPlayer, _lobbyCode)
                {
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                stackPanelMessages.Children.Add(messageComponent);
                tbxMessage.Text = defaultMessageText;

                SendMessageToLobby(senderUsername, message, idSenderPlayer);
            }
        }

        private void BtnExitMatch_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.MatchManagerClient client = new Server.MatchManagerClient(context);

            try
            {
                client.LeftMatch(_lobbyCode, PlayerSingleton.Player.Username);
                LeftMatch();
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
            finally
            {
                _turnTimer.Stop();
                _matchTimer.Stop();
                _dispatchTimer.Stop();
            }

        }

        private void ImageExitMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            gridExitMenu.Visibility = Visibility.Visible;
        }

        private void GridExitMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            gridExitMenu.Visibility = Visibility.Collapsed;
        }
    }
}
