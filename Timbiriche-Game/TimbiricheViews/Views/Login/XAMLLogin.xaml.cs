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
    public partial class XAMLLogin : Page
    {
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string MAIN_FONT = "Titan One";
        private const string SECONDARY_FONT = "Inter";
        private const string SPANISH_MX_CODE = "es-MX";
        private const string ENGLISH_US_CODE = "en";

        public XAMLLogin()
        {
            InitializeComponent();
        }

        private void ChangeLanguage()
        {
            string spanishMXLanguage = "Español";
            string englishUSLanguage = "English";
            string language = "";
            
            if (lbLanguage.Content.Equals(spanishMXLanguage))
            {
                language = ENGLISH_US_CODE;
            }

            if(lbLanguage.Content.Equals(englishUSLanguage))
            {
                language = SPANISH_MX_CODE;
            }

            ToggleLanguage(language);
        }

        private void ToggleLanguage(string language)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            XAMLLogin newLoginPage = new XAMLLogin();

            if (language.Equals(ENGLISH_US_CODE))
            {
                newLoginPage.imgUsaFlag.Visibility = Visibility.Visible;
                newLoginPage.imgMexicoFlag.Visibility = Visibility.Hidden;
            }

            if (language.Equals(SPANISH_MX_CODE))
            {
                newLoginPage.imgMexicoFlag.Visibility = Visibility.Visible;
                newLoginPage.imgUsaFlag.Visibility = Visibility.Hidden;
            }

            NavigationService.Navigate(newLoginPage);
        }

        private bool ValidateFields()
        {
            SetDefaultStyles();
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";

            if (!ValidationUtilities.IsValidUsername(tbxUsername.Text) || tbxUsername.Text.Equals(tbxUsername.Tag))
            {
                tbxUsername.Style = (Style)FindResource(errorTextBoxStyle);
                isValid = false;
            }

            if (!ValidationUtilities.IsValidPassword(pwBxPassword.Password) || pwBxPassword.Password.Equals(pwBxPassword.Tag))
            {
                pwBxPasswordMask.Style = (Style)FindResource(errorTextBoxStyle);
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";

            tbxUsername.Style = (Style)FindResource(normalTextBoxStyle);
            pwBxPasswordMask.Style = (Style)FindResource(normalTextBoxStyle);
            lbIncorrectCredentials.Visibility = Visibility.Hidden;
        }

        private void RectangleChangeLanguage_Click(object sender, MouseButtonEventArgs e)
        {
            ChangeLanguage();            
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            TryToLogin();
        }

        private void TryToLogin()
        {
            if (ValidateFields())
            {
                Server.Player playerLogged = ValidateLoginCredentials();

                if (playerLogged != null)
                {
                    ProcessLoggedInPlayer(playerLogged);
                }
                else
                {
                    lbIncorrectCredentials.Visibility = Visibility.Visible;
                }
            }
        }

        private Server.Player ValidateLoginCredentials()
        {
            UserManagerClient userManagerClient = new UserManagerClient();
            Server.Player playerLoging = null;

            try
            {
                playerLoging = userManagerClient.ValidateLoginCredentials(tbxUsername.Text.Trim(), pwBxPassword.Password.Trim());
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
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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

            return playerLoging;
        }

        private void ProcessLoggedInPlayer(Server.Player playerLogged)
        {
            if (ValidateIsAdmitedPlayer(playerLogged))
            {
                PlayerSingleton.Player = playerLogged;
                NavigationService.Navigate(new XAMLLobby());
            }
        }

        private bool ValidateIsAdmitedPlayer(Server.Player playerLogged)
        {
            bool isAdmittedPlayer = true;
            bool isPlayerBanned = ValidatePlayerIsNotBanned(playerLogged);
            bool isPlayerAlreadyOnline = ValidateIsPlayerAlreadyOnline(playerLogged.Username);

            if (isPlayerBanned)
            {
                isAdmittedPlayer = false;
                GoToPlayerBannedPage(playerLogged);
            }
            else if (isPlayerAlreadyOnline)
            {
                isAdmittedPlayer = false;
                ShowPlayerAlreadyOnlineMessage();
            }

            return isAdmittedPlayer;
        }

        private bool ValidatePlayerIsNotBanned(Server.Player player)
        {
            string statusBanned = "Banned";

            bool isPlayerBanned = false;

            if (player.Status.Equals(statusBanned))
            {
                isPlayerBanned = true;
            }

            return isPlayerBanned;
        }

        private bool ValidateIsPlayerAlreadyOnline(string username)
        {
            bool isAlreadyOnline = true;

            UserManagerClient userManagerClient = new UserManagerClient();

            try
            {
                isAlreadyOnline = userManagerClient.ValidateIsUserAlreadyOnline(username);
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
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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

            return isAlreadyOnline;
        }

        private void GoToPlayerBannedPage(Server.Player playerLogged)
        {
            NavigationService.Navigate(new XAMLBan(playerLogged.IdPlayer));
        }

        private void ShowPlayerAlreadyOnlineMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbExistentSessionTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkExistentSessionDescription;

            EmergentWindows.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLUserForm());
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

        private void PwBxPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pwBxPassword.Password == (string)pwBxPassword.Tag)
            {
                pwBxPassword.Password = string.Empty;
                pwBxPassword.Foreground = Brushes.Black;
            }
        }

        private void PwBxPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pwBxPassword.Password))
            {
                pwBxPassword.Password = (string)pwBxPassword.Tag;
                SolidColorBrush placeholderColor = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
                pwBxPassword.Foreground = placeholderColor;
            }
        }

        private void TbxJoinByCode_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbxJoinByCode.Text == (string)tbxJoinByCode.Tag)
            {
                tbxJoinByCode.Text = string.Empty;
                tbxJoinByCode.Foreground = Brushes.Black;
            }
        }

        private void TbxJoinByCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxJoinByCode.Text))
            {
                tbxJoinByCode.Text = (string)tbxJoinByCode.Tag;
                SolidColorBrush placeholderColor = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
                tbxJoinByCode.Foreground = placeholderColor;
            }
        }

        private void BtnForgottenPassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLPasswordReset());
        }

        private void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            bool existLobby = false;
            string lobbyCode = tbxJoinByCode.Text.Trim().ToUpper();
            Server.LobbyExistenceCheckerClient lobbyExistenceCheckerClient = new Server.LobbyExistenceCheckerClient();

            try
            {
                existLobby = lobbyExistenceCheckerClient.ExistLobbyCode(lobbyCode);
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
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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

            if (existLobby)
            {
                NavigationService.Navigate(new XAMLJoinGuest(lobbyCode));
            } 
            else
            {
                EmergentWindows.CreateLobbyNotFoundMessageWindow();
            }
        }
    }
}
