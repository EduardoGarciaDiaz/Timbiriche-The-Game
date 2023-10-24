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
    /// <summary>
    /// Lógica de interacción para XAMLOneInputWindow.xaml
    /// </summary>
    public partial class XAMLOneInputWindow : Window
    {
        private Window _mainWindow;
        private string _code;
        public XAMLOneInputWindow()
        {
            InitializeComponent();
        }

        public XAMLOneInputWindow(string code)
        {
            _mainWindow = Application.Current.MainWindow;
            _code = code;
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
            double centerX = _mainWindow.Left + (_mainWindow.Width - _mainWindow.Width) / 2;
            double centerY = _mainWindow.Top + (_mainWindow.Height - _mainWindow.Height) / 2;
            this.Left = centerX;
            this.Top = centerY;
        }

        public bool validateCorrectCode()
        {
            SetDefaultStyles();
            bool isCorrectCode = false;
            if (tbxCode.Text.Trim() != _code)
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
            if (validateCorrectCode())
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
