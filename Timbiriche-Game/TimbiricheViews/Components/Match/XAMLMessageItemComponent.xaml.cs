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

namespace TimbiricheViews.Components.Match
{
    public partial class XAMLMessageItemComponent : UserControl
    {
        public XAMLMessageItemComponent(string senderUsername, string message, bool isMessageReceived)
        {          
            InitializeComponent();
            tbkSenderUsername.Text = senderUsername;
            tbkMessage.Text = message;

            if (!isMessageReceived)
            {
                imgResportMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void ImgReportMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            imgResportMessage.Visibility = Visibility.Collapsed;
            gridReportMessage.Visibility = Visibility.Visible;
        }

        private void GridReportMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            imgResportMessage.Visibility = Visibility.Visible;
            gridReportMessage.Visibility = Visibility.Collapsed;
        }

    }
}
