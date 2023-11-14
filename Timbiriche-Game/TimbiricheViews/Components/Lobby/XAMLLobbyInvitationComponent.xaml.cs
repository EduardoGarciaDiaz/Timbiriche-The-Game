using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Components.Lobby
{
    public partial class XAMLLobbyInvitationComponent : Window
    {
        const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";

        private Window _mainWindow;
        private string _lobbyCode;

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
            double centerX = _mainWindow.Left + (_mainWindow.Width - _mainWindow.Width) / 2;
            double centerY = _mainWindow.Top + (_mainWindow.Height - _mainWindow.Height) / 2;
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
                invitationManagerClient.SendInvitationToEmail(_lobbyCode, tbxFriendEmail.Text.Trim());
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
                Color placeholderColor = (Color)ColorConverter.ConvertFromString(PLACEHOLDER_HEX_COLOR);
                SolidColorBrush placeholderBrush = new SolidColorBrush(placeholderColor);
                tbxFriendEmail.Foreground = placeholderBrush;
            }
        }

        private bool ValidateEmail()
        {
            SetDefaultStyles();
            bool isValid = true;

            if (!Utilities.IsValidEmail(tbxFriendEmail.Text) || tbxFriendEmail.Text.Equals(tbxFriendEmail.Tag))
            {
                tbxFriendEmail.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            tbxFriendEmail.Style = (Style)FindResource("NormalTextBoxStyle");
        }
    }
}
