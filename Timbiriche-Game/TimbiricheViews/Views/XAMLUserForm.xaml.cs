﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
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
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    /// <summary>
    /// Lógica de interacción para XAMLUserForm.xaml
    /// </summary>
    public partial class XAMLUserForm : Page
    {
        public XAMLUserForm(String language)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(language);
            InitializeComponent();
            ImgBack.MouseLeftButtonDown += ImgBack_Click;
        }

        public XAMLUserForm()
        {
            InitializeComponent();
            ImgBack.MouseLeftButtonDown += ImgBack_Click;
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DpBirthdate_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                datePicker.DisplayDateEnd = DateTime.Today.AddYears(-3);
            }
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                DateTime.TryParse(dpBirthdate.Text, out DateTime birthdate);
                Account newAccount = new Account()
                {
                    name = tbxName.Text.Trim(),
                    lastName = tbxLastName.Text.Trim(),
                    surname = tbxSurname.Text.Trim(),
                    birthdate = birthdate
                };

                Server.Player newPlayer = new Server.Player()
                {
                    username = tbxUsername.Text.Trim(),
                    email = tbxEmail.Text.Trim().ToLower(),
                    password = pwBxPassword.Password.Trim(),
                    coins = 0,
                    status = "Not-Banned",
                    accountFK = newAccount
                };
                
                if (!ValidateUniqueIdentifier(newPlayer))
                {
                    Server.UserManagerClient client = new Server.UserManagerClient();
                    int rowsAffected = client.AddUser(newPlayer);
                    if (rowsAffected > 0)
                    {
                        EmergentWindow emergentWindow = new EmergentWindow(
                            Properties.Resources.lbTitleAccountCreatedSuccess,
                            Properties.Resources.tbkDescriptionAccountCreatedSuccess
                        );
                        emergentWindow.ShowDialog();
                        NavigationService.GoBack();
                    }
                    else
                    {
                        EmergentWindow emergentWindow = new EmergentWindow(
                            Properties.Resources.lbTitleCreateAccountFail,
                            Properties.Resources.tbkDescriptionCreateAccountFail
                        );
                    }
                }
            }
        }

        public bool ValidateUniqueIdentifier(Server.Player newPlayer)
        {
            bool existUserIdentifier = false;
            Server.UserManagerClient client = new Server.UserManagerClient();
            if (client.ValidateUniqueIdentifierUser(newPlayer.email))
            {
                existUserIdentifier = true;
                lbExistentEmail.Visibility = Visibility.Visible;
            }
            if (client.ValidateUniqueIdentifierUser(newPlayer.username))
            {
                existUserIdentifier = true;
                lbExistentUsername.Visibility = Visibility.Visible;
            }
            return existUserIdentifier;
        }

        public bool ValidateFields()
        {
            SetDefaultStyles();
            ValidatePasswordProperties();
            bool isValid = true;

            if (!Utilities.IsValidPersonalInformation(tbxName.Text))
            {
                tbxName.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgNameErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidPersonalInformation(tbxLastName.Text))
            {
                tbxLastName.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgLastNameErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidEmail(tbxEmail.Text))
            {
                tbxEmail.Style = (Style)FindResource("ErrorTextBoxStyle");
                lbEmailError.Visibility = Visibility.Visible;
                ImgEmailErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidUsername(tbxUsername.Text))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgUsernameErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidPassword(pwBxPassword.Password))
            {
                pwBxPassword.Style = (Style)FindResource("ErrorPasswordBoxStyle");
                ImgPasswordErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!DateTime.TryParse(dpBirthdate.Text, out _))
            {
                dpBirthdate.Style = (Style)FindResource("ErrorDatePickerStyle");
                isValid = false;
            }
            return isValid;
        }

        private void SetDefaultStyles()
        {
            tbxName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxLastName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxEmail.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxUsername.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxEmail.Style = (Style)FindResource("NormalTextBoxStyle");
            pwBxPassword.Style = (Style)FindResource("NormalPasswordBoxStyle");
            dpBirthdate.Style = (Style)FindResource("NormalDatePickerStyle");

            lbExistentEmail.Visibility = Visibility.Hidden;
            lbExistentUsername.Visibility = Visibility.Hidden;
            lbEmailError.Visibility = Visibility.Hidden;

            lbPasswordLengthInstruction.Foreground = Brushes.Red;
            lbPasswordSymbolInstruction.Foreground = Brushes.Red;
            lbPasswordCapitalLetterInstruction.Foreground = Brushes.Red;
            lbPasswordLowerLetterInstruction.Foreground = Brushes.Red;
            lbPasswordNumberInstruction.Foreground = Brushes.Red;

            ImgNameErrorDetails.Visibility = Visibility.Hidden;
            ImgLastNameErrorDetails.Visibility = Visibility.Hidden;
            ImgEmailErrorDetails.Visibility = Visibility.Hidden;
            ImgUsernameErrorDetails.Visibility = Visibility.Hidden;
            ImgPasswordErrorDetails.Visibility = Visibility.Hidden;
        }

        private void ValidatePasswordProperties()
        {
            if (pwBxPassword.Password.Trim().Length >= 12)
            {
                lbPasswordLengthInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidSymbol(pwBxPassword.Password))
            {
                lbPasswordSymbolInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidCapitalLetter(pwBxPassword.Password))
            {
                lbPasswordCapitalLetterInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidLowerLetter(pwBxPassword.Password))
            {
                lbPasswordLowerLetterInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidNumber(pwBxPassword.Password))
            {
                lbPasswordNumberInstruction.Foreground = Brushes.GreenYellow;
            }
        }

    }

}


