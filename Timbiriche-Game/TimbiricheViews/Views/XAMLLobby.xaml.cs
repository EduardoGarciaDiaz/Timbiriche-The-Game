using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TimbiricheViews.Components;
using TimbiricheViews.Components.Lobby;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IOnlineUsersManagerCallback, IPlayerStylesManagerCallback
    {
        private Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string ONLINE_STATUS_PLAYER_HEX_COLOR = "#61FF00";
        private const string OFFLINE_STATUS_PLAYER_HEX_COLOR = "#FF5A5E59";
        private const string NOT_SELECTED_BUTTON_HEX_COLOR = "#FF13546C";
        private const string SELECTED_BUTTON_HEX_COLOR = "#FF063343";

        public XAMLLobby()
        {
            InitializeComponent();
            ShowAsActiveUser();
            LoadDataPlayer();
            LoadPlayerFriends();

            bool isRematch = false;
            RestartSelectedColor(isRematch);

            this.Loaded += Lobby_Loaded;
        }

        public XAMLLobby(string lobbyCode, bool isHost)
        {
            InitializeComponent();

            _lobbyCode = lobbyCode;

            LoadDataPlayer();
            LoadPlayerFriends();

            bool isRematch = true;
            RestartSelectedColor(isRematch);
            ConfigureRematch(isHost);
        }

        private void ConfigureRematch(bool isHost)
        {

            if (isHost)
            {
                InstanceContext context = new InstanceContext(this);
                LobbyManagerClient client = new LobbyManagerClient(context);
                client.JoinLobbyAsHost(_lobbyCode);
            }
            else
            {
                JoinLobbyByLobbyCode(_lobbyCode);
            }
        }

        private void RestartSelectedColor(bool isRematch)
        {
            int defaultColor = 0;
            PlayerSingleton.Player.IdColorSelected = defaultColor;

            if (isRematch)
            {
                InstanceContext context = new InstanceContext(this);
                PlayerColorsManagerClient client = new PlayerColorsManagerClient(context);
                client.UnsubscribeColorToColorsSelected(_lobbyCode, CreateLobbyPlayer());
            }
        }

        private void Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataPlayer();
            PrepareNotificationOfStyleUpdated();
        }

        private void PrepareNotificationOfStyleUpdated()
        {
            bool isLoaded = true;
            LobbyPlayer lobbyPlayer = CreateLobbyPlayer();
            InformUpdateStyleForPlayers(lobbyPlayer, isLoaded);
        }

        private void InformUpdateStyleForPlayers(LobbyPlayer lobbyPlayer, bool isLoaded)
        {
            InstanceContext context = new InstanceContext(this);
            Server.PlayerStylesManagerClient playerStylesManagerClient = new Server.PlayerStylesManagerClient(context);
            if (!isLoaded)
            {
                playerStylesManagerClient.AddStyleCallbackToLobbiesList(_lobbyCode, lobbyPlayer);
            }
            else if (_lobbyCode != null)
            {
                playerStylesManagerClient.ChooseStyle(_lobbyCode, lobbyPlayer);
            }
        }

        public void NotifyStyleSelected(LobbyPlayer lobbyPlayer)
        {
            string username = lobbyPlayer.Username;
            int idStyle = lobbyPlayer.IdStylePath;

            if (lbSecondPlayerUsername.Content.Equals(username))
            {
                LoadFaceBox(lbSecondPlayerFaceBox, idStyle, username);
            }
            else if (lbThirdPlayerUsername.Content.Equals(username))
            {
                LoadFaceBox(lbThirdPlayerUsername, idStyle, username);
            }
            else if (lbFourthPlayerUsername.Content.Equals(username))
            {
                LoadFaceBox(lbFourthPlayerUsername, idStyle, username);
            }
        }

        private void LoadDataPlayer()
        {
            lbUsername.Content = _playerLoggedIn.Username;
            lbCoins.Content = _playerLoggedIn.Coins;
            LoadFaceBox(lbUserFaceBox, _playerLoggedIn.IdStyleSelected, _playerLoggedIn.Username);
        }

        private void LoadPlayerFriends()
        {
            Server.FriendshipManagerClient friendshipManagerClient = new FriendshipManagerClient();
            string[] usernamePlayerFriends = friendshipManagerClient.GetListUsernameFriends(_playerLoggedIn.IdPlayer);
            AddUsersToFriendsList(usernamePlayerFriends);
        }

        private void LoadFaceBox(Label lbFaceBox, int idStyle, string username)
        {
            const int ID_DEFAULT_STYLE = 1;
            const int INDEX_FIRST_LETTER = 0;

            if (idStyle == ID_DEFAULT_STYLE)
            {
                lbFaceBox.Content = username[INDEX_FIRST_LETTER].ToString();
            }
            else
            {
                string stylePath = GetPathByIdStyle(idStyle);
                Image styleImage = Utilities.CreateImageByPath(stylePath);
                lbFaceBox.Content = styleImage;
            }
        }

        private string GetPathByIdStyle(int idStyle)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            string playerStylePath = playerCustomizationManagerClient.GetStylePath(idStyle);
            return playerStylePath;
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.RegisterUserToOnlineUsers(_playerLoggedIn.IdPlayer, _playerLoggedIn.Username);
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
            string idUserItem = "lb" + username;
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
            const string ID_ITEM = "lb";
            string idUserItem = ID_ITEM + username;
            XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
            userOnlineItem.Name = idUserItem;
            userOnlineItem.ButtonClicked += UserOnlineItem_BtnDeleteFriendClicked;
            SolidColorBrush onlinePlayerColor = Utilities.CreateColorFromHexadecimal(haxadecimalColor);
            userOnlineItem.rectangleStatusPlayer.Fill = onlinePlayerColor;
            return userOnlineItem;
        }

        private void UserOnlineItem_BtnDeleteFriendClicked(object sender, ButtonClickEventArgs e)
        {
            const string BTN_DELETE_FRIEND = "DeleteFriend";
            if (e.ButtonName.Equals(BTN_DELETE_FRIEND))
            {
                DeleteFriend(e.Username);
            }
        }

        private void DeleteFriend(string usernameFriendToDelete)
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);
            friendRequestManagerClient.DeleteFriend(_playerLoggedIn.IdPlayer, _playerLoggedIn.Username, usernameFriendToDelete);
        }


        private void BtnSignOff_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.UnregisterUserToOnlineUsers(_playerLoggedIn.Username);
            PlayerSingleton.Player = null;
            NavigationService.Navigate(new XAMLLogin());
        }

        public void BtnCloseWindow_Click()
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.UnregisterUserToOnlineUsers(_playerLoggedIn.Username);
            PlayerSingleton.Player = null;
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

    public partial class XAMLLobby : Page, IBanManagerCallback
    {
        private void RegisterToBansNotifications(string lobbyCode)
        {
            InstanceContext context = new InstanceContext(this);
            Server.BanManagerClient banManagerClient = new Server.BanManagerClient(context);
            banManagerClient.RegisterToBansNotifications(lobbyCode, _playerLoggedIn.Username);
        }

        private void LbSecondPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (gridOptionsSecondPlayer.Visibility == Visibility.Collapsed)
            {
                string secondPlayerUsername = (string)lbSecondPlayerUsername.Content;
                XAMLOptionsPlayerComponent optionsPlayerComponent = ConfigureOptionsPlayerComponent(secondPlayerUsername);

                gridOptionsSecondPlayer.Children.Add(optionsPlayerComponent);
                gridOptionsSecondPlayer.Visibility = Visibility.Visible;
            } 
            else
            {
                gridOptionsSecondPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }
        }

        private void LbThirdPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (gridOptionsThirdPlayer.Visibility == Visibility.Collapsed)
            {
                gridOptionsThirdPlayer.Children.Clear();
                string thirdPlayerUsername = (string)lbThirdPlayerUsername.Content;
                XAMLOptionsPlayerComponent optionsPlayerComponent = ConfigureOptionsPlayerComponent(thirdPlayerUsername);

                gridOptionsThirdPlayer.Children.Add(optionsPlayerComponent);
                gridOptionsThirdPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                gridOptionsThirdPlayer.Children.Clear();
                gridOptionsThirdPlayer.Visibility = Visibility.Collapsed;
            }
        }

        private void LbFourthPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (gridOptionsFourthPlayer.Visibility == Visibility.Collapsed)
            {
                gridOptionsFourthPlayer.Children.Clear();
                string fourthPlayerUsername = (string)lbFourthPlayerUsername.Content;
                XAMLOptionsPlayerComponent optionsPlayerComponent = ConfigureOptionsPlayerComponent(fourthPlayerUsername);

                gridOptionsFourthPlayer.Children.Add(optionsPlayerComponent);
                gridOptionsFourthPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                gridOptionsFourthPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }
        }

        private XAMLOptionsPlayerComponent ConfigureOptionsPlayerComponent(string username)
        {
            bool isHost = true;
            XAMLOptionsPlayerComponent optionsPlayerComponent;
           
            optionsPlayerComponent = CreateOptionsPlayerComponent(isHost, username);
            
            return optionsPlayerComponent;
        }

        private XAMLOptionsPlayerComponent CreateOptionsPlayerComponent(bool isHost, string username)
        {
            XAMLOptionsPlayerComponent optionsPlayerComponent = new XAMLOptionsPlayerComponent(isHost, username);
            optionsPlayerComponent.ButtonClicked += BtnOptionPlayer_Click;

            return optionsPlayerComponent;
        }

        private void BtnOptionPlayer_Click(object sender, ButtonClickEventArgs e)
        {
            const string BTN_REPORT = "Report";
            const string BTN_EXPULSE = "Expulse";
            if (e.ButtonName.Equals(BTN_REPORT))
            {
                ReportPlayer(e.Username);
            }
            if (e.ButtonName.Equals(BTN_EXPULSE))
            {
                ExpulsePlayer(e.Username);
            }
        }

        private void ReportPlayer(string username)
        {
            InstanceContext context = new InstanceContext(this);
            BanManagerClient banManagerClient = new BanManagerClient(context);
            UserManagerClient userManagerClient = new UserManagerClient();

            int idPlayerReporter = PlayerSingleton.Player.IdPlayer;
            int idPlayerReported = userManagerClient.GetIdPlayerByUsername(username);

            if (idPlayerReported > 0)
            {
                banManagerClient.ReportPlayer(_lobbyCode, idPlayerReported, idPlayerReporter);
            } 
            else
            {
                ShowCantExpulsePlayerMessage();
            }
        }

        private void ShowCantExpulsePlayerMessage()
        {
            string title = Properties.Resources.lbCantExpulsePlayerTitle;
            string description = Properties.Resources.tbkCantExpulsePlayerDescription; 

            EmergentWindows.CreateEmergentWindowNoModal(title, description);
        }

        private void ExpulsePlayer(string username)
        {
            //ExpulsePlayerFromLobby(username);
            _ = ExpulsePlayerFromLobbyAsync(username);
        }

        public void NotifyReportCompleted()
        {
            EmergentWindows.CreateSuccesfulReportMessageWindow();
        }

        public void NotifyPlayerAlreadyReported()
        {
            EmergentWindows.CreateReportedPlayerMessageWindow();
        }

        public void NotifyPlayerBanned(int idPlayerBanned)
        {
            ExitBanned(idPlayerBanned);
            EmergentWindows.CreateBannedPlayerMessageWindow();
        }

        private void ExitBanned(int idPlayerBanned)
        {
            ExitCurrentLobby(PlayerSingleton.Player.Username);
            ReestablishSelectedColor();
            NavigationService.Navigate(new XAMLBan(idPlayerBanned));
        }
    }

    public partial class XAMLLobby : Page, IFriendRequestManagerCallback
    {
        private void SuscribeUserToOnlineFriendsDictionary()
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);
            friendRequestManagerClient.AddToOnlineFriendshipDictionary(_playerLoggedIn.Username);
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
                Server.FriendshipManagerClient friendshipManagerClient = new Server.FriendshipManagerClient();
                friendshipManagerClient.AddRequestFriendship(idPlayer, usernamePlayerRequested);

                InstanceContext context = new InstanceContext(this);
                Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);
                friendRequestManagerClient.SendFriendRequest(_playerLoggedIn.Username, usernamePlayerRequested);

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

        private bool ValidateSendRequest(int idPlayer, string usernamePlayerRequested)
        {
            bool isRequestValid = false;
            if (ValidationUtilities.IsValidUsername(usernamePlayerRequested))
            {
                Server.FriendshipManagerClient friendshipManagerClient = new Server.FriendshipManagerClient();
                isRequestValid = friendshipManagerClient.ValidateFriendRequestSending(idPlayer, usernamePlayerRequested);
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
            string[] usernamePlayers = friendshipManagerClient.GetUsernamePlayersRequesters(_playerLoggedIn.IdPlayer);
            if (usernamePlayers != null)
            {
                return usernamePlayers;
            }
            return null;
        }

        private void AddUsersToFriendsRequestList(string[] usernamePlayers)
        {
            foreach(string username in usernamePlayers)
            {
                AddUserToFriendRequestList(username);
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
            friendRequestManagerClient.AcceptFriendRequest(_playerLoggedIn.IdPlayer, _playerLoggedIn.Username, usernameSender);
        }

        private void RejectFriendRequest(string username)
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new FriendRequestManagerClient(context);
            friendRequestManagerClient.RejectFriendRequest(_playerLoggedIn.IdPlayer, username);
            RemoveFriendRequestFromStackPanel(username);
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

    public partial class XAMLLobby : Page, ILobbyManagerCallback
    {
        private string _lobbyCode;
        private int _numberOfPlayersInLobby = 1;

        public void NotifyLobbyCreated(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControl.Visibility = Visibility.Visible;

            ShowSelectPlayerColorGrid();
            ValidateStartOfMatch();  
        }

        private void ValidateStartOfMatch()
        {
            // TODO: VALIDATE ALL PLAYERS HAVE SELECTED COLOR
            const int INITIAL_NUMBER_OF_PLAYERS = 1;
            if (_numberOfPlayersInLobby > INITIAL_NUMBER_OF_PLAYERS)
            {
                btnStartMatch.IsEnabled = true;
            }
            else
            {
                btnStartMatch.IsEnabled = false;
            }
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
            _numberOfPlayersInLobby = ++numOfPlayersInLobby;
            ValidateStartOfMatch();

            if (gridSecondPlayer.Visibility == Visibility.Collapsed)
            {
                lbSecondPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
                return;
            }

            if (gridThirdPlayer.Visibility == Visibility.Collapsed)
            {
                lbThirdPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbThirdPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
                return;
            }

            if (gridFourthPlayer.Visibility == Visibility.Collapsed)
            {
                lbFourthPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbFourthPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridFourthPlayer.Visibility = Visibility.Visible;
                return;
            }
        }

        public void NotifyPlayerLeftLobby(string username)
        {
            string secondPlayerUsername = (string) lbSecondPlayerUsername.Content;
            string thirdPlayerUsername = (string)lbThirdPlayerUsername.Content;
            string fourthPlayerUsername = (string)lbFourthPlayerUsername.Content;

            if (username.Equals(secondPlayerUsername))
            {
                gridSecondPlayer.Visibility = Visibility.Collapsed;
                gridOptionsSecondPlayer.Children.Clear();
                gridOptionsSecondPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(thirdPlayerUsername))
            {
                gridThirdPlayer.Visibility = Visibility.Collapsed;
                gridOptionsThirdPlayer.Children.Clear();
                gridOptionsThirdPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(fourthPlayerUsername))
            {
                gridFourthPlayer.Visibility = Visibility.Collapsed;
                gridOptionsFourthPlayer.Children.Clear();
                gridOptionsFourthPlayer.Visibility = Visibility.Collapsed;
            }

            _numberOfPlayersInLobby--;
            ValidateStartOfMatch();
        }

        public void NotifyHostPlayerLeftLobby()
        {
            EmergentWindows.CreateHostLeftLobbyMessageWindow();

            NavigationService.Navigate(new XAMLLobby());
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControlNotLeadPlayer.Visibility = Visibility.Visible;

            _lobbyCode = lobbyCode;
            int numPlayersInLobby = lobbyPlayers.Length;
            const int SECOND_PLAYER_ID = 0;
            const int THIRD_PLAYER_ID = 1;
            const int FOURTH_PLAYER_ID = 2;


            if (numPlayersInLobby > SECOND_PLAYER_ID)
            {
                lbSecondPlayerUsername.Content = lobbyPlayers[SECOND_PLAYER_ID].Username;
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayers[SECOND_PLAYER_ID].IdStylePath, lobbyPlayers[SECOND_PLAYER_ID].Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > THIRD_PLAYER_ID)
            {
                lbThirdPlayerUsername.Content = lobbyPlayers[THIRD_PLAYER_ID].Username;
                LoadFaceBox(lbThirdPlayerFaceBox, lobbyPlayers[THIRD_PLAYER_ID].IdStylePath, lobbyPlayers[THIRD_PLAYER_ID].Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > FOURTH_PLAYER_ID)
            {
                lbFourthPlayerUsername.Content = lobbyPlayers[FOURTH_PLAYER_ID].Username;
                LoadFaceBox(lbFourthPlayerFaceBox, lobbyPlayers[FOURTH_PLAYER_ID].IdStylePath, lobbyPlayers[FOURTH_PLAYER_ID].Username);
                gridFourthPlayer.Visibility = Visibility.Visible;
            }

            gridCodeDialog.Visibility = Visibility.Collapsed;

            ShowSelectPlayerColorGrid();
        }

        public void NotifyLobbyIsFull()
        {
            EmergentWindows.CreateLobbyIsFullMessageWindow();
        }

        public void NotifyLobbyDoesNotExist()
        {
            EmergentWindows.CreateLobbyNotFoundMessageWindow();
        }

        public void NotifyStartOfMatch()
        {
            (string, string) playerCustomization = GetPlayerCustomization();
            NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
        }

        private void JoinLobbyByLobbyCode(String lobbyCode)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = _playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = _playerLoggedIn.IdStyleSelected;

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.JoinLobby(lobbyCode, lobbyPlayer);
        }

        public void NotifyExpulsedFromLobby()
        {
            string title = Properties.Resources.lbExpulsedTilte; ;
            string message = Properties.Resources.tbkExpulsedDescription; ;
            EmergentWindows.CreateEmergentWindowNoModal(title, message);

            NavigationService.Navigate(new XAMLLobby());
        }

        private void BtnCreateMatch_Click(object sender, RoutedEventArgs e)
        { 
            ConfigureMatch();
        }

        private void BtnJoinByCode_Click(object sender, RoutedEventArgs e)
        {
            gridCodeDialog.Visibility = Visibility.Visible;
        }

        private void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            string lobbyCode = tbxJoinByCode.Text.Trim();

            JoinLobbyByLobbyCode(lobbyCode);
        }

        private void BtnStartMatch_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.StartMatch(_lobbyCode);

            (string, string) playerCustomization = GetPlayerCustomization();

            NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
        }

        private void BtnInviteToLobby_Click(object sender, RoutedEventArgs e)
        {
            EmergentWindows.CreateLobbyInvitationWindow(_lobbyCode);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            ExitToLobby();
        }

        private void ExitToLobby()
        {
            ExitCurrentLobby(PlayerSingleton.Player.Username);
            ReestablishSelectedColor();

            NavigationService.Navigate(new XAMLLobby());
        }

        private void ExitCurrentLobby(string username)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClient = new LobbyManagerClient(context);
            lobbyManagerClient.ExitLobby(_lobbyCode, username);
        }

        private void ReestablishSelectedColor()
        {
            int defaultColor = 0;

            InstanceContext context = new InstanceContext(this);
            PlayerColorsManagerClient playerColorsManagerClient = new PlayerColorsManagerClient(context);
            playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, CreateLobbyPlayer());

            PlayerSingleton.Player.IdColorSelected = defaultColor;
        }

        public async Task ExpulsePlayerFromLobbyAsync(string username)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClientExpulse = new LobbyManagerClient(context);
            await lobbyManagerClientExpulse.ExpulsePlayerFromLobbyAsync(_lobbyCode, username);
        }

        private (string, string) GetPlayerCustomization()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            string playerHexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(_playerLoggedIn.IdColorSelected);
            string playerStylePath = playerCustomizationManagerClient.GetStylePath(_playerLoggedIn.IdStyleSelected);

            return (playerHexadecimalColor, playerStylePath);
        }
    }

    public partial class XAMLLobby : Page, IPlayerColorsManagerCallback
    {
        private PlayerColor[] _myColors = null;
        private const string SELECTED_STROKE_COLOR_HEXADECIMAL = "#000000";
        private const int DEFAULT_SELECTED_COLOR = 0;

        private void ShowSelectPlayerColorGrid()
        {
            gridSelectColor.Visibility = Visibility.Visible;
            GetMyColors();
        }

        private void GetMyColors()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            _myColors = playerCustomizationManagerClient.GetMyColors(_playerLoggedIn.IdPlayer);

            if (_myColors != null)
            {
                SetMyColors();
            }
            else
            {
                Console.WriteLine("The player doesn't have colors related");
            }
        }

        private void SetMyColors()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            SolidColorBrush color;
            foreach (PlayerColor playerColor in _myColors)
            {
                string hexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(playerColor.IdColor);

                color = Utilities.CreateColorFromHexadecimal(hexadecimalColor);
                Rectangle colorRectangle = CreateColorBoxes(playerColor.IdColor, color, PlayerColorTemplate);
                stackPanelColors.Children.Add(colorRectangle);
            }

            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);
            playerColorsManagerClient.SubscribeColorToColorsSelected(_lobbyCode);

            LobbyPlayer lobbyPlayer = CreateLobbyPlayer();
            playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, lobbyPlayer);
        }

        private Rectangle CreateColorBoxes(int idColor, SolidColorBrush color, Rectangle rectangleTemplate )
        {
            Rectangle colorRectangle = XamlReader.Parse(XamlWriter.Save(rectangleTemplate)) as Rectangle;
            colorRectangle.Name = "colorRectangle" + "_" + idColor;
            colorRectangle.Fill = color;
            colorRectangle.MouseLeftButtonDown += RectangleColor_Click;
            colorRectangle.IsEnabled = true;
            colorRectangle.Visibility = Visibility.Visible;
            return colorRectangle;
        }

        private LobbyPlayer CreateLobbyPlayer()
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = _playerLoggedIn.Username;
            lobbyPlayer.IdHexadecimalColor = _playerLoggedIn.IdColorSelected;
            lobbyPlayer.IdStylePath = _playerLoggedIn.IdStyleSelected;
            return lobbyPlayer;
        }

        private void RectangleColor_Click(object sender, RoutedEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            SelectColor(clickedRectangle);
        }

        private void SelectColor(Rectangle rectangleSelected)
        {
            int idColor = GetIdColorByRectangle(rectangleSelected);

            LobbyPlayer lobbyPlayer2 = CreateLobbyPlayer();

            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);
            playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, lobbyPlayer2);

            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            playerCustomizationManagerClient.SelectMyColor(_playerLoggedIn.IdPlayer, idColor);
            _playerLoggedIn.IdColorSelected = idColor;

            LobbyPlayer lobbyPlayer = CreateLobbyPlayer();            
            playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, lobbyPlayer);
            MarkAsSelectedColor(rectangleSelected);
        }

        private int GetIdColorByRectangle(Rectangle rectangleSelected)
        {
            const char SPLIT_SYMBOL = '_';
            const int INDEX_ID_COLOR_PART = 1;
            string rectangleName = rectangleSelected.Name.ToString();
            string[] nameParts = rectangleName.Split(SPLIT_SYMBOL);
            int idColor = int.Parse(nameParts[INDEX_ID_COLOR_PART]);
            return idColor;
        }

        private void MarkAsSelectedColor(Rectangle rectangleSelected)
        {
            SolidColorBrush blackColor = Utilities.CreateColorFromHexadecimal(SELECTED_STROKE_COLOR_HEXADECIMAL);
            rectangleSelected.Stroke = blackColor;
            rectangleSelected.StrokeThickness = 2;
            ClearOtherColorSelections(rectangleSelected);
            UpdatePlayerColor(rectangleSelected);
        }

        private void StablishOcuppiedColors(LobbyPlayer[] lobbyPlayers)
        {
            bool isOcuppied = true;
            int idColor;
            foreach (LobbyPlayer lobbyPlayer in lobbyPlayers)
            {
                if (lobbyPlayer.IdHexadecimalColor != DEFAULT_SELECTED_COLOR)
                {
                    idColor = lobbyPlayer.IdHexadecimalColor;
                    HandleColorOccupation(idColor, isOcuppied);
                    ChangeColorOfOtherPlayer(lobbyPlayer);
                }
            }
        }

        private void HandleColorOccupation(int idColor, bool isOccupied)
        {
            const int DEFAULT_SELECTED_COLOR = 0;
            if (idColor != DEFAULT_SELECTED_COLOR && VerifyPlayerHasColor(idColor))
            {
                string idRectangle = "colorRectangle" + "_" + idColor;
                if (isOccupied)
                {
                    MarkAsOccupiedColor(idRectangle);
                }
                else
                {
                    MarkAsUnoccupiedColor(idRectangle);
                }
            }
        }

        private void ChangeColorOfOtherPlayer(LobbyPlayer lobbyPlayer)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            string username = lobbyPlayer.Username;
            int idColor = lobbyPlayer.IdHexadecimalColor;
            string selectedHexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(idColor);
            SolidColorBrush colorPlayer = Utilities.CreateColorFromHexadecimal(selectedHexadecimalColor);

            if (lbSecondPlayerUsername.Content.Equals(username))
            {
                rectangleSecondPlayerColor.Fill = colorPlayer;
                rectangleSecondPlayerUsernameColor.Fill = colorPlayer;
            } 
            else if (lbThirdPlayerUsername.Content.Equals(username))
            {
                rectangleThirdPlayerColor.Fill = colorPlayer;
                rectangleThirdPlayerUsernameColor.Fill = colorPlayer;
            }
            else if (lbFourthPlayerUsername.Content.Equals(username))
            {
                rectangleFourthPlayerColor.Fill = colorPlayer;
                rectangleFourthPlayerUsernameColor.Fill = colorPlayer;
            }
        }

        private void MarkAsOccupiedColor(string idRectangle)
        {
            const string OCCUPIED_COLOR_HEXADECIMAL = "#7F000000";
            SolidColorBrush difuminedColor = Utilities.CreateColorFromHexadecimal(OCCUPIED_COLOR_HEXADECIMAL);

            Rectangle rectangleFinded = LogicalTreeHelper.FindLogicalNode(stackPanelColors, idRectangle)
                as Rectangle;
            rectangleFinded.OpacityMask = difuminedColor;
            rectangleFinded.IsEnabled = false;
        }

        private void MarkAsUnoccupiedColor(string idRectangle)
        {
            Rectangle rectangleFinded = LogicalTreeHelper.FindLogicalNode(stackPanelColors, idRectangle)
                as Rectangle;
            rectangleFinded.OpacityMask = null;
            rectangleFinded.IsEnabled = true;
        }

        private void ClearOtherColorSelections(Rectangle rectangleSelected)
        {
            foreach (Rectangle colorRectangle in stackPanelColors.Children)
            {
                if (colorRectangle.Name != rectangleSelected.Name)
                {
                    colorRectangle.Stroke = null;
                }
            }
        }

        private void UpdatePlayerColor(Rectangle rectangleSelected)
        {
            rectangleFirstPlayerColor.Fill = rectangleSelected.Fill;
            rectangleFirstPlayerUsernameColor.Fill = rectangleSelected.Fill;
        }

        private void ImgCloseGridColorSelection_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatePlayerSelectColor())
            {
                gridSelectColor.Visibility = Visibility.Collapsed;
            }
        }

        private bool ValidatePlayerSelectColor()
        {
            bool isColorSelected = false;
            if(_playerLoggedIn.IdColorSelected > DEFAULT_SELECTED_COLOR)
            {
                isColorSelected = true;
            }
            return isColorSelected;
        }

        private bool VerifyPlayerHasColor(int idColor)
        {
            int idPlayer = _playerLoggedIn.IdPlayer;
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            bool hasColor = playerCustomizationManagerClient.CheckColorForPlayer(idPlayer, idColor);
            return hasColor;
        }

        public void NotifyColorSelected(LobbyPlayer lobbyPlayer)
        {
            int idSelectedColor = lobbyPlayer.IdHexadecimalColor;
            bool isOcuppied = true;

            if (idSelectedColor == DEFAULT_SELECTED_COLOR)
            {
                rectangleFirstPlayerColor.Fill = PlayerColorTemplate.Fill;
                rectangleFirstPlayerUsernameColor.Fill = PlayerColorTemplate.Fill;
            }
            else
            {
                HandleColorOccupation(idSelectedColor, isOcuppied);
                ChangeColorOfOtherPlayer(lobbyPlayer);
            }
        }

        public void NotifyColorUnselected(int idUnselectedColor)
        {
            bool isOcuppied = false;
            HandleColorOccupation(idUnselectedColor, isOcuppied);
        }

        public void NotifyOccupiedColors(LobbyPlayer[] ocuppedColors)
        {
            StablishOcuppiedColors(ocuppedColors);
            InformUpdateStyleForPlayers(CreateLobbyPlayer(), false);
            RegisterToBansNotifications(_lobbyCode);
        }
    }

    public partial class XAMLLobby : Page
    {
        private const float DEFAULT_MATCH_DURATION_IN_MINUTES = 5;
        private const float MAXIMIUN_MATCH_DURATION_IN_MINUTES = 20;
        private const float MINIMIUN_MATCH_DURATION_IN_MINUTES = 2;

        private float _matchDurationInMinutes = DEFAULT_MATCH_DURATION_IN_MINUTES;

        private void ConfigureMatch()
        {
            ShowMatchSettingsGrid();
        }

        private void ShowMatchSettingsGrid()
        {
            lbMatchTime.Content = _matchDurationInMinutes;
            gridMatchSettings.Visibility = Visibility.Visible;
        }

        private void BtnAcceptSettings_Click(object sender, RoutedEventArgs e)
        {
            _matchDurationInMinutes = (float)lbMatchTime.Content;
            gridMatchSettings.Visibility = Visibility.Collapsed;

            ConfigureMatchSettings();
        }

        private void ConfigureMatchSettings()
        {
            LobbyInformation lobbyInformation = ConfigureLobbyInformation();
            LobbyPlayer lobbyPlayer = ConfigureLobbyPlayer();

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.CreateLobby(lobbyInformation, lobbyPlayer);
        }

        private LobbyInformation ConfigureLobbyInformation()
        {
            const float TURN_DURATION_IN_MINUTES = 0.5F;

            LobbyInformation lobbyInformation = new LobbyInformation();
            lobbyInformation.TurnDurationInMinutes = TURN_DURATION_IN_MINUTES;
            lobbyInformation.MatchDurationInMinutes = _matchDurationInMinutes;

            return lobbyInformation;
        }

        private LobbyPlayer ConfigureLobbyPlayer()
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = _playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = _playerLoggedIn.IdStyleSelected;

            return lobbyPlayer;
        }

        private void BtnIncrementTime_Click(object sender, RoutedEventArgs e)
        {
            IncrementMatchDuration();
        }

        private void IncrementMatchDuration()
        {
            if (_matchDurationInMinutes < MAXIMIUN_MATCH_DURATION_IN_MINUTES)
            {
                _matchDurationInMinutes++;
                lbMatchTime.Content = _matchDurationInMinutes;
            }
        }

        private void BtnDecrementTime_Click(object sender, RoutedEventArgs e)
        {
            DecrementMatchDuration();
        }

        private void DecrementMatchDuration()
        {
            if (_matchDurationInMinutes > MINIMIUN_MATCH_DURATION_IN_MINUTES)
            {
                _matchDurationInMinutes--;
                lbMatchTime.Content = _matchDurationInMinutes;
            }
        }

        private void ImgCloseMatchSettings_Click(object sender, RoutedEventArgs e)
        {
            gridMatchSettings.Visibility = Visibility.Collapsed;
        }
    }
}