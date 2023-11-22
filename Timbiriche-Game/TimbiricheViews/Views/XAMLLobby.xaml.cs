using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
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
using Path = System.IO.Path;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IOnlineUsersManagerCallback, IPlayerStylesManagerCallback
    {
        private Server.Player playerLoggedIn = PlayerSingleton.Player;
        private const string PLACEHOLDER_HEX_COLOR = "#CDCDCD";
        private const string ONLINE_STATUS_PLAYER_HEX_COLOR = "#61FF00";
        private const string OFFLINE_STATUS_PLAYER_HEX_COLOR = "#FF5A5E59";

        public XAMLLobby()
        {
            InitializeComponent();
            ShowAsActiveUser();
            LoadDataPlayer();
            LoadPlayerFriends();
            this.Loaded += Lobby_Loaded;
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

        private Image CreateImageByPath(int idStyle)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            string playerStylePath = playerCustomizationManagerClient.GetStylePath(idStyle);
            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, playerStylePath);

            Image styleImage = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
            styleImage.Source = bitmapImage;

            return styleImage;
        }
        
        private void LoadDataPlayer()
        {
            lbUsername.Content = playerLoggedIn.Username;
            lbCoins.Content = playerLoggedIn.Coins;
            LoadFaceBox(lbUserFaceBox, playerLoggedIn.IdStyleSelected, playerLoggedIn.Username);
        }

        private void LoadPlayerFriends()
        {
            Server.FriendshipManagerClient friendshipManagerClient = new FriendshipManagerClient();
            string[] usernamePlayerFriends = friendshipManagerClient.GetListUsernameFriends(playerLoggedIn.IdPlayer);
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
                Image styleImage = CreateImageByPath(idStyle);
                lbFaceBox.Content = styleImage;
            }
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.RegisterUserToOnlineUsers(playerLoggedIn.IdPlayer, playerLoggedIn.Username);
        }

        public void NotifyUserLoggedIn(string username)
        {
            //AddUserToOnlineUserList(username);
            bool isOnline = true;
            ChangePlayerStatus(username, isOnline);
        }

        public void NotifyUserLoggedOut(string username)
        {
            bool isOnline = false;
            //RemoveUserFromOnlineUserList(username);
            ChangePlayerStatus(username, isOnline);
        }

        public void NotifyOnlineFriends(string[] onlineUsernames)
        {
            bool isOnline = true;
            //AddUsersToOnlineUsersList(onlineUsernames);
            ChangeStatusFriends(onlineUsernames, isOnline);
            SuscribeUserToOnlineFriendsDictionary();
        }

        private void ChangeStatusFriends(string[] onlineUsernames, bool isOnline)
        {
            foreach(string onlineUsername in onlineUsernames)
            {
                ChangePlayerStatus(onlineUsername, isOnline);
            }
        }

        private void ChangePlayerStatus(string username, bool isOnline)
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
                AddUserToFriendsList(username,  OFFLINE_STATUS_PLAYER_HEX_COLOR);
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
            friendRequestManagerClient.DeleteFriend(playerLoggedIn.IdPlayer, playerLoggedIn.Username, usernameFriendToDelete);
        }


        private void BtnSignOff_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.UnregisterUserToOnlineUsers(playerLoggedIn.Username);
            NavigationService.Navigate(new XAMLLogin());
        }

        public void BtnCloseWindow_Click()
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.UnregisterUserToOnlineUsers(playerLoggedIn.Username);
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
    }

    public partial class XAMLLobby : Page, IFriendRequestManagerCallback
    {
        private void SuscribeUserToOnlineFriendsDictionary()
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);
            friendRequestManagerClient.AddToOnlineFriendshipDictionary(playerLoggedIn.Username);
        }

        private void BtnSendRequest_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }

        private void SendRequest()
        {
            lbFriendRequestUsernameError.Visibility = Visibility.Collapsed;
            
            string usernamePlayerRequested = tbxUsernameSendRequest.Text.Trim();
            int idPlayer = playerLoggedIn.IdPlayer;

            if (ValidateSendRequest(idPlayer, usernamePlayerRequested))
            {
                Server.FriendshipManagerClient friendshipManagerClient = new Server.FriendshipManagerClient();
                friendshipManagerClient.AddRequestFriendship(idPlayer, usernamePlayerRequested);

                InstanceContext context = new InstanceContext(this);
                Server.FriendRequestManagerClient friendRequestManagerClient = new Server.FriendRequestManagerClient(context);
                friendRequestManagerClient.SendFriendRequest(playerLoggedIn.Username, usernamePlayerRequested);

                EmergentWindows.CreateEmergentWindow(Properties.Resources.lbFriendRequest,
                    Properties.Resources.lbFriendRequestSent + " " + usernamePlayerRequested);
                tbxUsernameSendRequest.Text = string.Empty;
            }
            else
            {
                EmergentWindows.CreateEmergentWindow(Properties.Resources.lbFriendRequest,
                    "No fue posible enviar la solictud de amistad, inténtelo de nuevo");
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
            const string NOT_SELECTED_BUTTON = "#FF063343";
            const string SELECTED_BUTTON = "#FF13546C";
            scrollViewerFriends.Visibility = Visibility.Visible;
            scrollViewerFriendsRequest.Visibility = Visibility.Collapsed;

            ChangeButtonColor(btnFriends, SELECTED_BUTTON);
            ChangeButtonColor(btnFriendRequest, NOT_SELECTED_BUTTON);
        }

        private void BtnFriendsRequest_Click(object sender, RoutedEventArgs e)
        {
            const string NOT_SELECTED_BUTTON = "#FF063343";
            const string SELECTED_BUTTON = "#FF13546C";
            scrollViewerFriendsRequest.Visibility = Visibility.Visible;
            scrollViewerFriends.Visibility = Visibility.Collapsed;

            ChangeButtonColor(btnFriendRequest, SELECTED_BUTTON);
            ChangeButtonColor(btnFriends, NOT_SELECTED_BUTTON);

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
            string[] usernamePlayers = friendshipManagerClient.GetUsernamePlayersRequesters(playerLoggedIn.IdPlayer);
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
            friendRequestManagerClient.AcceptFriendRequest(playerLoggedIn.IdPlayer, playerLoggedIn.Username, usernameSender);
        }

        private void RejectFriendRequest(string username)
        {
            InstanceContext context = new InstanceContext(this);
            Server.FriendRequestManagerClient friendRequestManagerClient = new FriendRequestManagerClient(context);
            friendRequestManagerClient.RejectFriendRequest(playerLoggedIn.IdPlayer, username);
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

        public void NotifyLobbyCreated(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControl.Visibility = Visibility.Visible;

            ShowSelectPlayerColorGrid();
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
            const int ONE_PLAYER_IN_LOBBY = 1;
            const int TWO_PLAYER_IN_LOBBY = 2;
            const int THREE_PLAYER_IN_LOBBY = 3;

            if (numOfPlayersInLobby == ONE_PLAYER_IN_LOBBY)
            {
                lbSecondPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if (numOfPlayersInLobby == TWO_PLAYER_IN_LOBBY)
            {
                lbThirdPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbThirdPlayerUsername, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if (numOfPlayersInLobby == THREE_PLAYER_IN_LOBBY)
            {
                lbFourthPlayerUsername.Content = lobbyPlayer.Username;
                LoadFaceBox(lbFourthPlayerUsername, lobbyPlayer.IdStylePath, lobbyPlayer.Username);
                gridFourthPlayer.Visibility = Visibility.Visible;
            }

        }

        public void NotifyPlayerLeftLobby()
        {
            throw new NotImplementedException();
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
                LoadFaceBox(lbSecondPlayerFaceBox, lobbyPlayers[SECOND_PLAYER_ID].IdStylePath, lobbyPlayers[0].Username);
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > THIRD_PLAYER_ID)
            {
                lbThirdPlayerUsername.Content = lobbyPlayers[THIRD_PLAYER_ID].Username;
                LoadFaceBox(lbThirdPlayerFaceBox, lobbyPlayers[THIRD_PLAYER_ID].IdStylePath, lobbyPlayers[1].Username);
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > FOURTH_PLAYER_ID)
            {
                lbFourthPlayerUsername.Content = lobbyPlayers[FOURTH_PLAYER_ID].Username;
                LoadFaceBox(lbFourthPlayerFaceBox, lobbyPlayers[FOURTH_PLAYER_ID].IdStylePath, lobbyPlayers[2].Username);
                gridFourthPlayer.Visibility = Visibility.Visible;
            }

            gridCodeDialog.Visibility = Visibility.Collapsed;

            ShowSelectPlayerColorGrid();
        }

        public void NotifyLobbyIsFull()
        {
            string title = "Lobby lleno";
            string message = "El lobby al que estas intentando entrar esta lleno.";
            EmergentWindows.CreateEmergentWindow(title, message);
        }

        public void NotifyLobbyDoesNotExist()
        {
            string title = "Lobby no encontrado";
            string message = "El lobby al que estas intentando entrar no existe.";
            EmergentWindows.CreateEmergentWindow(title, message);
        }

        public void NotifyStartOfMatch()
        {
            (string, string) playerCustomization = GetPlayerCustomization();
            NavigationService.Navigate(new XAMLGameBoard(_lobbyCode, playerCustomization.Item1, playerCustomization.Item2));
        }

        private void BtnCreateMatch_Click(object sender, RoutedEventArgs e)
        {
            const int MATCH_DURATION_IN_MINUTES = 1;
            const int TURN_DURATION_IN_MINUTES = 1;

            LobbyInformation lobbyInformation = new LobbyInformation();
            lobbyInformation.TurnDurationInMinutes = TURN_DURATION_IN_MINUTES;
            lobbyInformation.MatchDurationInMinutes = MATCH_DURATION_IN_MINUTES;

            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = playerLoggedIn.IdStyleSelected;

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.CreateLobby(lobbyInformation, lobbyPlayer);
        }

        private void BtnJoinByCode_Click(object sender, RoutedEventArgs e)
        {
            gridCodeDialog.Visibility = Visibility.Visible;
        }

        private void BtnJoin_Click(object sender, RoutedEventArgs e)
        {
            string lobbyCode = tbxJoinByCode.Text.Trim();

            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = playerLoggedIn.IdStyleSelected;

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.JoinLobby(lobbyCode, lobbyPlayer);
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

        private (string, string) GetPlayerCustomization()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            string playerHexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(playerLoggedIn.IdColorSelected);
            string playerStylePath = playerCustomizationManagerClient.GetStylePath(playerLoggedIn.IdStyleSelected);

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
            _myColors = playerCustomizationManagerClient.GetMyColors(playerLoggedIn.IdPlayer);

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
            lobbyPlayer.Username = playerLoggedIn.Username;
            lobbyPlayer.IdHexadecimalColor = playerLoggedIn.IdColorSelected;
            lobbyPlayer.IdStylePath = playerLoggedIn.IdStyleSelected;
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
            playerCustomizationManagerClient.SelectMyColor(playerLoggedIn.IdPlayer, idColor);
            playerLoggedIn.IdColorSelected = idColor;

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
            } else if (lbThirdPlayerUsername.Content.Equals(username))
            {
                rectangleThirdPlayerColor.Fill = colorPlayer;
            }
            else if (lbFourthPlayerUsername.Content.Equals(username))
            {
                rectangleFourthPlayerColor.Fill = colorPlayer;
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
            if(playerLoggedIn.IdColorSelected > DEFAULT_SELECTED_COLOR)
            {
                isColorSelected = true;
            }
            return isColorSelected;
        }

        private bool VerifyPlayerHasColor(int idColor)
        {
            int idPlayer = playerLoggedIn.IdPlayer;
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

        }   
    }
}