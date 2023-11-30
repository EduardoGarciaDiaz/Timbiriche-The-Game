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

namespace TimbiricheViews.Components
{
    public partial class XAMLBeginnerCodeVerification : Window
    {
        private Window _mainWindow;

        public XAMLBeginnerCodeVerification()
        {
            InitializeComponent();

            _mainWindow = Application.Current.MainWindow;
            InitializeComponent();
            ConfigureEmergentWindow();
        }

        private void ConfigureEmergentWindow()
        {
            this.Owner = _mainWindow;

            SetSizeWindow();
            SetCenterWindow();
        }

        private void SetSizeWindow()
        {
            this.Width = _mainWindow.Width;
            this.Height = _mainWindow.Height;
        }

        private void SetCenterWindow()
        {
            double centerX = _mainWindow.Left + (_mainWindow.Width - this.Width) / 2;
            double centerY = _mainWindow.Top + (_mainWindow.Height - this.Height) / 2;
            this.Left = centerX;
            this.Top = centerY;
        }

        public bool ValidateCorrectCode()
        {
            SetDefaultStyles();

            bool isCorrectCode = false;
            string codeEntered = tbxCode.Text.Trim().ToUpper();

            Server.EmailVerificationManagerClient emailVerificationManagerClient = 
                new Server.EmailVerificationManagerClient();
            bool isTokenValid = emailVerificationManagerClient.VerifyEmailToken(codeEntered);

            if (!isTokenValid)
            {
                lbCodeError.Visibility = Visibility.Visible;
            }
            else
            {
                isCorrectCode = true;
            }

            return isCorrectCode;
        }

        private void SetDefaultStyles()
        {
            lbCodeError.Visibility = Visibility.Collapsed;
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateCorrectCode())
            {
                DialogResult = true;
            }
        }

        private void ImgClose_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}