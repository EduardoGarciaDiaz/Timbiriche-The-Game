using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Components
{
    public partial class XAMLBeginnerCodeVerification : Window
    {
        private readonly Window _mainWindow;
        private readonly string _username;

        public XAMLBeginnerCodeVerification()
        {
            InitializeComponent();

            _mainWindow = Application.Current.MainWindow;
            InitializeComponent();
            ConfigureEmergentWindow();
        }

        public XAMLBeginnerCodeVerification(string username)
        {
            InitializeComponent();

            _username = username;
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

            
            bool isTokenValid = TokenValidation(codeEntered);

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

        private bool TokenValidation(string code)
        {
            bool isTokenValid = false;
            Server.EmailVerificationManagerClient emailVerificationManagerClient = new Server.EmailVerificationManagerClient();

            try
            {
                isTokenValid = emailVerificationManagerClient.VerifyEmailToken(code, _username);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleComponentErrorException(ex);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleComponentErrorException(ex);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleComponentErrorException(ex);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleComponentFatalException(ex);
            }

            return isTokenValid;
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