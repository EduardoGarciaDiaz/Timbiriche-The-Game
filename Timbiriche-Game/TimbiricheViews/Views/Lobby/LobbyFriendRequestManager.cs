using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using TimbiricheViews.Components;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IFriendRequestManagerCallback
    {
        private void SuscribeUserToOnlineFriendsDictionary()
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);

            try
            {
                friendRequestManagerClient.AddToOnlineFriendshipDictionary(_playerLoggedIn.Username);
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
            catch (FaultException ex)
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

        private void BtnSendRequest_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }

        private void SendRequest()
        {
            lbFriendRequestUsernameError.Visibility = Visibility.Collapsed;

            string usernamePlayerRequested = tbxUsernameSendRequest.Text.Trim();
            int idPlayer = _playerLoggedIn.IdPlayer;

            if (ValidateSendRequest(idPlayer, usernamePlayerRequested))
            {
                AddRequestFriendship(idPlayer, usernamePlayerRequested);
                SendFriendRequest(usernamePlayerRequested);

                EmergentWindows.CreateEmergentWindow(Properties.Resources.lbFriendRequest,
                    Properties.Resources.lbFriendRequestSent + " " + usernamePlayerRequested);

                tbxUsernameSendRequest.Text = string.Empty;
            }
            else
            {
                EmergentWindows.CreateEmergentWindow(Properties.Resources.lbFriendRequest,
                    Properties.Resources.tbkFriendRequestErrorDescription);
            }
        }

        private void AddRequestFriendship(int idPlayer, string usernamePlayerRequested)
        {
            Server.FriendshipManagerClient friendshipManagerClient = new Server.FriendshipManagerClient();
            try
            {
                friendshipManagerClient.AddRequestFriendship(idPlayer, usernamePlayerRequested);
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
            catch (FaultException<TimbiricheServerException> ex)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException ex)
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

        private void SendFriendRequest(string usernamePlayerRequested)
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);
            try
            {
                friendRequestManagerClient.SendFriendRequest(_playerLoggedIn.Username, usernamePlayerRequested);
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
            catch (FaultException ex)
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

        private bool ValidateSendRequest(int idPlayer, string usernamePlayerRequested)
        {
            bool isRequestValid = false;
            if (ValidationUtilities.IsValidUsername(usernamePlayerRequested))
            {
                Server.FriendshipManagerClient friendshipManagerClient = new Server.FriendshipManagerClient();
                try
                {
                    isRequestValid = friendshipManagerClient.ValidateFriendRequestSending(idPlayer, usernamePlayerRequested);
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
                catch (FaultException<TimbiricheServerException> ex)
                {
                    EmergentWindows.CreateDataBaseErrorMessageWindow();
                    NavigationService.Navigate(new XAMLLogin());
                }
                catch (FaultException ex)
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
                lbFriendRequestUsernameError.Visibility = Visibility.Visible;
            }

            return isRequestValid;
        }

        private void BtnFriends_Click(object sender, RoutedEventArgs e)
        {
            scrollViewerFriends.Visibility = Visibility.Visible;
            scrollViewerFriendsRequest.Visibility = Visibility.Collapsed;

            ChangeButtonColor(btnFriends, SELECTED_BUTTON_HEX_COLOR);
            ChangeButtonColor(btnFriendRequest, NOT_SELECTED_BUTTON_HEX_COLOR);
        }

        private void BtnFriendsRequest_Click(object sender, RoutedEventArgs e)
        {
            scrollViewerFriendsRequest.Visibility = Visibility.Visible;
            scrollViewerFriends.Visibility = Visibility.Collapsed;

            ChangeButtonColor(btnFriendRequest, SELECTED_BUTTON_HEX_COLOR);
            ChangeButtonColor(btnFriends, NOT_SELECTED_BUTTON_HEX_COLOR);

            stackPanelFriendsRequest.Children.Clear();
            string[] usernamePlayers = GetCurrentFriendRequests();
            AddUsersToFriendsRequestList(usernamePlayers);
        }

        private void ChangeButtonColor(Button btnAppareance, string hexadecimalColor)
        {
            SolidColorBrush buttonColor = Utilities.CreateColorFromHexadecimal(hexadecimalColor);
            btnAppareance.Background = buttonColor;
        }

        private string[] GetCurrentFriendRequests()
        {
            Server.FriendshipManagerClient friendshipManagerClient = new Server.FriendshipManagerClient();
            string[] usernamePlayers = null;

            try
            {
                usernamePlayers = friendshipManagerClient.GetUsernamePlayersRequesters(_playerLoggedIn.IdPlayer);
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
            catch (FaultException<TimbiricheServerException> ex)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException ex)
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

            return usernamePlayers;
        }

        private void AddUsersToFriendsRequestList(string[] usernamePlayers)
        {
            if (usernamePlayers != null)
            {
                foreach (string username in usernamePlayers)
                {
                    AddUserToFriendRequestList(username);
                }
            }
        }

        private void AddUserToFriendRequestList(string username)
        {
            XAMLFriendRequestItemComponent friendRequestItem = CreateFriendRequestItemControl(username);
            stackPanelFriendsRequest.Children.Add(friendRequestItem);
        }

        private XAMLFriendRequestItemComponent CreateFriendRequestItemControl(string username)
        {
            const string ID_ITEM = "lbRequest";
            string idUserItem = ID_ITEM + username;
            XAMLFriendRequestItemComponent friendRequestItem = new XAMLFriendRequestItemComponent(username);
            friendRequestItem.Name = idUserItem;
            friendRequestItem.ButtonClicked += FriendRequestItem_BtnClicked;

            return friendRequestItem;
        }

        private void FriendRequestItem_BtnClicked(object sender, ButtonClickEventArgs e)
        {
            const string BTN_ACCEPT = "Accept";
            const string BTN_REJECT = "Reject";

            if (e.ButtonName.Equals(BTN_ACCEPT))
            {
                AcceptFriendRequest(e.Username);
            }
            if (e.ButtonName.Equals(BTN_REJECT))
            {
                RejectFriendRequest(e.Username);
            }
        }

        private void AcceptFriendRequest(string usernameSender)
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new FriendRequestManagerClient(context);
            try
            {
                friendRequestManagerClient.AcceptFriendRequest(_playerLoggedIn.IdPlayer, _playerLoggedIn.Username, usernameSender);
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
            catch (FaultException<TimbiricheServerException> ex)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException ex)
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

        private void RejectFriendRequest(string username)
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new FriendRequestManagerClient(context);

            try
            {
                friendRequestManagerClient.RejectFriendRequest(_playerLoggedIn.IdPlayer, username);
                RemoveFriendRequestFromStackPanel(username);
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
            catch (FaultException<TimbiricheServerException> ex)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException ex)
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

        private void RemoveFriendRequestFromStackPanel(string username)
        {
            string idFriendRequestItem = "lbRequest" + username;
            XAMLFriendRequestItemComponent friendRequestItemToRemove = FindFriendRequeustItemControlById(idFriendRequestItem);

            if (friendRequestItemToRemove != null)
            {
                stackPanelFriendsRequest.Children.Remove(friendRequestItemToRemove);
            }
        }

        private XAMLFriendRequestItemComponent FindFriendRequeustItemControlById(string idFriendRequestItem)
        {
            foreach (XAMLFriendRequestItemComponent item in stackPanelFriendsRequest.Children)
            {
                if (item.Name == idFriendRequestItem)
                {
                    return item;
                }
            }

            return null;
        }

        public void NotifyNewFriendRequest(string username)
        {
            AddUserToFriendRequestList(username);
        }

        public void NotifyFriendRequestAccepted(string username)
        {
            AddUserToFriendsList(username, ONLINE_STATUS_PLAYER_HEX_COLOR);
            RemoveFriendRequestFromStackPanel(username);
        }

        public void NotifyDeletedFriend(string username)
        {
            RemoveFriendFromFriendsList(username);
        }

        private void RemoveFriendFromFriendsList(string username)
        {
            string idUserItem = "lb" + username;
            XAMLActiveUserItemControl userOnlineItemToRemove = FindActiveUserItemControlById(idUserItem);

            if (userOnlineItemToRemove != null)
            {
                stackPanelFriends.Children.Remove(userOnlineItemToRemove);
            }
        }
    }
}
