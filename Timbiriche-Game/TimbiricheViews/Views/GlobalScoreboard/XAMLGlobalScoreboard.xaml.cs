using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLGlobalScoreboard : Page, IGlobalScoreManagerCallback
    {
        private const string IDENTIFIER_CURRENT_PLAYER_HEX_COLOR = "#FF232B9B";
        private readonly Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private GlobalScore[] _globalScores;

        public XAMLGlobalScoreboard()
        {
            InitializeComponent();
            this.Loaded += GlobalScoreboard_Loaded;
        }

        private void GlobalScoreboard_Loaded(object sender, RoutedEventArgs e)
        {
            SuscribeToGlobalScoreboard();
        }

        private void SuscribeToGlobalScoreboard()
        {
            InstanceContext context = new InstanceContext(this);
            Server.GlobalScoreManagerClient globalScoreManagerClient = new Server.GlobalScoreManagerClient(context);

            try
            {
                globalScoreManagerClient.SubscribeToGlobalScoreRealTime(_playerLoggedIn.Username);
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

        private void UnsuscribeToGlobalScoreboard()
        {
            InstanceContext context = new InstanceContext(this);
            Server.GlobalScoreManagerClient globalScoreManagerClient = new Server.GlobalScoreManagerClient(context);

            try
            {
                globalScoreManagerClient.UnsubscribeToGlobalScoreRealTime(_playerLoggedIn.Username);
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

        private void GetAllScorePlayers()
        {
            Server.ScoreboardManagerClient scoreboardManagerClient = new Server.ScoreboardManagerClient();
            try
            {
                _globalScores = scoreboardManagerClient.GetGlobalScores();
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

        private void SetScores()
        {
            int position = 1;

            if (_globalScores != null)
            {
                foreach (GlobalScore score in _globalScores)
                {
                    Grid gridScorePlayer = CloneScorePlayerGrid(score, position);
                    stackPanelScoreboard.Children.Add(gridScorePlayer);

                    position++;
                }
            }
        }

        private Grid CloneScorePlayerGrid(GlobalScore scorePlayer, int position)
        {
            string idGrid = "grid";
            string idLbPosition = "lbPosition";
            string idLbUsernamePlayer = "lbUsernamePlayer";
            string idLbVictorries = "lbVictories";
            string positionSymbol = "#";
            string splitSymbol = "_";

            string idGlobalScore = scorePlayer.IdGlobalScore.ToString();
            string idPlayer = scorePlayer.IdPlayer.ToString();
            string idGridScorePlayer = idGlobalScore + splitSymbol + idPlayer;

            Grid gridScorePlayer = XamlReader.Parse(XamlWriter.Save(gridScorePlayerTemplate)) as Grid;
            gridScorePlayer.Visibility = Visibility.Visible;
            gridScorePlayer.Name = idGrid + splitSymbol + idGridScorePlayer;

            string username = GetUsernamePlayerById(scorePlayer.IdPlayer);
            UpdateLabel(gridScorePlayer, idLbPosition, positionSymbol + position);
            UpdateLabel(gridScorePlayer, idLbUsernamePlayer, username);
            UpdateLabel(gridScorePlayer, idLbVictorries, scorePlayer.WinsNumber.ToString());

            if (_playerLoggedIn.Username == username)
            {
                gridScorePlayer.Background = Utilities.CreateColorFromHexadecimal(IDENTIFIER_CURRENT_PLAYER_HEX_COLOR);
            }

            return gridScorePlayer;
        }

        private void UpdateLabel(Grid grid, string labelName, string content)
        {
            Label label = FindLabel(grid, labelName);
            label.Content = content;
        }

        private Label FindLabel(Grid gridForSearch, string name)
        {
            return gridForSearch.FindName(name) as Label;
        }

        private string GetUsernamePlayerById(int idPlayer)
        {
            string username = string.Empty;
            UserManagerClient userManagerClient = new UserManagerClient();

            try
            {
                username = userManagerClient.GetUsernameByIdPlayer(idPlayer);
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

            return username;
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            UnsuscribeToGlobalScoreboard();

            NavigationService.GoBack();
        }

        public void BtnCloseWindow_Click()
        {
            UnsuscribeToGlobalScoreboard();
        }

        public void NotifyGlobalScoreboardUpdated()
        {
            UpdateGlobalScoreboard();
        }

        private void UpdateGlobalScoreboard()
        {
            ClearScoreBoard();
            GetAllScorePlayers();
            SetScores();
        }

        private void ClearScoreBoard()
        {
            stackPanelScoreboard.Children.Clear();
        }
    }
}
