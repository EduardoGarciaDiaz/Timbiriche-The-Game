using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Player;
using TimbiricheViews.Server;

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
