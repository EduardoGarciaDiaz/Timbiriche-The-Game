﻿using System.Windows;

namespace TimbiricheViews.Components
{
    public partial class XAMLEmergentWindow : Window
    {
        private readonly Window _mainWindow;

        public XAMLEmergentWindow()
        {
            InitializeComponent();
        }

        public XAMLEmergentWindow(string title, string description)
        {
            InitializeComponent();

            _mainWindow = Application.Current.MainWindow;
            lbTitleEmergentWindow.Content = title;
            tbkDescriptionEmergentWindow.Text = description;

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

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}