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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TimbiricheViews.Components;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;
using Path = System.IO.Path;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page, IOnlineUsersManagerCallback
    {
        private Server.Player playerLoggedIn = PlayerSingleton.Player;

        public XAMLLobby()
        {
            InitializeComponent();
            ShowAsActiveUser();
            LoadDataPlayer();
            LoadFaceBox();
            this.Loaded += Lobby_Loaded;

        }

        private void Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataPlayer();
            LoadFaceBox();
        }

        private void LoadDataPlayer()
        {
            lbUsername.Content = playerLoggedIn.Username;
            lbCoins.Content = playerLoggedIn.Coins;
        }

        private void LoadFaceBox()
        {
            const int ID_DEFAULT_STYLE = 0;
            int idStyle = playerLoggedIn.IdStyleSelected;
            if (idStyle == ID_DEFAULT_STYLE)
            {
                lbUserFaceBox.Content = playerLoggedIn.Username[ID_DEFAULT_STYLE].ToString();
            }
            else
            {
                Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

                string stylePath = playerCustomizationManagerClient.GetStylePath(idStyle);
                string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stylePath);

                Image styleImage = new Image();
                BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
                styleImage.Source = bitmapImage;

                lbUserFaceBox.Content = styleImage;
            }
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

        private void ChangeColorOfOtherPlayer(string rectangleName)
        {
            string hexadecimalColorPlayer = "#ffffff";
            SolidColorBrush colorPlayer = Utilities.CreateColorFromHexadecimal(hexadecimalColorPlayer);
            Rectangle rectangleFinded = LogicalTreeHelper.FindLogicalNode(this, rectangleName)
                as Rectangle;
            rectangleFinded.Fill = colorPlayer;
        }

        public void NotifyPlayerLeftLobby()
        {
            throw new NotImplementedException();
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
            _lobbyCode = lobbyCode;
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

            ShowSelectPlayerColorGrid();
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

        public void NotifyStartOfMatch()
        {
             NavigationService.Navigate(new XAMLGameBoard(_lobbyCode));
        }

        private void BtnCreateMatch_Click(object sender, RoutedEventArgs e)
        {
            LobbyInformation lobbyInformation = new LobbyInformation();
            lobbyInformation.MatchDurationInMinutes = 5;
            lobbyInformation.TurnDurationInMinutes = 1;

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
        }

        private void BtnStartMatch_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.StartMatch(_lobbyCode);

            NavigationService.Navigate(new XAMLGameBoard(_lobbyCode));
        }

    }

    public partial class XAMLLobby : Page, IPlayerColorsManagerCallback
    {
        private PlayerColor[] _myColors = null;
        private const string SELECTED_STROKE_COLOR_HEXADECIMAL = "#000000";

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
            playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, playerLoggedIn.IdColorSelected);
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

        private void RectangleColor_Click(object sender, RoutedEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            SelectColor(clickedRectangle);
        }

        private void SelectColor(Rectangle rectangleSelected)
        {
            int idColor = GetIdColorByRectangle(rectangleSelected);

            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);
            playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, playerLoggedIn.IdColorSelected);

            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            playerCustomizationManagerClient.SelectMyColor(playerLoggedIn.IdPlayer, idColor);
            playerLoggedIn.IdColorSelected = idColor;

            playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, idColor);
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

        private void StablishOcuppiedColors(int[] occupiedColors)
        {
            bool isOcuppied = true;
            foreach (int idColor in occupiedColors)
            {
                HandleColorOccupation(idColor, isOcuppied);
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

        private void ImgClose_Click(object sender, RoutedEventArgs e)
        {
            if (ValidatePlayerSelectColor())
            {
                gridSelectColor.Visibility = Visibility.Collapsed;
            }
        }

        private bool ValidatePlayerSelectColor()
        {
            bool isColorSelected = false;
            const int COLOR_NOT_SELECTED_ID = 0;
            if(playerLoggedIn.IdColorSelected > COLOR_NOT_SELECTED_ID)
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

        public void NotifyColorSelected(int idSelectedColor)
        {
            const int DEFAULT_SELECTED_COLOR = 0;
            bool isOcuppied = true;

            if (idSelectedColor == DEFAULT_SELECTED_COLOR)
            {
                rectangleFirstPlayerColor.Fill = PlayerColorTemplate.Fill;
            }
            else
            {
                HandleColorOccupation(idSelectedColor, isOcuppied);
            }
        }

        public void NotifyColorUnselected(int idUnselectedColor)
        {
            bool isOcuppied = false;
            HandleColorOccupation(idUnselectedColor, isOcuppied);
        }

        public void NotifyOccupiedColors(int[] ocuppedColors)
        {
            StablishOcuppiedColors(ocuppedColors);
        }

    }
}