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
    public partial class XAMLPasswordReset : Page
    {
        private string _email;
        public XAMLPasswordReset()
        {
            InitializeComponent();
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

        }
    }
}
