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
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{

    public partial class XAMLLogin : Page
    {

        const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        const string MAIN_FONT = "Titan One";
        const string SECONDARY_FONT = "Inter";

        public XAMLLogin()
        {
            InitializeComponent();
        }

        private void ChangeLanguage()
        {
            string language = "";
            string spanishMXLanguage = "es-MX";
            string englishUSLanguage = "en";
            
            if (lbLanguage.Content.Equals("Español"))
            {
                language = englishUSLanguage;
            }

            if(lbLanguage.Content.Equals("English"))
            {
                language = spanishMXLanguage;
            }
            
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            XAMLLogin newLoginPage = new XAMLLogin();

            if (language.Equals(englishUSLanguage))
            {
                newLoginPage.imgUsaFlag.Visibility = Visibility.Visible;
                newLoginPage.imgMexicoFlag.Visibility = Visibility.Hidden;
            }

            if (language.Equals(spanishMXLanguage))
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

            if (!Utilities.IsValidUsername(tbxUsername.Text) || tbxUsername.Text.Equals(tbxUsername.Tag))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }

            if (!Utilities.IsValidPassword(pwBxPassword.Password) || pwBxPassword.Password.Equals(pwBxPassword.Tag))
            {
                pwBxPasswordMask.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            return isValid;
        }

        private void SetDefaultStyles()
        {
            tbxUsername.Style = (Style)FindResource("NormalTextBoxStyle");
            pwBxPasswordMask.Style = (Style)FindResource("NormalTextBoxStyle");
        }

        private void OnClickChangeLanguage(object sender, MouseButtonEventArgs e)
        {
            ChangeLanguage();            
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
                Server.Player playerLogged = null;
                try
                {
                    playerLogged = userManagerClient.ValidateLoginCredentials(tbxUsername.Text, pwBxPassword.Password);
                }
                catch (EndpointNotFoundException ex)
                {
                    Utilities.CreateConnectionFailedMessageWindow();
                    // TODO: Log the excepction
                }   // TODO: EXCEPTION FOR TIME
                if (playerLogged != null)
                {
                    PlayerSingleton.player = playerLogged;
                    NavigationService.Navigate(new XAMLLobby());
                }
            }
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
                Color placeholderColor = (Color)ColorConverter.ConvertFromString(PLACEHOLDER_HEX_COLOR);
                SolidColorBrush placeholderBrush = new SolidColorBrush(placeholderColor);
                tbxUsername.Foreground = placeholderBrush;
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
                Color placeholderColor = (Color)ColorConverter.ConvertFromString(PLACEHOLDER_HEX_COLOR);
                SolidColorBrush placeholderBrush = new SolidColorBrush(placeholderColor);
                pwBxPassword.Foreground = placeholderBrush;
            }
        }

    }

}
