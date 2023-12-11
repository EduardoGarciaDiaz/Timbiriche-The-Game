using Serilog;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TimbiricheViews.Components;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{

    public partial class XAMLJoinGuest : Page
    {
        private ILogger _logger = LoggerManager.GetLogger();
        private string _lobbyCode;
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string MAIN_FONT = "Titan One";
        private const string SECONDARY_FONT = "Inter";

        public XAMLJoinGuest()
        {
            InitializeComponent();
        }

        public XAMLJoinGuest(string lobbyCode)
        {
            InitializeComponent ();
            _lobbyCode = lobbyCode;
        }

        private void TbxUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbxUsername.Text == (string)tbxUsername.Tag) 
            {
                tbxUsername.Text = string.Empty;
                tbxUsername.Foreground = Brushes.Black;
                tbxUsername.FontFamily = new FontFamily(SECONDARY_FONT);
                tbxUsername.FontWeight = FontWeights.Bold;
            }
        }

        private void TbxUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxUsername.Text))
            {
                tbxUsername.Text = (string) tbxUsername.Tag;
                SolidColorBrush placeholderColor = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
                tbxUsername.Foreground = placeholderColor;
                tbxUsername.FontFamily = new FontFamily(MAIN_FONT);
            }
        }

        private void BtnReady_Click(object sender, RoutedEventArgs e)
        {
            string username = tbxUsername.Text.Trim();

            if (ValidateFields())
            {
                if (IsUniqueIdentifier(username)) //TODO: Validate the guest with same username
                {
                    JoinToLobby(username);
                }
            }
        }

        private bool ValidateFields()
        {
            SetDefaultStyles();
            bool isValid = true;

            if (!ValidationUtilities.IsValidUsername(tbxUsername.Text) || tbxUsername.Text.Equals(tbxUsername.Tag))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgUsernameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }
            return isValid;
        }

        private void SetDefaultStyles()
        {
            tbxUsername.Style = (Style)FindResource("NormalTextBoxStyle");
            lbExistentUsername.Visibility = Visibility.Hidden;
            ImgUsernameErrorDetails.Visibility = Visibility.Hidden;
        }

        public bool IsUniqueIdentifier(string username)
        {
            bool isUsernameUnique = true;
            try
            {
                Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
                if (userManagerClient.ValidateUniqueIdentifierUser(username))
                {
                    isUsernameUnique = false;
                    lbExistentUsername.Visibility = Visibility.Visible;
                }
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);

                NavigationService.Navigate(new XAMLLogin());
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);

                NavigationService.Navigate(new XAMLLogin());
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

                NavigationService.Navigate(new XAMLLogin());
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);

                NavigationService.Navigate(new XAMLLogin());
            }

            return isUsernameUnique;
        }

        private void JoinToLobby(string username)
        {
            InitializatePlayerSingleton(username);
            NavigationService.Navigate(new XAMLGuestLobby(_lobbyCode));
        }

        private void InitializatePlayerSingleton(string username)
        {
            const int ID_DEFAULT_STYLE_SELECTED = 1;
            Server.Player guestPlayer = new Server.Player();
            guestPlayer.Username = username;
            guestPlayer.IdStyleSelected = ID_DEFAULT_STYLE_SELECTED;
            PlayerSingleton.Player = guestPlayer;
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
