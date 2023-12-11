using System;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLGlobalScoreboard : Page, IGlobalScoreManagerCallback
    {
        private GlobalScore[] _globalScores;
        private readonly Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private const string IDENTIFIER_CURRENT_PLAYER_HEX_COLOR = "#FF232B9B";

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

        private void GetAllScorePlayers()
        {
            Server.ScoreboardManagerClient scoreboardManagerClient = new Server.ScoreboardManagerClient();
            try
            {
                _globalScores = scoreboardManagerClient.GetGlobalScores(PlayerSingleton.Player.Username);
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
            const string ID_GRID = "grid";
            const string ID_LB_POSITION = "lbPosition";
            const string ID_LB_USERNAME_PLAYER = "lbUsernamePlayer";
            const string ID_LB_VICTORIES = "lbVictories";
            const string POSITION_SYMBOL = "#";
            const string SEPARATOR_SYMBOL = "_";

            string idGlobalScore = scorePlayer.IdGlobalScore.ToString();
            string idPlayer = scorePlayer.IdPlayer.ToString();
            string idGridScorePlayer = idGlobalScore + SEPARATOR_SYMBOL + idPlayer;

            Grid gridScorePlayer = XamlReader.Parse(XamlWriter.Save(gridScorePlayerTemplate)) as Grid;
            gridScorePlayer.Visibility = Visibility.Visible;
            gridScorePlayer.Name = ID_GRID + SEPARATOR_SYMBOL + idGridScorePlayer;

            string username = GetUsernamePlayerById(scorePlayer.IdPlayer);
            UpdateLabel(gridScorePlayer, ID_LB_POSITION, POSITION_SYMBOL + position);
            UpdateLabel(gridScorePlayer, ID_LB_USERNAME_PLAYER, username);
            UpdateLabel(gridScorePlayer, ID_LB_VICTORIES, scorePlayer.WinsNumber.ToString());

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
            Label lbFound = gridForSearch.FindName(name) as Label;

            return lbFound;
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
