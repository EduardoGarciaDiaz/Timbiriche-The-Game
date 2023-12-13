using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TimbiricheViews.Components;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IOnlineUsersManagerCallback
    {
        private const string ONLINE_STATUS_PLAYER_HEX_COLOR = "#61FF00";
        private const string OFFLINE_STATUS_PLAYER_HEX_COLOR = "#FF5A5E59";
        private void ConfigureRematch(bool isHost)
        {
            if (isHost)
            {
                InstanceContext context = new InstanceContext(this);
                LobbyManagerClient client = new LobbyManagerClient(context);

                try
                {
                    client.JoinLobbyAsHost(_lobbyCode);
                }
                catch (EndpointNotFoundException ex)
                {
                    EmergentWindows.CreateConnectionFailedMessageWindow();
                    HandlerException.HandleErrorException(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    EmergentWindows.CreateTimeOutMessageWindow();
                    HandlerException.HandleErrorException(ex, NavigationService);
                }
                catch (FaultException)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                    NavigationService.Navigate(new XAMLLogin());
                }
                catch (CommunicationException ex)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
                    HandlerException.HandleErrorException(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    EmergentWindows.CreateUnexpectedErrorMessageWindow();
                    HandlerException.HandleFatalException(ex, NavigationService);
                }
            }
            else
            {
                JoinLobbyByLobbyCode(_lobbyCode);
            }
        }

        private void LoadPlayerFriends()
        {
            FriendshipManagerClient friendshipManagerClient = new FriendshipManagerClient();

            try
            {
                string[] usernamePlayerFriends = friendshipManagerClient.GetListUsernameFriends(_playerLoggedIn.IdPlayer);
                AddUsersToFriendsList(usernamePlayerFriends);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            OnlineUsersManagerClient client = new OnlineUsersManagerClient(context);

            try
            {
                client.RegisterUserToOnlineUsers(_playerLoggedIn.IdPlayer, _playerLoggedIn.Username);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }
        }

        public void NotifyUserLoggedIn(string username)
        {
            bool isOnline = true;
            ChangeStatusPlayer(username, isOnline);
        }

        public void NotifyUserLoggedOut(string username)
        {
            bool isOnline = false;
            ChangeStatusPlayer(username, isOnline);
        }

        public void NotifyOnlineFriends(string[] onlineUsernames)
        {
            bool isOnline = true;

            ChangeStatusFriends(onlineUsernames, isOnline);
            SuscribeUserToOnlineFriendsDictionary();
        }

        private void ChangeStatusFriends(string[] onlineUsernames, bool isOnline)
        {
            foreach (string onlineUsername in onlineUsernames)
            {
                ChangeStatusPlayer(onlineUsername, isOnline);
            }
        }

        private void ChangeStatusPlayer(string username, bool isOnline)
        {
            string idLabel = "lb";
            string idUserItem = idLabel + username;
            XAMLActiveUserItemControl userOnlineItem = FindActiveUserItemControlById(idUserItem);

            if (userOnlineItem != null)
            {
                SolidColorBrush statusPlayerColor;

                if (isOnline)
                {
                    statusPlayerColor = Utilities.CreateColorFromHexadecimal(ONLINE_STATUS_PLAYER_HEX_COLOR);
                }
                else
                {
                    statusPlayerColor = Utilities.CreateColorFromHexadecimal(OFFLINE_STATUS_PLAYER_HEX_COLOR);
                }

                userOnlineItem.rectangleStatusPlayer.Fill = statusPlayerColor;
            }
        }

        private XAMLActiveUserItemControl FindActiveUserItemControlById(string idUserItem)
        {
            foreach (XAMLActiveUserItemControl item in stackPanelFriends.Children)
            {
                if (item.Name == idUserItem)
                {
                    return item;
                }
            }

            return null;
        }

        private void AddUsersToFriendsList(string[] onlineUsernames)
        {
            foreach (var username in onlineUsernames)
            {
                AddUserToFriendsList(username, OFFLINE_STATUS_PLAYER_HEX_COLOR);
            }
        }

        private void AddUserToFriendsList(string username, string connectionStatusPlayer)
        {
            XAMLActiveUserItemControl userOnlineItem = CreateActiveUserItemControl(username, connectionStatusPlayer);
            stackPanelFriends.Children.Add(userOnlineItem);
        }

        private XAMLActiveUserItemControl CreateActiveUserItemControl(string username, string haxadecimalColor)
        {
            string idItem = "lb";
            string idUserItem = idItem + username;
            XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
            userOnlineItem.Name = idUserItem;
            userOnlineItem.ButtonClicked += UserOnlineItem_BtnDeleteFriendClicked;
            SolidColorBrush onlinePlayerColor = Utilities.CreateColorFromHexadecimal(haxadecimalColor);
            userOnlineItem.rectangleStatusPlayer.Fill = onlinePlayerColor;

            return userOnlineItem;
        }

        private void UserOnlineItem_BtnDeleteFriendClicked(object sender, ButtonClickEventArgs e)
        {
            string btnDeleteFriend = "DeleteFriend";

            if (e.ButtonName.Equals(btnDeleteFriend))
            {
                DeleteFriend(e.Username);
            }
        }

        private void DeleteFriend(string usernameFriendToDelete)
        {
            InstanceContext context = new InstanceContext(this);
            FriendRequestManagerClient friendRequestManagerClient = new FriendRequestManagerClient(context);

            try
            {
                friendRequestManagerClient.DeleteFriend(_playerLoggedIn.IdPlayer, _playerLoggedIn.Username, usernameFriendToDelete);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }
        }

        private void BtnSignOff_Click(object sender, RoutedEventArgs e)
        {
            ExitGameFromLobby();
            NavigationService.Navigate(new XAMLLogin());
        }

        public void BtnCloseWindow_Click()
        {
            ExitGameFromLobby();
        }

        private void ExitGameFromLobby()
        {
            InstanceContext context = new InstanceContext(this);
            OnlineUsersManagerClient client = new OnlineUsersManagerClient(context);

            try
            {
                client.UnregisterUserToOnlineUsers(_playerLoggedIn.Username);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }
            finally
            {
                PlayerSingleton.Player = null;
            }
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

        private void BtnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLMyProfile());
        }

        private void BtnShop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLShop());
        }

        private void ImgCloseGridCodeDialog_Click(object sender, MouseButtonEventArgs e)
        {
            if (gridCodeDialog.Visibility == Visibility.Visible)
            {
                gridCodeDialog.Visibility = Visibility.Collapsed;
            }
        }

        private void TbxJoinByCode_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbxJoinByCode.Text == (string)tbxJoinByCode.Tag)
            {
                tbxJoinByCode.Text = string.Empty;
                tbxJoinByCode.Foreground = Brushes.Black;
            }
        }

        private void TbxJoinByCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxJoinByCode.Text))
            {
                tbxJoinByCode.Text = (string)tbxJoinByCode.Tag;
                SolidColorBrush placeholderBrush = Utilities.CreateColorFromHexadecimal(PLACEHOLDER_HEX_COLOR);
                tbxJoinByCode.Foreground = placeholderBrush;
            }
        }

        private void BtnScoreboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLGlobalScoreboard());
        }
    }
}
