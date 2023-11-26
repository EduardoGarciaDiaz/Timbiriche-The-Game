using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimbiricheViews.Player;
using TimbiricheViews.Utils;
using TimbiricheViews.Views;

namespace TimbiricheViews
{

    public partial class XAMLMainWindow : Window
    {
        private ILogger _logger = LoggerManager.GetLogger();

        public XAMLMainWindow()
        {
            InitializeComponent();
        }

        private void BtnWindowClosing_Click(object sender, CancelEventArgs e)
        {
            Page currentPage = frameNavigation.Content as Page;
            if (currentPage != null)
            {
                if (currentPage is XAMLLobby)
                {
                    XAMLLobby lobby = (XAMLLobby)currentPage;
                    lobby.BtnCloseWindow_Click();
                }
            }
            PlayerSingleton.Player = null;
            LoggerManager.CloseAndFlush();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
        }
    }
}