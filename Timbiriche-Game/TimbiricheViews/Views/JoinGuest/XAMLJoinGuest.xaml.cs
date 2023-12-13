using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLJoinGuest : Page
    {
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string MAIN_FONT = "Titan One";
        private const string SECONDARY_FONT = "Inter";
        private readonly string _lobbyCode;

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

            if (ValidateFields() && IsUniqueIdentifier(username))
            {
                JoinToLobby(username);
            }
        }

        private bool ValidateFields()
        {
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";
            SetDefaultStyles();

            if (!ValidationUtilities.IsValidUsername(tbxUsername.Text) || tbxUsername.Text.Equals(tbxUsername.Tag))
            {
                tbxUsername.Style = (Style)FindResource(errorTextBoxStyle);
                ImgUsernameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }
            return isValid;
        }

        private void SetDefaultStyles()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";

            tbxUsername.Style = (Style)FindResource(normalTextBoxStyle);
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

            return isUsernameUnique;
        }

        private void JoinToLobby(string username)
        {
            InitializatePlayerSingleton(username);
            NavigationService.Navigate(new XAMLGuestLobby(_lobbyCode));
        }

        private void InitializatePlayerSingleton(string username)
        {
            int idDefaultStyleSelected = 1;
            Server.Player guestPlayer = new Server.Player
            {
                Username = username,
                IdStyleSelected = idDefaultStyleSelected
            };

            PlayerSingleton.Player = guestPlayer;
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
