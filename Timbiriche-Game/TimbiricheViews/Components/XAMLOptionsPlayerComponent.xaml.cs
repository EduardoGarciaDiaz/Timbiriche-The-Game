using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimbiricheViews.Components
{
    public partial class XAMLOptionsPlayerComponent : UserControl
    {
        private const string BTN_REPORT = "Report";
        private const string BTN_EXPULSE = "Expulse";
        private readonly bool _isHost;
        private readonly string _username;

        public event EventHandler<ButtonClickEventArgs> ButtonClicked;

        public XAMLOptionsPlayerComponent(string username)
        {  
            InitializeComponent();

            _username = username;
            lbUsername.Content = username;
        }

        public XAMLOptionsPlayerComponent(bool isHost, string username)
        {
            InitializeComponent();
            _isHost = isHost;
            _username = username;
            lbUsername.Content = username;

            ConfigurePlayerOptions();
        }

        private void ConfigurePlayerOptions()
        {
            ValidateIsHost();
        }

        private void ValidateIsHost()
        {
            if (_isHost)
            {
                btnExpulse.IsEnabled = true;
                btnExpulse.Visibility = Visibility.Visible;
            }
        }

        private void ImgCloseOptions_Click(object sender, MouseButtonEventArgs e)
        {
            gridOptionsPlayer.Visibility = Visibility.Collapsed;
            gridOptionsPlayer = null;
        }

        private void BtnExpulsePlayer_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonClickEventArgs(BTN_EXPULSE, _username));
        }

        private void BtnReportPlayer_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonClickEventArgs(BTN_REPORT, _username));
        }
    }
}
