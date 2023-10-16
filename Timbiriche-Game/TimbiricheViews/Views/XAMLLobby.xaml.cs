using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using TimbiricheViews.Components;
using TimbiricheViews.Player;
using TimbiricheViews.Server;

namespace TimbiricheViews.Views
{

    public partial class XAMLLobby : Page, Server.IManagerOnlineUsersCallback
    {

        Server.Player playerLoggedIn = PlayerSingleton.player;

        public XAMLLobby()
        {
            InitializeComponent();
            ShowAsActiveUser();
            LoadDataPlayer();
        }

        private void LoadDataPlayer()
        {
            string initialPlayerNameLetter = playerLoggedIn.username[0].ToString();
            lbUsername.Content = playerLoggedIn.username;
            lbCoins.Content = playerLoggedIn.coins;
            lbUserFaceBox.Content = initialPlayerNameLetter;
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            Server.ManagerOnlineUsersClient client = new Server.ManagerOnlineUsersClient(context);
            client.RegisteredUserToOnlineUsers(playerLoggedIn.username);
        }

        public void NotifyUserLoggedIn(string username)
        {
            Console.WriteLine("Conected User: " + username);
            AddUserToOnlineUserList(username);
        }

        private void BtnSignOff_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.ManagerOnlineUsersClient client = new Server.ManagerOnlineUsersClient(context);
            client.UnregisteredUserToOnlineUsers(playerLoggedIn.username);

            // Commented while we do the update of the stackpanel view
            // NavigationService.Navigate(new XAMLLogin());

        }

        public void NotifyUserLoggedOut(string username)
        {
            Console.WriteLine("Disconected User: " + username);
            RemoveUserFromOnlineUserList(username);
        }

        private void AddUserToOnlineUserList(string username)
        {
            string idUserItem = "lb" + username;
            XAMLActiveUserItemControl userItem = new XAMLActiveUserItemControl(username);
            userItem.Name = idUserItem;
            stckPnlFriends.Children.Add(userItem);
        }

        private void RemoveUserFromOnlineUserList(string username)
        {
            // TODO: Delete the user disconected from the list of active friends on the stack panel
            string idLabel = "lb" + username;
            Label labelAuxiliar = stckPnlFriends.FindName(idLabel) as Label;
            stckPnlFriends.Children.Remove(labelAuxiliar);
            stckPnlFriends.UpdateLayout();
            scrlVwrFriends.UpdateLayout();
        }

        private void btnFriendsMenu_Click(object sender, RoutedEventArgs e)
        {
            gridFriendsMenu.Visibility = Visibility.Visible;
            btnFriendsMenu.Visibility = Visibility.Collapsed;
        }

        private void btnCloseFriendsMenu_Click(object sender, RoutedEventArgs e)
        {
            btnFriendsMenu.Visibility = Visibility.Visible;
            gridFriendsMenu.Visibility = Visibility.Collapsed;
        }

    }
}
