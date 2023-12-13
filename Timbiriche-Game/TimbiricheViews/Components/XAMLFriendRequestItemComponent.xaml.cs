using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimbiricheViews.Components
{
    public partial class XAMLFriendRequestItemComponent : UserControl
    {
        private const string BTN_ACCEPT = "Accept";
        private const string BTN_REJECT = "Reject";
        private readonly string _username;

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
}
