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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Components;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IOnlineUsersManagerCallback
    {
        private Server.Player playerLoggedIn = PlayerSingleton.player;

        public XAMLLobby()
        {
            InitializeComponent();
            ShowAsActiveUser();
            LoadDataPlayer();
        }

        private void LoadDataPlayer()
        {
            string initialPlayerNameLetter = playerLoggedIn.Username[0].ToString();
            lbUsername.Content = playerLoggedIn.Username;
            lbCoins.Content = playerLoggedIn.Coins;
            lbUserFaceBox.Content = initialPlayerNameLetter;
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.RegisterUserToOnlineUsers(playerLoggedIn.Username);
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
            foreach (var username in onlineUsernames)
            {
                string idUserItem = "lb" + username;
                XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
                userOnlineItem.Name = idUserItem;
                stackPanelFriends.Children.Add(userOnlineItem);
            }
        }

        private void AddUserToOnlineUserList(string username)
        {
            string idUserItem = "lb" + username;
            XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
            userOnlineItem.Name = idUserItem;
            stackPanelFriends.Children.Add(userOnlineItem);
        }

        private void RemoveUserFromOnlineUserList(string username)
        {
            string idUserItem = "lb" + username;
            XAMLActiveUserItemControl userOnlineItemToRemove = null;
            foreach (XAMLActiveUserItemControl item in stackPanelFriends.Children)
            {
                if (item.Name == idUserItem)
                {
                    userOnlineItemToRemove = item;
                    break;
                }
            }
            if (userOnlineItemToRemove != null)
            {
                stackPanelFriends.Children.Remove(userOnlineItemToRemove);
            }
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
    }

    public partial class XAMLLobby : Page, ILobbyManagerCallback
    {
        public void NotifyLobbyCreated()
        {
            gridMatchCreation.Visibility = Visibility.Collapsed;
            gridMatchControl.Visibility = Visibility.Visible;
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
            if(numOfPlayersInLobby == 1)
            {
                lbSecondPlayerUsername.Content = lobbyPlayer.Username;
                lbSecondPlayerFaceBox.Content = lobbyPlayer.Username[0].ToString();
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if(numOfPlayersInLobby == 2)
            {
                lbThirdPlayerUsername.Content = lobbyPlayer.Username;
                lbThirdPlayerUsername.Content = lobbyPlayer.Username[0].ToString();
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if(numOfPlayersInLobby == 3)
            {
                lbFourthPlayerUsername.Content = lobbyPlayer.Username;
                lbFourthPlayerUsername.Content = lobbyPlayer.Username[0].ToString();
                gridFourthPlayer.Visibility = Visibility.Visible;
            }
        }

        public void NotifyPlayerLeftLobby()
        {
            throw new NotImplementedException();
        }

        public void NotifyPlayersInLobby(LobbyPlayer[] lobbyPlayers)
        {
            int numPlayersInLobby = lobbyPlayers.Length;

            if (numPlayersInLobby > 0)
            {
                lbSecondPlayerUsername.Content = lobbyPlayers[0].Username;
                lbSecondPlayerFaceBox.Content = lobbyPlayers[0].Username[0].ToString();
                gridSecondPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > 1)
            {
                lbThirdPlayerUsername.Content = lobbyPlayers[1].Username;
                lbThirdPlayerFaceBox.Content = lobbyPlayers[1].Username[0].ToString();
                gridThirdPlayer.Visibility = Visibility.Visible;
            }

            if (numPlayersInLobby > 2)
            {
                lbFourthPlayerUsername.Content = lobbyPlayers[2].Username;
                lbFourthPlayerFaceBox.Content = lobbyPlayers[2].Username[0].ToString();
                gridFourthPlayer.Visibility = Visibility.Visible;
            }

            gridCodeDialog.Visibility = Visibility.Collapsed;
        }

        public void NotifyLobbyIsFull()
        {
            string title = "Lobby lleno";
            string message = "El lobby al que estas intentando entrar esta lleno.";
            Utilities.CreateEmergentWindow(title, message);
        }

        public void NotifyLobbyDoesNotExist()
        {
            string title = "Lobby no encontrado";
            string message = "El lobby al que estas intentando entrar no existe.";
            Utilities.CreateEmergentWindow(title, message);
        }

        private void BtnCreateMatch_Click(object sender, RoutedEventArgs e)
        {
            ShowSelectPlayerColorGrid();

            LobbyInformation lobbyInformation = new LobbyInformation();
            lobbyInformation.MatchDurationInMinutes = 5;

            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = playerLoggedIn.Username;

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

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.JoinLobby(lobbyCode, lobbyPlayer);

            ShowSelectPlayerColorGrid();
        }

        private void BtnStartMatch_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new XAMLGameBoard());
        }

    }

    public partial class XAMLLobby : Page
    {
        private PlayerColor[] _myColors = null;
        private string[] _hexadecimalColors = null;

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
        }

        private Rectangle CreateColorBoxes(int id, SolidColorBrush color, Rectangle rectangleTemplate )
        {
            Rectangle colorRectangle = XamlReader.Parse(XamlWriter.Save(rectangleTemplate)) as Rectangle;
            colorRectangle.Name = "colorRectangle_" + id;
            colorRectangle.Fill = color;
            colorRectangle.MouseLeftButtonDown += RectangleColor_Click;
            colorRectangle.IsEnabled = true;
            colorRectangle.Visibility = Visibility.Visible;
            return colorRectangle;
        }

        private void RectangleColor_Click(object sender, RoutedEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            SelectColor(clickedRectangle);
        }

        private void SelectColor(Rectangle rectangleSelected)
        {
            SolidColorBrush blackColor = Utilities.CreateColorFromHexadecimal("#000000");
            rectangleSelected.Stroke = blackColor;
            foreach(Rectangle colorRectangle in stackPanelColors.Children)
            {
                if (colorRectangle.Name != rectangleSelected.Name) 
                {
                    colorRectangle.Stroke = null;
                }
            }
        }

        private void ImgClose_Click(object sender, RoutedEventArgs e)
        {
            gridSelectColor.Visibility = Visibility.Collapsed;
        }


    }
}