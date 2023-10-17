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

    public partial class XAMLLobby : Page, IManagerOnlineUsersCallback
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
            AddUserToOnlineUserList(username);
        }

        public void NotifyUserLoggedOut(string username)
        {
            RemoveUserFromOnlineUserList(username);
        }

        public void NotifyOnlineUsers(string[] onlineUsernames)
        {
            AddUsersToOnlineUsersList(onlineUsernames);
        }

        private void AddUsersToOnlineUsersList(string[] onlineUsernames)
        {
            foreach(var username in onlineUsernames)
            {
                string idUserItem = "lb" + username;
                XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
                userOnlineItem.Name = idUserItem;
                stckPnlFriends.Children.Add(userOnlineItem);
            }
        }

        private void AddUserToOnlineUserList(string username)
        {
            string idUserItem = "lb" + username;
            XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
            userOnlineItem.Name = idUserItem;
            stckPnlFriends.Children.Add(userOnlineItem);
        }

        private void RemoveUserFromOnlineUserList(string username)
        {
            string  idUserItem = "lb" + username;
            XAMLActiveUserItemControl userOnlineItemToRemove = null;
            foreach (XAMLActiveUserItemControl item in stckPnlFriends.Children)
            {
                if(item.Name == idUserItem)
                {
                    userOnlineItemToRemove = item;
                    break;
                }
            }
            if(userOnlineItemToRemove != null)
            {
                stckPnlFriends.Children.Remove(userOnlineItemToRemove);
            }
        }

        private void BtnSignOff_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.ManagerOnlineUsersClient client = new Server.ManagerOnlineUsersClient(context);
            client.UnregisteredUserToOnlineUsers(playerLoggedIn.username);

            NavigationService.Navigate(new XAMLLogin());
        }

        private void BtnFriendsMenu_Click(object sender, RoutedEventArgs e)
        {
            gridFriendsMenu.Visibility = Visibility.Visible;
            btnFriendsMenu.Visibility = Visibility.Collapsed;
            imgFriendsMenu.Visibility = Visibility.Collapsed;
            lbFriends.Visibility = Visibility.Collapsed;
        }

        private void BtnCloseFriendsMenu_Click(object sender, RoutedEventArgs e)
        {
            gridFriendsMenu.Visibility = Visibility.Collapsed;
            btnFriendsMenu.Visibility = Visibility.Visible;
            imgFriendsMenu.Visibility = Visibility.Visible;
            lbFriends.Visibility = Visibility.Visible;
        }

    }
}
