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
    public partial class XAMLFriendRequestItemComponent : UserControl
    {
        private const string BTN_ACCEPT = "Accept";
        private const string BTN_REJECT = "Reject";
        private string _username;

        public event EventHandler<ButtonClickEventArgs> ButtonClicked;

        public XAMLFriendRequestItemComponent(string username)
        {  
            InitializeComponent();

            _username = username;
            lbUsername.Content = username;
        }

        private void ImgAcceptFriendRequest_Click(object sender, MouseButtonEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonClickEventArgs(BTN_ACCEPT, _username));
        }

        private void ImgRejectFriendRequest_Click(object sender, MouseButtonEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonClickEventArgs(BTN_REJECT, _username));
        }
    }

    public class ButtonClickEventArgs : EventArgs
    {
        public string ButtonName { get; private set; }
        public string Username { get; private set; }

        public ButtonClickEventArgs(string buttonName, string username)
        {
            ButtonName = buttonName;
            Username = username;
        }
    }
}
