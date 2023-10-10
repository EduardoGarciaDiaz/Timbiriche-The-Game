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

namespace TimbiricheViews.Views
{
    /// <summary>
    /// Lógica de interacción para EmergentWindow.xaml
    /// </summary>
    public partial class EmergentWindow : Window
    {
        public EmergentWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Normal;
            this.ResizeMode = ResizeMode.NoResize;
        }

        public EmergentWindow(string title, string description)
        {
            InitializeComponent();
            lbTitleEmergentWindow.Content = title;
            tbkDescriptionEmergentWindow.Text = description;
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}