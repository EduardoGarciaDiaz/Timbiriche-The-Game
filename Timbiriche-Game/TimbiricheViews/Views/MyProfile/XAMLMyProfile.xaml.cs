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
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;
using Path = System.IO.Path;
using Serilog;

namespace TimbiricheViews.Views
{
    public partial class XAMLMyProfile : Page
    {
        private static readonly ILogger _logger = LoggerManager.GetLogger();
        private Server.Player playerLoggedIn = PlayerSingleton.Player;
        private PlayerStyle[] _myStyles;
        private string _initialPlayerNameLetter;
        const int ID_DEFAULT_STYLE = 1;
        private string HEXADECIMAL_RECTANGLE_COLOR = "#FF6C6868";
        private const string HEXADECIMAL_COLOR_BTN_PRESSED = "#0F78C4";
        private const string HEXADECIMAL_COLOR_BTN_NOT_PRESSED = "#1C95D1";
        private SolidColorBrush colorButtonPressed = Utilities.CreateColorFromHexadecimal(HEXADECIMAL_COLOR_BTN_PRESSED);
        private SolidColorBrush colorButtonNotPressed = Utilities.CreateColorFromHexadecimal(HEXADECIMAL_COLOR_BTN_NOT_PRESSED);

        public XAMLMyProfile()
        {
            InitializeComponent();
            LoadDataPlayer();
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadDataPlayer()
        {
            const int INDEX_FIRST_LETTER = 0;
            lbUsername.Content = playerLoggedIn.Username;
            _initialPlayerNameLetter = playerLoggedIn.Username[INDEX_FIRST_LETTER].ToString();

            GetMyStyles();
        }

        private void GetMyStyles()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            try
            {
                _myStyles = playerCustomizationManagerClient.GetMyStyles(playerLoggedIn.IdPlayer);
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

            if (_myStyles != null)
            {
                SetMyStyles();
            }
            else
            {
                _logger.Warning("The player doesn't have styles related. - Class MyProfile.xaml.cs on method GetMyStyles");
            }
        }

        private void SetMyStyles()
        {
            SetDefaultStyleFacebox();

            foreach (PlayerStyle playerStyle in _myStyles)
            {
                if(playerStyle.IdStyle != ID_DEFAULT_STYLE)
                {
                    Grid gridPlayerStyle = CreateStyle(playerStyle);
                    wrapPanelPlayerStyles.Children.Add(gridPlayerStyle);
                }
            }
        }

        private void SetDefaultStyleFacebox()
        {
            Grid gridPlayerStyle = XamlReader.Parse(XamlWriter.Save(gridPlayerStyleTemplate)) as Grid;
            gridPlayerStyle.Name = "gridStylePlayer" + "_" + ID_DEFAULT_STYLE;
            gridPlayerStyle.MouseLeftButtonDown += GridStyle_Click;
            gridPlayerStyle.IsEnabled = true;
            gridPlayerStyle.Visibility = Visibility.Visible;

            Label lbInitialNameLetter = XamlReader.Parse(XamlWriter.Save(lbPlayerStyleTemplate)) as Label;

            Rectangle backgroundRectangle = CreateBackgroundRectangle(ID_DEFAULT_STYLE);

            lbInitialNameLetter.Name = "lbPlayerStyle" + "_" + ID_DEFAULT_STYLE;
            lbInitialNameLetter.Content = _initialPlayerNameLetter;
            lbInitialNameLetter.Visibility = Visibility.Visible;

            gridPlayerStyle.Children.Add(backgroundRectangle);
            gridPlayerStyle.Children.Add(lbInitialNameLetter);

            if (IsCurrentStyle(ID_DEFAULT_STYLE))
            {
                MarkAsSelectedStyle(gridPlayerStyle);
            }

            wrapPanelPlayerStyles.Children.Add(gridPlayerStyle);
        }

        private Grid CreateStyle(PlayerStyle playerStyle)
        {
            int idStyle = playerStyle.IdStyle;
            Grid gridPlayerStyle = XamlReader.Parse(XamlWriter.Save(gridPlayerStyleTemplate)) as Grid;
            gridPlayerStyle.Name = "gridStylePlayer" + "_" + idStyle;
            gridPlayerStyle.MouseLeftButtonDown += GridStyle_Click;
            gridPlayerStyle.IsEnabled = true;
            gridPlayerStyle.Visibility = Visibility.Visible;

            Rectangle rectangleBackground = CreateBackgroundRectangle(idStyle);
            Label lbPlayerStyle = CreateLabelPlayerStyle(idStyle);

            gridPlayerStyle.Children.Add(rectangleBackground);
            gridPlayerStyle.Children.Add(lbPlayerStyle);

            if(IsCurrentStyle(idStyle))
            {
                MarkAsSelectedStyle(gridPlayerStyle);
            }

            return gridPlayerStyle;
        }

        private bool IsCurrentStyle(int idStyle)
        {
            bool isCurrentStyleSelected = false;

            if (idStyle == playerLoggedIn.IdStyleSelected)
            {
                isCurrentStyleSelected = true;
            }

            return isCurrentStyleSelected;
        }

        private Rectangle CreateBackgroundRectangle(int idStyle)
        {
            SolidColorBrush colorBackground = Utilities.CreateColorFromHexadecimal(HEXADECIMAL_RECTANGLE_COLOR);
            Rectangle backgroundRectangle = XamlReader.Parse(XamlWriter.Save(rectanglePlayerStyleBackgroundTemplate)) as Rectangle;
            backgroundRectangle.Name = "rectangleBackground" + "_" + idStyle;
            backgroundRectangle.Fill = colorBackground;
            backgroundRectangle.IsEnabled = false;
            backgroundRectangle.Visibility = Visibility.Visible;

            return backgroundRectangle;
        }

        private Label CreateLabelPlayerStyle(int idStyle)
        {
            Image styleImage = CreateImageByIdStyle(idStyle);

            Label lbStyle = XamlReader.Parse(XamlWriter.Save(lbPlayerStyleTemplate)) as Label;
            lbStyle.Name = "lbPlayerStyle" + "_" + idStyle;
            lbStyle.Content = styleImage;
            lbStyle.Visibility = Visibility.Visible;

            return lbStyle;
        }

        private Image CreateImageByIdStyle(int idStyle)
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            Image styleImage = null;

            try
            {
                string stylePath = playerCustomizationManagerClient.GetStylePath(idStyle);
                styleImage = Utilities.CreateImageByPath(stylePath);
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

            return styleImage;
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            gridCustomizeProfile.Visibility = Visibility.Visible;
            gridCustomizeCharacter.Visibility = Visibility.Collapsed;
            btnProfile.Background = colorButtonPressed;
            btnCharacter.Background = colorButtonNotPressed;

            LoadPersonalDataPlayer();
        }

        private void BtnCharacter_Click(object sender, RoutedEventArgs e)
        {
            gridCustomizeCharacter.Visibility = Visibility.Visible;
            gridCustomizeProfile.Visibility = Visibility.Collapsed;
            btnCharacter.Background = colorButtonPressed;
            btnProfile.Background = colorButtonNotPressed;
        }

        private void GridStyle_Click(object sender, MouseButtonEventArgs e)
        {
            Grid clickedGrid = sender as Grid;
            SelectStyle(clickedGrid);
        }

        private void SelectStyle(Grid gridSelected)
        {
            const char SPLIT_SYMBOL = '_';
            const int INDEX_ID_COLOR_PART = 1;
            string[] nameParts = gridSelected.Name.ToString().Split(SPLIT_SYMBOL);
            int idStyle = int.Parse(nameParts[INDEX_ID_COLOR_PART]);

            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            try
            {
                playerCustomizationManagerClient.SelectMyStyle(playerLoggedIn.IdPlayer, idStyle);
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

            playerLoggedIn.IdStyleSelected = idStyle;
            MarkAsSelectedStyle(gridSelected);
        }

        private void MarkAsSelectedStyle(Grid gridSelected)
        {
            const int INDEX_BACKGORUND_RECTANGLE = 2;
            const int SIZE_STYLE_SELECTED = 175;

            Rectangle rectangleSelected = gridSelected.Children[INDEX_BACKGORUND_RECTANGLE] as Rectangle;
            gridSelected.Width = SIZE_STYLE_SELECTED;
            gridSelected.Height = SIZE_STYLE_SELECTED;
            rectangleSelected.Width = SIZE_STYLE_SELECTED;
            rectangleSelected.Height = SIZE_STYLE_SELECTED;

            ClearOtherStylesSelections(gridSelected);
        }

        private void ClearOtherStylesSelections(Grid gridSelected)
        {
            const int INDEX_BACKGORUND_RECTANGLE = 2;
            const int SIZE_STYLE_NOT_SELECTED = 150;
            const string GRID_STYLE_TEMPLATE = "gridPlayerStyleTemplate";

            foreach (Grid gridStylePlayer in wrapPanelPlayerStyles.Children)
            {
                if (gridStylePlayer.Name != gridSelected.Name && !gridStylePlayer.Name.Equals(GRID_STYLE_TEMPLATE))
                {
                    Rectangle rectangleStyle = gridStylePlayer.Children[INDEX_BACKGORUND_RECTANGLE] as Rectangle;
                    gridStylePlayer.Width = SIZE_STYLE_NOT_SELECTED;
                    gridStylePlayer.Height = SIZE_STYLE_NOT_SELECTED;
                    rectangleStyle.Width = SIZE_STYLE_NOT_SELECTED;
                    rectangleStyle.Height = SIZE_STYLE_NOT_SELECTED;
                }
            }
        }       
    }
}
