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
            String language = "es";
            if (lbLanguage.Content.Equals("Español"))
            {
                language = "en";
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            MainWindow newWindow = new MainWindow();
            if (language.Equals("en"))
            {
                Frame mainWindowFrame = newWindow.frameNavigation;
                Page loginPage = mainWindowFrame.Content as Page;

                if(loginPage != null)
                {
                    Label lbLanguage = loginPage.FindName("lbLanguage") as Label;
                    if(lbLanguage != null)
                    {
                        lbLanguage.Content = "English";
                    }
                }

            }
            Application.Current.MainWindow.Close();
            newWindow.Show();
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
