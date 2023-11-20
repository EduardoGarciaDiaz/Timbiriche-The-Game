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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLPasswordReset : Page
    {
        const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private string _email;

        public XAMLPasswordReset()
        {
            InitializeComponent();
        }

        private bool ValidatePassword()
        {
            SetDefaultStyles();
            bool isValid = true;

            if (!ValidationUtilities.IsValidPassword(pwBxNewPassword.Password) || pwBxNewPassword.Password.Equals(pwBxNewPassword.Tag))
            {
                pwBxNewPasswordMask.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }

            if (!ValidationUtilities.IsValidPassword(pwBxConfirmNewPassword.Password) || pwBxConfirmNewPassword.Password.Equals(pwBxConfirmNewPassword.Tag))
            {
                pwBxConfirmNewPasswordMask.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }

            if (pwBxConfirmNewPassword.Password != pwBxNewPassword.Password)
            {
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            pwBxNewPasswordMask.Style = (Style)FindResource("NormalTextBoxStyle");
            pwBxConfirmNewPasswordMask.Style = (Style)FindResource("NormalTextBoxStyle");
        }

        private void BtnSendToken_Click(object sender, RoutedEventArgs e)
        {
            _email = tbxEmail.Text;
            Server.PasswordResetClient passwordResetClient = new Server.PasswordResetClient();
            bool isResetTokenSent = passwordResetClient.SendResetToken(_email);
            if (isResetTokenSent)
            {
                gridEmailConfirmation.Visibility = Visibility.Collapsed;
                gridCodeConfirmation.Visibility = Visibility.Visible;
            }
        }

        private void BtnVerifyToken_Click(object sender, RoutedEventArgs e)
        {
            Server.PasswordResetClient passwordResetClient = new Server.PasswordResetClient();
            bool isTokenValid = passwordResetClient.ValidateResetToken(_email, Int32.Parse(tbxToken.Text));
            if (isTokenValid)
            {
                gridCodeConfirmation.Visibility = Visibility.Collapsed;
                gridNewPassword.Visibility = Visibility.Visible;
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatePassword())
            {
                string password = pwBxNewPassword.Password.Trim();
                Server.PasswordResetClient passwordResetClient = new Server.PasswordResetClient();
                bool isPasswordRessetted = passwordResetClient.ChangePassword(password, _email);
                if (isPasswordRessetted)
                {
                    string title = "Contraseña Cambiada";
                    string message = "La contraseña fue cambiada con éxito";
                    EmergentWindows.CreateEmergentWindow(title, message);
                    NavigationService.GoBack();
                }
                else
                {
                    string title = "Error al Cambiar Contraseña";
                    string message = "Hubo un error al cambiar la contraeña. Intentelo mas tarde.";
                    EmergentWindows.CreateEmergentWindow(title, message);
                }
            }
        }

        private void PwBxConfirmNewPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pwBxConfirmNewPassword.Password == (string)pwBxConfirmNewPassword.Tag)
            {
                pwBxConfirmNewPassword.Password = string.Empty;
                pwBxConfirmNewPassword.Foreground = Brushes.Black;
            }
        }

        private void PwBxConfirmNewPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pwBxConfirmNewPassword.Password))
            {
                pwBxConfirmNewPassword.Password = (string)pwBxConfirmNewPassword.Tag;
                Color placeholderColor = (Color)ColorConverter.ConvertFromString(PLACEHOLDER_HEX_COLOR);
                SolidColorBrush placeholderBrush = new SolidColorBrush(placeholderColor);
                pwBxConfirmNewPassword.Foreground = placeholderBrush;
            }
        }

        private void PwBxNewPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pwBxNewPassword.Password == (string)pwBxNewPassword.Tag)
            {
                pwBxNewPassword.Password = string.Empty;
                pwBxNewPassword.Foreground = Brushes.Black;
            }
        }

        private void PwBxNewPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pwBxNewPassword.Password))
            {
                pwBxNewPassword.Password = (string)pwBxNewPassword.Tag;
                Color placeholderColor = (Color)ColorConverter.ConvertFromString(PLACEHOLDER_HEX_COLOR);
                SolidColorBrush placeholderBrush = new SolidColorBrush(placeholderColor);
                pwBxNewPassword.Foreground = placeholderBrush;
            }
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
