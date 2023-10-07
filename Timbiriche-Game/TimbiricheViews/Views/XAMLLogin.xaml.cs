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
using System.Resources;

namespace TimbiricheViews.Views
{

    public partial class XAMLLogin : Page
    {
        public XAMLLogin()
        {
            InitializeComponent();
        }


        private void ChangeLanguage()
        {
            String language = ""; 
            if (lbLanguage.Content.Equals("Español"))
            {
                language = "en";
            }

            if(lbLanguage.Content.Equals("English"))
            {
                language = "es";
            }
            

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            XAMLLogin newLoginPage = new XAMLLogin();

            if (language.Equals("en"))
            {
                newLoginPage.imgUsaFlag.Visibility = Visibility.Visible;
                newLoginPage.imgMexicoFlag.Visibility = Visibility.Hidden;
            }

            if (language.Equals("es"))
            {
                newLoginPage.imgMexicoFlag.Visibility = Visibility.Visible;
                newLoginPage.imgUsaFlag.Visibility = Visibility.Hidden;
            }

            NavigationService.Navigate(newLoginPage);
        }

        private void OnClickChangeLanguage(object sender, MouseButtonEventArgs e)
        {
            ChangeLanguage();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
            //TODo: Excepcion
            bool isUserValid = userManagerClient.ValidateLoginCredentials(tbxUsername.Text, tbxPassword.Text);
            if (isUserValid)
            {
                NavigationService.Navigate(new XAMLLobby());
            }
        }

        private void MouseEnter_tbxUsername(object sender, MouseEventArgs e)
        {
            
        }

        private void MouseLeave_tbxUsername(object sender, MouseEventArgs e)
        {

        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLUserForm());

        }
    }
}
