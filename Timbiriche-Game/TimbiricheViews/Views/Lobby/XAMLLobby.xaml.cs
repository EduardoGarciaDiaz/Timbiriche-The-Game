using System;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
        private const float MINIMIUN_MATCH_DURATION_IN_MINUTES = 1;
        private readonly Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private float _matchDurationInMinutes = DEFAULT_MATCH_DURATION_IN_MINUTES;

        public XAMLLobby()
        {
            InitializeComponent();

            btnSignOff.Visibility = Visibility.Visible;
            btnShop.IsEnabled = true;

            bool isRematch = false;

            if (ConfigureLobby(isRematch))
            {
                this.Loaded += Lobby_Loaded;
            }
        }

        public XAMLLobby(string lobbyCode, bool isHost)
        {
            InitializeComponent();

            _lobbyCode = lobbyCode;

            bool isRematch = true;

            if (ConfigureLobby(isRematch))
            {
                ConfigureRematch(isHost);
            }
        }

        private bool ConfigureLobby(bool isRematch)
        {
            bool areSuccessMethods = false;

            try
            {
                LoadDataPlayer();
                LoadPlayerFriends();
                ShowAsActiveUser();
                RestartSelectedColor(isRematch);

                areSuccessMethods = true;
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

            return areSuccessMethods;
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

        private LobbyInformation ConfigureLobbyInformation()
        {
            float turnDurationInMinutes = 0.5F;

            LobbyInformation lobbyInformation = new LobbyInformation
            {
                TurnDurationInMinutes = turnDurationInMinutes,
                MatchDurationInMinutes = _matchDurationInMinutes
            };

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