using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace TimbiricheViews.Views
{
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

            try
            {
                _myColors = playerCustomizationManagerClient.GetMyColors(_playerLoggedIn.IdPlayer);
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
            SolidColorBrush color;

            foreach (PlayerColor playerColor in _myColors)
            {
                string hexadecimalColor = GetHexadecimalColor(playerColor.IdColor);

                color = Utilities.CreateColorFromHexadecimal(hexadecimalColor);
                Rectangle colorRectangle = CreateColorBoxes(playerColor.IdColor, color, PlayerColorTemplate);
                stackPanelColors.Children.Add(colorRectangle);
            }

            SubscribeColorToColorsSelected();

        }

        private string GetHexadecimalColor(int idColor)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            string hexadecimalColor = null;

            try
            {
                hexadecimalColor = playerCustomizationManagerClient.GetHexadecimalColors(idColor);
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

            return hexadecimalColor;
        }

        private void SubscribeColorToColorsSelected()
        {
            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);

            try
            {
                playerColorsManagerClient.SubscribeColorToColorsSelected(_lobbyCode);
                LobbyPlayer lobbyPlayer = CreateLobbyPlayer();
                playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, lobbyPlayer);
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

        private Rectangle CreateColorBoxes(int idColor, SolidColorBrush color, Rectangle rectangleTemplate)
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

            UnsusbcribeColorToColorsSelected();
            SelectPlayerColor(idColor);
            RenewSubscriptionToColorsSelected();

            MarkAsSelectedColor(rectangleSelected);
        }

        private void UnsusbcribeColorToColorsSelected()
        {
            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);
            LobbyPlayer lobbyPlayer = CreateLobbyPlayer();

            try
            {
                playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, lobbyPlayer);
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

        private void SelectPlayerColor(int idColor)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            try
            {
                playerCustomizationManagerClient.SelectMyColor(_playerLoggedIn.IdPlayer, idColor);
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

            _playerLoggedIn.IdColorSelected = idColor;
        }

        private void RenewSubscriptionToColorsSelected()
        {
            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);
            LobbyPlayer lobbyPlayer = CreateLobbyPlayer();

            try
            {
                playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, lobbyPlayer);
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
            string username = lobbyPlayer.Username;
            int idColor = lobbyPlayer.IdHexadecimalColor;
            string selectedHexadecimalColor = GetHexadecimalColor(idColor);
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

            if (_playerLoggedIn.IdColorSelected > DEFAULT_SELECTED_COLOR)
            {
                isColorSelected = true;
            }

            return isColorSelected;
        }

        private bool VerifyPlayerHasColor(int idColor)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            int idPlayer = _playerLoggedIn.IdPlayer;
            bool hasColor = false;

            try
            {
                hasColor = playerCustomizationManagerClient.CheckColorForPlayer(idPlayer, idColor);
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
}