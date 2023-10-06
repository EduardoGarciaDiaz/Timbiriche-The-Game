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

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                DateTime birthdate;
                DateTime.TryParse(dpBirthdate.Text, out birthdate);

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
                    email = tbxEmail.Text.Trim(),
                    password = pwBxPassword.Password.Trim(),
                    accountFK = newAccount
                };

                Server.UserManagerClient cliente = new Server.UserManagerClient();
                int rowsAffected = cliente.addUser(newAccount, newPlayer);
                Console.WriteLine("rows " + rowsAffected);
                if (rowsAffected > 0)
                {
                    NavigationService.GoBack();
                } 
                else
                {
                    Console.WriteLine("Error al crear una cuenta de usuario");
                }
            }

        }

        private bool ValidateFields()
        {
            SetDefaultStyles();
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

        private void SetDefaultStyles()
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


