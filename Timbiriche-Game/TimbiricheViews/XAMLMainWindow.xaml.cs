using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimbiricheViews.Player;
using TimbiricheViews.Utils;
using TimbiricheViews.Views;

namespace TimbiricheViews
{

    public partial class XAMLMainWindow : Window
    {
        public XAMLMainWindow()
        {
            InitializeComponent();
        }

        private void BtnWindowClosing_Click(object sender, CancelEventArgs e)
        {
            if (frameNavigation.Content is Page currentPage)
            {
                if (currentPage is XAMLLobby lobby)
                {
                    lobby.BtnCloseWindow_Click();
                }

                if (currentPage is XAMLGlobalScoreboard globalScoreboard)
                {
                    globalScoreboard.BtnCloseWindow_Click();
                }
            }

            PlayerSingleton.Player = null;
            LoggerManager.CloseAndFlush();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
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
            if (e.Key == Key.Escape)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
        }
    }
}