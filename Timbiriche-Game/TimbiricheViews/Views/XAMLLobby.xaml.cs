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
using TimbiricheViews.Server;

namespace TimbiricheViews.Views
{
    /// <summary>
    /// Lógica de interacción para XAMLLobby.xaml
    /// </summary>
    public partial class XAMLLobby : Page, Server.IManagerOnlineUsersCallback
    {
        private Player player;
        public XAMLLobby()
        {
            InitializeComponent();
        }

        public XAMLLobby(Player playerLogged)
        {
            InitializeComponent();
            player = playerLogged;
            ShowAsActiveUser();
            LoadDataPlayer();
        }

        private void LoadDataPlayer()
        {
            string initialPlayerNameLetter = player.username[0].ToString();
            lbUsername.Content = player.username;
            lbCoins.Content = player.coins;
            lbUserFaceBox.Content = initialPlayerNameLetter;
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            Server.ManagerOnlineUsersClient client = new Server.ManagerOnlineUsersClient(context);
            client.RegisteredUserToOnlineUsers(player.username);
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
            client.UnregisteredUserToOnlineUsers(player.username);

            // Commented while we do the update of the stackpanel view
            // NavigationService.Navigate(new XAMLLogin());

        }

        public void NotifyUserLoggedOut(string username)
        {
            Console.WriteLine("Disconected User: " + username);
            RemoveUserToOnlineUserList(username);
        }


        private void AddUserToOnlineUserList(string username)
        {
            string idLabel = "lb" + username;
            Label lbActiveUser = new Label
            {
                Content = username,
                Name = idLabel
            };
            stckPnlFriends.Children.Add(lbActiveUser);
        }

        private void RemoveUserToOnlineUserList(string username)
        {
            // TODO: Delete the user disconected from the list of active friends on the stack panel
            string idLabel = "lb" + username;
            Label labelAuxiliar = stckPnlFriends.FindName(idLabel) as Label;
            stckPnlFriends.Children.Remove(labelAuxiliar);
            stckPnlFriends.UpdateLayout();
            scrlVwrFriends.UpdateLayout();
        }
    }
}
