using System;
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

                Player newPlayer = new Player()
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
                    Server.UserManagerClient cliente = new Server.UserManagerClient();
                    int rowsAffected = cliente.AddUser(newPlayer);
                    if (rowsAffected > 0)
                    {
                        //TODO: Message successful registration
                        NavigationService.GoBack();
                    }
                    else
                    {
                        //TODO: Internationalizate
                        System.Windows.MessageBox.Show("No se pudo registrar el usuario, inténtalo de nuevo",
                            "Error al registrar usuario", MessageBoxButton.OK);
                    }
                }
            }
        }

        public bool ValidateUniqueIdentifier(Player newPlayer)
        {
            bool existUserIdentifier = false;
            Server.UserManagerClient cliente = new Server.UserManagerClient();
            if (cliente.ValidateUniqueIdentifierUser(newPlayer.email))
            {
                existUserIdentifier = true;
                lbExistentEmail.Visibility = Visibility.Visible;
            }
            if (cliente.ValidateUniqueIdentifierUser(newPlayer.username))
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
                isValid = false;
            }
            if (!Utilities.IsValidPersonalInformation(tbxLastName.Text))
            {
                tbxLastName.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            if (!Utilities.IsValidEmail(tbxEmail.Text))
            {
                tbxEmail.Style = (Style)FindResource("ErrorTextBoxStyle");
                lbEmailError.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidUsername(tbxUsername.Text))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            if (!Utilities.IsValidPassword(pwBxPassword.Password))
            {
                pwBxPassword.Style = (Style)FindResource("ErrorPasswordBoxStyle");
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


