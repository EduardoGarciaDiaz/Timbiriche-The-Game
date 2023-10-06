using System;
using System.Collections.Generic;
using System.Linq;
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
using TimbiricheViews.Servidor;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    /// <summary>
    /// Lógica de interacción para XAMLUserForm.xaml
    /// </summary>
    public partial class XAMLUserForm : Window
    {
        public XAMLUserForm(String language)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(language);
            InitializeComponent();
        }


        public XAMLUserForm()
        {
            InitializeComponent();
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                DateTime birthdate;
                DateTime.TryParse(dpBirthdate.Text, out birthdate);

                Account newAccount = new Account()
                {
                    Name = tbxName.Text.Trim(),
                    LastName = tbxLastName.Text.Trim(),
                    Surname = tbxSurname.Text.Trim(),
                    Birthdate = birthdate
                };

                Player newPlayer = new Player()
                {
                    Username = tbxUsername.Text.Trim(),
                    Email = tbxEmail.Text.Trim(),
                    Password = pwBxPassword.Password.Trim(),
                    AccountFK = newAccount
                };

                Servidor.UserManagerClient cliente = new Servidor.UserManagerClient();
                _ = cliente.AddUser(newAccount, newPlayer);
            }



        }


        private bool ValidateFields()
        {
            setDefaultStyles();
            bool isValid = true;

            if (!Utilities.IsValidPersonalInformation(tbxName))
            {
                tbxName.Style = (Style)FindResource("ErrorTextBoxStyle");
                lbNameError.Content = "UPPSS ERROR";

                isValid = false;
            }
            if (!Utilities.IsValidPersonalInformation(tbxLastName))
            {
                tbxLastName.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            if (!Utilities.IsValidEmail(tbxEmail))
            {
                tbxEmail.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            if (!Utilities.IsValidPersonalInformation(tbxUsername))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            if (!Utilities.IsValidPassword(pwBxPassword))
            {
                tbxEmail.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }
            return isValid;
        }

        private void setDefaultStyles()
        {
            tbxName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxLastName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxEmail.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxUsername.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxEmail.Style = (Style)FindResource("NormalTextBoxStyle");
            lbNameError.Content = "";
            lbLastNameError.Content = "";
            lbBirthdateError.Content = "";
            lbEmailError.Content = "";
            lbUsernameError.Content = "";
            lbPasswordError.Content = "";
        }

    }

}
