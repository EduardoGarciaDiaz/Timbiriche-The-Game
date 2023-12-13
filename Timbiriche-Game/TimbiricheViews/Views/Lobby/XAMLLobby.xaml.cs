using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    public partial class XAMLLobby : Page
    {
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string NOT_SELECTED_BUTTON_HEX_COLOR = "#FF13546C";
        private const string SELECTED_BUTTON_HEX_COLOR = "#FF063343";
        private const float DEFAULT_MATCH_DURATION_IN_MINUTES = 5;
        private const float MAXIMIUN_MATCH_DURATION_IN_MINUTES = 20;
        private const float MINIMIUN_MATCH_DURATION_IN_MINUTES = 2;
        private readonly Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private float _matchDurationInMinutes = DEFAULT_MATCH_DURATION_IN_MINUTES;

        public XAMLLobby()
        {
            InitializeComponent();
            ShowAsActiveUser();
            LoadDataPlayer();
            LoadPlayerFriends();

            bool isRematch = false;
            RestartSelectedColor(isRematch);

            this.Loaded += Lobby_Loaded;
        }

        public XAMLLobby(string lobbyCode, bool isHost)
        {
            InitializeComponent();

            _lobbyCode = lobbyCode;

            LoadDataPlayer();
            LoadPlayerFriends();

            bool isRematch = true;
            RestartSelectedColor(isRematch);
            ConfigureRematch(isHost);
        }

        private void ConfigureMatch()
        {
            ShowMatchSettingsGrid();
        }

        private void ShowMatchSettingsGrid()
        {
            lbMatchTime.Content = _matchDurationInMinutes;
            gridMatchSettings.Visibility = Visibility.Visible;
        }

        private void BtnAcceptSettings_Click(object sender, RoutedEventArgs e)
        {
            _matchDurationInMinutes = (float)lbMatchTime.Content;
            gridMatchSettings.Visibility = Visibility.Collapsed;

            ConfigureMatchSettings();
        }

        private void ConfigureMatchSettings()
        {
            LobbyInformation lobbyInformation = ConfigureLobbyInformation();
            LobbyPlayer lobbyPlayer = ConfigureLobbyPlayer();

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            try
            {
                client.CreateLobby(lobbyInformation, lobbyPlayer);
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

        private LobbyInformation ConfigureLobbyInformation()
        {
            float turnDurationInMinutes = 0.5F;

            LobbyInformation lobbyInformation = new LobbyInformation();
            lobbyInformation.TurnDurationInMinutes = turnDurationInMinutes;
            lobbyInformation.MatchDurationInMinutes = _matchDurationInMinutes;

            return lobbyInformation;
        }

        private LobbyPlayer ConfigureLobbyPlayer()
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer
            {
                Username = _playerLoggedIn.Username,
                IdStylePath = _playerLoggedIn.IdStyleSelected
            };

            return lobbyPlayer;
        }

        private void BtnIncrementTime_Click(object sender, RoutedEventArgs e)
        {
            IncrementMatchDuration();
        }

        private void IncrementMatchDuration()
        {
            if (_matchDurationInMinutes < MAXIMIUN_MATCH_DURATION_IN_MINUTES)
            {
                _matchDurationInMinutes++;
                lbMatchTime.Content = _matchDurationInMinutes;
            }
        }

        private void BtnDecrementTime_Click(object sender, RoutedEventArgs e)
        {
            DecrementMatchDuration();
        }

        private void DecrementMatchDuration()
        {
            if (_matchDurationInMinutes > MINIMIUN_MATCH_DURATION_IN_MINUTES)
            {
                _matchDurationInMinutes--;
                lbMatchTime.Content = _matchDurationInMinutes;
            }
        }

        private void ImgCloseMatchSettings_Click(object sender, RoutedEventArgs e)
        {
            gridMatchSettings.Visibility = Visibility.Collapsed;
        }
    }
}