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
    public partial class XAMLGuestLobby : Page, IPlayerStylesManagerCallback
    {
        private Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private string _lobbyCode;

        public XAMLGuestLobby()
        {
            InitializeComponent();
            LoadDataPlayer();
            this.Loaded += Lobby_Loaded;
        }

        public XAMLGuestLobby(string lobbyCode)
        {
            InitializeComponent();
            _lobbyCode = lobbyCode;
            LoadDataPlayer();
            this.Loaded += Lobby_Loaded;
        }

        private void Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataPlayer();
            PrepareNotificationOfStyleUpdated();
            JoinToLobby();
        }

        private void JoinToLobby()
        {
            const int ID_DEFAULT_STYLE = 1;
            LobbyPlayer lobbyPlayer = new LobbyPlayer();
            lobbyPlayer.Username = _playerLoggedIn.Username;
            lobbyPlayer.IdStylePath = ID_DEFAULT_STYLE;

            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClient = new LobbyManagerClient(context);
            lobbyManagerClient.JoinLobby(_lobbyCode, lobbyPlayer);
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
            lbUsername.Content = _playerLoggedIn.Username;
            LoadFaceBox(lbUserFaceBox, _playerLoggedIn.IdStyleSelected, _playerLoggedIn.Username);
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

        public void BtnCloseWindow_Click()
        {
            InstanceContext context = new InstanceContext(this);
            Server.OnlineUsersManagerClient client = new Server.OnlineUsersManagerClient(context);
            client.UnregisterUserToOnlineUsers(_playerLoggedIn.Username);
            PlayerSingleton.Player = null;
        }
    }

    public partial class XAMLGuestLobby : Page, ILobbyManagerCallback
    {
        public void NotifyLobbyCreated(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            ShowSelectPlayerColorGrid();
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
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

        public void NotifyPlayerLeftLobby(String username)
        {
            String secondPlayerUsername = (String)lbSecondPlayerUsername.Content;
            String thirdPlayerUsername = (String)lbThirdPlayerUsername.Content;
            String fourthPlayerUsername = (String)lbFourthPlayerUsername.Content;

            if (username.Equals(secondPlayerUsername))
            {
                gridSecondPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(thirdPlayerUsername))
            {
                gridThirdPlayer.Visibility = Visibility.Collapsed;
            }

            if (username.Equals(fourthPlayerUsername))
            {
                gridFourthPlayer.Visibility = Visibility.Collapsed;
            }
        }

        public void NotifyHostPlayerLeftLobby()
        {
            Utils.EmergentWindows.CreateEmergentWindow(Properties.Resources.lbHostLeftLobbyTitle ,
                Properties.Resources.tbkHostLeftLobbyDescription);
            NavigationService.Navigate(new XAMLLogin());
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
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
            ShowSelectPlayerColorGrid();
        }

        public void NotifyLobbyIsFull()
        {
            EmergentWindows.CreateLobbyIsFullMessageWindow();

            NavigationService.Navigate(new XAMLLogin());
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

        private void BtnInviteToLobby_Click(object sender, RoutedEventArgs e)
        {
            EmergentWindows.CreateLobbyInvitationWindow(_lobbyCode);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient client = new LobbyManagerClient(context);
            client.ExitLobby(_lobbyCode, PlayerSingleton.Player.Username);

            NavigationService.Navigate(new XAMLLogin());
        }

        private (string, string) GetPlayerCustomization()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            string playerHexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(_playerLoggedIn.IdColorSelected);
            string playerStylePath = playerCustomizationManagerClient.GetStylePath(_playerLoggedIn.IdStyleSelected);

            return (playerHexadecimalColor, playerStylePath);
        }

        public void NotifyExpulsedFromLobby()
        {
            string title = Properties.Resources.lbExpulsedTilte;
            string message = Properties.Resources.tbkExpulsedDescription;
            EmergentWindows.CreateEmergentWindowNoModal(title, message);

            NavigationService.Navigate(new XAMLLogin());
        }
    }

    public partial class XAMLGuestLobby : Page, IPlayerColorsManagerCallback
    {
        private const string SELECTED_STROKE_COLOR_HEXADECIMAL = "#000000";
        private const int DEFAULT_SELECTED_COLOR = 0;
        private int[] _idDefaultColors = new int[] {1, 2, 3, 4};

        private void ShowSelectPlayerColorGrid()
        {
            gridSelectColor.Visibility = Visibility.Visible;
            SetMyColors();
        }

        private void SetMyColors()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            SolidColorBrush color;
            foreach (int idColor in _idDefaultColors)
            {
                string hexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(idColor);

                color = Utilities.CreateColorFromHexadecimal(hexadecimalColor);
                Rectangle colorRectangle = CreateColorBoxes(idColor, color, PlayerColorTemplate);
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
            const int LOW_LIMITD_DEFAULTS_COLOR = 0;
            const int HIGH_LIMIT_DEFAULTS_COLOR = 4;
            bool hasColor = false;
            if (idColor > LOW_LIMITD_DEFAULTS_COLOR && idColor <= HIGH_LIMIT_DEFAULTS_COLOR)
            {
                hasColor = true;
            }

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
        }   
    }
}