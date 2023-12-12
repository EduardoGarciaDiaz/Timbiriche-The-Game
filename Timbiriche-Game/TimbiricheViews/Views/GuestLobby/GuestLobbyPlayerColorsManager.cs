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
    public partial class XAMLGuestLobby : Page, IPlayerColorsManagerCallback
    {
        private const string SELECTED_STROKE_COLOR_HEXADECIMAL = "#000000";
        private const int DEFAULT_SELECTED_COLOR = 0;
        private int[] _idDefaultColors = new int[] { 1, 2, 3, 4 };

        private void ShowSelectPlayerColorGrid()
        {
            gridSelectColor.Visibility = Visibility.Visible;
            SetMyColors();
        }

        private void SetMyColors()
        {
            SolidColorBrush color;

            foreach (int idColor in _idDefaultColors)
            {
                string hexadecimalColor = GetHexadecimalColorByIdColor(idColor);

                color = Utilities.CreateColorFromHexadecimal(hexadecimalColor);
                Rectangle colorRectangle = CreateColorBoxes(idColor, color, PlayerColorTemplate);
                stackPanelColors.Children.Add(colorRectangle);
            }


            ManageColorsSelected();

        }

        private void ManageColorsSelected()
        {
            InstanceContext context = new InstanceContext(this);
            Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);

            try
            {
                LobbyPlayer lobbyPlayer = CreateLobbyPlayer();
                playerColorsManagerClient.SubscribeColorToColorsSelected(_lobbyCode);
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
            catch (FaultException<TimbiricheServerException> ex)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
            }
            catch (FaultException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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

            LobbyPlayer lobbyPlayer2 = CreateLobbyPlayer();

            InstanceContext context = new InstanceContext(this);

            try
            {
                Server.PlayerColorsManagerClient playerColorsManagerClient = new Server.PlayerColorsManagerClient(context);
                playerColorsManagerClient.UnsubscribeColorToColorsSelected(_lobbyCode, lobbyPlayer2);

                _playerLoggedIn.IdColorSelected = idColor;

                LobbyPlayer lobbyPlayer = CreateLobbyPlayer();
                playerColorsManagerClient.RenewSubscriptionToColorsSelected(_lobbyCode, lobbyPlayer);
                MarkAsSelectedColor(rectangleSelected);
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
            }
            catch (FaultException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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
            string selectedHexadecimalColor = GetHexadecimalColorByIdColor(idColor);

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

        private string GetHexadecimalColorByIdColor(int idColor)
        {
            string hexadecimalColor = "";

            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

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
            }
            catch (FaultException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
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
