﻿using Serilog;
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

    public partial class XAMLLogin : Page
    {
        private ILogger _logger = LoggerManager.GetLogger();
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string MAIN_FONT = "Titan One";
        private const string SECONDARY_FONT = "Inter";

        public XAMLLogin()
        {
            InitializeComponent();
        }

        private void ChangeLanguage()
        {
            string spanishMXLanguage = "es-MX";
            string englishUSLanguage = "en";
            string language = "";

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

            if (!ValidationUtilities.IsValidUsername(tbxUsername.Text) || tbxUsername.Text.Equals(tbxUsername.Tag))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }

            if (!ValidationUtilities.IsValidPassword(pwBxPassword.Password) || pwBxPassword.Password.Equals(pwBxPassword.Tag))
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
            lbIncorrectCredentials.Visibility = Visibility.Hidden;
        }

        private void RectangleChangeLanguage_Click(object sender, MouseButtonEventArgs e)
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
                    EmergentWindows.CreateConnectionFailedMessageWindow();
                    _logger.Error(ex.StackTrace);
                }
                catch (TimeoutException ex)
                {
                    EmergentWindows.CreateTimeOutMessageWindow();
                    //TODO: log
                }
                catch (FaultException<TimbiricheServerException> ex)
                {
                    //TODO: Show emergent window and log
                    Console.WriteLine("Upss... Ocurrió un error en la base de datos, por favor inténtelo de nuevo");
                }
                catch (FaultException ex)
                {
                    //TODO: Show emergent window and log
                    Console.WriteLine("Upss... Ocurrió un error en la base de datos. Por favor inténtelo de nuevo");
                    Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                }
                catch (CommunicationException ex)
                {
                    //TODO: Show emergent window and log
                }
                catch (Exception ex)
                {
                    //TODO: Show emergent window and log...Ups has ocurried an unexpected error. Please try again later
                    Console.WriteLine("Upss... Ocurrió un error inesperado. Por favor, intentelo más tarde");
                }

                if (playerLogged != null)
                {
                    bool isPlayerBanned = ValidatePlayerIsNotBanned(playerLogged);

                    if (isPlayerBanned)
                    {
                        NavigationService.Navigate(new XAMLBan(playerLogged.IdPlayer));
                    }
                    else
                    {
                        PlayerSingleton.Player = playerLogged;
                        NavigationService.Navigate(new XAMLLobby());
                    }
                } 
                else
                {
                    lbIncorrectCredentials.Visibility = Visibility.Visible;
                }
            }
            
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
            string lobbyCode = tbxJoinByCode.Text.Trim().ToUpper();
            Server.LobbyExistenceCheckerClient lobbyExistenceCheckerClient = new Server.LobbyExistenceCheckerClient();
            bool existLobby = lobbyExistenceCheckerClient.ExistLobbyCode(lobbyCode);
            if (existLobby)
            {
                NavigationService.Navigate(new XAMLJoinGuest(lobbyCode));
            } 
            else
            {
                string title = "Lobby no encontrado";
                string message = "El lobby al que estas intentando entrar no existe.";
                EmergentWindows.CreateEmergentWindow(title, message);
            }
        }
    }
}
