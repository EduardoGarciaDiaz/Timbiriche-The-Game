using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimbiricheViews.Components
{

    public partial class XAMLActiveUserItemControl : UserControl
    {
        private const string BTN_DELETE_FRIEND = "DeleteFriend";
        private readonly string _username;

        public event EventHandler<ButtonClickEventArgs> ButtonClicked;

        public XAMLActiveUserItemControl(string username)
        {  
            InitializeComponent();

            _username = username;
            lbUsername.Content = _username;
        }

        private void ImgOptionPlayer_Click(object sender, MouseButtonEventArgs e)
        {
            if (gridOptionsPlayer.Visibility == Visibility.Visible)
            {
                gridOptionsPlayer.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridOptionsPlayer.Visibility= Visibility.Visible;
            }
        }

        private void BtnDeleteFriend_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonClickEventArgs(BTN_DELETE_FRIEND, _username));
        }
    }
}
