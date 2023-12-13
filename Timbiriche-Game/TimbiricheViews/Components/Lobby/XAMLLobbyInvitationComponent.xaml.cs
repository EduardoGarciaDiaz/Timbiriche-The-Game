using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Components.Lobby
{
    public partial class XAMLLobbyInvitationComponent : Window
    {
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private readonly Window _mainWindow;
        private readonly string _lobbyCode;

        public XAMLLobbyInvitationComponent(string lobbyCode)
        {
            InitializeComponent();

            _mainWindow = Application.Current.MainWindow;
            _lobbyCode = lobbyCode;

            ConfigureEmergentWindow();
        }

        private void ConfigureEmergentWindow()
        {
            this.Owner = _mainWindow;
            tbkLobbyCode.Text = _lobbyCode;
            SetSizeWindow();
            SetCenterWindow();
        }

        private void SetSizeWindow()
        {
            this.Width = _mainWindow.Width;
            this.Height = _mainWindow.Height;
        }

        private void SetCenterWindow()
        {
            double centerX = _mainWindow.Left + (_mainWindow.Width - this.Width) / 2;
            double centerY = _mainWindow.Top + (_mainWindow.Height - this.Height) / 2;
            this.Left = centerX;
            this.Top = centerY;
        }

        private void ImgClose_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void BtnInviteByEmail_Click(object sender, RoutedEventArgs e)
        {
            borderInviteByCode.Visibility = Visibility.Collapsed;
            borderInviteByEmail.Visibility = Visibility.Visible;
        }

        private void BtnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_lobbyCode);
        }

        private void BtnSendInvitation_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateEmail())
            {
                Server.InvitationManagerClient invitationManagerClient = new Server.InvitationManagerClient();

                try
                {
                    invitationManagerClient.SendInvitationToEmail(_lobbyCode, tbxFriendEmail.Text.Trim());
                }
                catch (EndpointNotFoundException ex)
                {
                    EmergentWindows.CreateConnectionFailedMessageWindow();
                    HandlerException.HandleComponentErrorException(ex);
                }
                catch (TimeoutException ex)
                {
                    EmergentWindows.CreateTimeOutMessageWindow();
                    HandlerException.HandleComponentErrorException(ex);
                }
                catch (FaultException)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                }
                catch (CommunicationException ex)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                    HandlerException.HandleComponentErrorException(ex);
                }
                catch (Exception ex)
                {
                    EmergentWindows.CreateUnexpectedErrorMessageWindow();
                    HandlerException.HandleComponentFatalException(ex);
                }
            }   
        }

        private void TbxFriendEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbxFriendEmail.Text == (string)tbxFriendEmail.Tag)
            {
                tbxFriendEmail.Text = string.Empty;
                tbxFriendEmail.Foreground = Brushes.Black;
            }
        }

        private void TbxFriendEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxFriendEmail.Text))
            {
                tbxFriendEmail.Text  = (string)tbxFriendEmail.Tag;
                SolidColorBrush placeholderBrush = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
                tbxFriendEmail.Foreground = placeholderBrush;
            }
        }

        private bool ValidateEmail()
        {
            SetDefaultStyles();
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";

            if (!ValidationUtilities.IsValidEmail(tbxFriendEmail.Text) || tbxFriendEmail.Text.Equals(tbxFriendEmail.Tag))
            {
                tbxFriendEmail.Style = (Style)FindResource(errorTextBoxStyle);
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";
            tbxFriendEmail.Style = (Style)FindResource(normalTextBoxStyle);
        }
    }
}
