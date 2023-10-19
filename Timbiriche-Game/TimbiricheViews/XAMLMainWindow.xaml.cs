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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            Page currentPage = frameNavigation.Content as Page;
            if (currentPage != null)
            {
                if (currentPage is XAMLLobby)
                {
                    XAMLLobby lobby = (XAMLLobby)currentPage;
                    lobby.BtnCloseWindow_Click();
                }
            }
        }

    }

}