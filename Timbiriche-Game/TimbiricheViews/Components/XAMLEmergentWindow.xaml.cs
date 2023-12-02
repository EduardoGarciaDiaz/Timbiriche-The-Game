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
using System.Windows.Shapes;

namespace TimbiricheViews.Components
{
    public partial class XAMLEmergentWindow : Window
    {
        private Window _mainWindow;

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