﻿using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using TimbiricheViews.Server;
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
            _email = tbxEmail.Text.Trim();
            bool isResetTokenSent = false;
            Server.PasswordResetClient passwordResetClient = new Server.PasswordResetClient();

            try
            {
                isResetTokenSent = passwordResetClient.SendResetToken(_email);
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
            catch (FaultException<TimbiricheServerException>)
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
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

            if (isResetTokenSent)
            {
                gridEmailConfirmation.Visibility = Visibility.Collapsed;
                gridCodeConfirmation.Visibility = Visibility.Visible;
            }
        }

        private void BtnVerifyToken_Click(object sender, RoutedEventArgs e)
        {
            bool isTokenValid = false;
            Server.PasswordResetClient passwordResetClient = new Server.PasswordResetClient();

            try
            {
                isTokenValid = passwordResetClient.ValidateResetToken(_email, Int32.Parse(tbxToken.Text.Trim()));
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
            catch (FaultException<TimbiricheServerException>)
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
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

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
                bool isPasswordReseted = ChangePassword(password);
                HandleResultOfChangePassword(isPasswordReseted);
            }
        }

        private bool ChangePassword(string password)
        {
            PasswordResetClient passwordResetClient = new PasswordResetClient();
            bool isPasswordReseted = false;

            try
            {
                isPasswordReseted = passwordResetClient.ChangePassword(password, _email);
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
            catch (FaultException<TimbiricheServerException>)
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
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

            return isPasswordReseted;
        }

        private void HandleResultOfChangePassword(bool isPasswordReseted)
        {
            if (isPasswordReseted)
            {
                string title = Properties.Resources.lbPasswordResetedTitle;
                string message = Properties.Resources.tbkPasswordResetedDescription;
                EmergentWindows.CreateEmergentWindow(title, message);
                NavigationService.GoBack();
            }
            else
            {
                string title = Properties.Resources.lbErrorPasswordResetTitle;
                string message = Properties.Resources.tbkErrorPasswordResetDescription;
                EmergentWindows.CreateEmergentWindow(title, message);
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
                SolidColorBrush placeholderBrush = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
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
                SolidColorBrush placeholderBrush = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
                pwBxNewPassword.Foreground = placeholderBrush;
            }
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
