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

namespace TimbiricheViews.Views
{
    public partial class XAMLMyProfile : Page
    {
        private Server.Player playerLoggedIn = PlayerSingleton.Player;
        private PlayerStyle[] _myStyles;
        private string _initialPlayerNameLetter;
        private string _hexadecimalColorRectangle = "#FF6C6868";
        private const string SELECTED_STROKE_STYLE_HEXADECIMAL = "#000000";

        private const string HEXADECIMAL_COLOR_BTN_PRESSED = "#0F78C4";
        private const string HEXADECIMAL_COLOR_BTN_NOT_PRESSED = "#1C95D1";
        private SolidColorBrush colorButtonPressed = (SolidColorBrush)(new BrushConverter()
            .ConvertFrom(HEXADECIMAL_COLOR_BTN_PRESSED));
        private SolidColorBrush colorButtonNotPressed = (SolidColorBrush)(new BrushConverter()
            .ConvertFrom(HEXADECIMAL_COLOR_BTN_NOT_PRESSED));



        public XAMLMyProfile()
        {
            InitializeComponent();
            LoadDataPlayer();
        }

        private void DpBirthdate_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                datePicker.DisplayDateEnd = DateTime.Today.AddYears(-3);
            }
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadDataPlayer()
        {
            lbUsername.Content = playerLoggedIn.Username;
            _initialPlayerNameLetter = playerLoggedIn.Username[0].ToString();
            GetMyStyles();
        }

        private void GetMyStyles()
        {
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();

            _myStyles = playerCustomizationManagerClient.GetMyStyles(playerLoggedIn.IdPlayer);
            if (_myStyles != null)
            {
                SetMyStyles();
            }
            else
            {
                Console.WriteLine("The player doesn't have styles related");
            }
        }

        private void SetMyStyles()
        {
            //SetDefaultStyleFacebox();
            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            SolidColorBrush colorBackground;
            foreach (PlayerStyle playerStyle in _myStyles)
            {
                string stylePath = playerCustomizationManagerClient.GetStylePath(playerStyle.IdStyle);

                string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stylePath);
                ImageBrush styleImage = CreateImageByPath(absolutePath);
                Rectangle styleRectangle = CreateStylesRectangles(playerStyle.IdStyle, styleImage, PlayerStyleTemplate);

                colorBackground = Utilities.CreateColorFromHexadecimal(_hexadecimalColorRectangle);
                Rectangle backgroundRectangle = CreateBackgroundRectangle(playerStyle.IdStyle, colorBackground,
                    PlayerStyleBackgroundTemplate);

                wrapPanelPlayerStyles.Children.Add(backgroundRectangle);
                wrapPanelPlayerStyles.Children.Add(styleRectangle);
            }
        }

        //private void SetDefaultStyleFacebox()
        //{
        //    SolidColorBrush colorBackground;
        //    const int ID_DEFAULT_STYLE = 0;
        //    colorBackground = Utilities.CreateColorFromHexadecimal(_hexadecimalColorRectangle);
        //    Label lbInitialNameLetter = XamlReader.Parse(XamlWriter.Save(lbInitialPlayerNameLetter)) as Label;

        //    Rectangle backgroundRectangle = CreateBackgroundRectangle(ID_DEFAULT_STYLE, colorBackground,
        //        PlayerStyleBackgroundTemplate);

        //    lbInitialNameLetter.Name = "styleRectangle" + "_" + ID_DEFAULT_STYLE;
        //    lbInitialNameLetter.Content = _initialPlayerNameLetter;
        //    lbInitialNameLetter.MouseLeftButtonDown += RectangleStyle_Click;
        //    lbInitialNameLetter.Visibility = Visibility.Visible;

        //    wrapPanelPlayerStyles.Children.Add(backgroundRectangle);
        //    wrapPanelPlayerStyles.Children.Add(lbInitialNameLetter);
        //}

        private Rectangle CreateBackgroundRectangle(int idStyle, SolidColorBrush colorBackground, Rectangle rectangleTemplate)
        {
            Rectangle backgroundRectangle = XamlReader.Parse(XamlWriter.Save(rectangleTemplate)) as Rectangle;
            backgroundRectangle.Name = "backgroundRectangle" + "_" + idStyle;
            backgroundRectangle.Fill = colorBackground;
            backgroundRectangle.IsEnabled = false;
            backgroundRectangle.Visibility = Visibility.Visible;
            return backgroundRectangle;
        }

        private Rectangle CreateStylesRectangles(int idStyle, ImageBrush styleImage, Rectangle rectangleTemplate)
        {
            Rectangle styleRectangle = XamlReader.Parse(XamlWriter.Save(rectangleTemplate)) as Rectangle;
            styleRectangle.Name = "styleRectangle" + "_" + idStyle;
            styleRectangle.Fill = styleImage;
            styleRectangle.MouseLeftButtonDown += RectangleStyle_Click;
            styleRectangle.IsEnabled = true;
            styleRectangle.Visibility = Visibility.Visible;
            return styleRectangle;
        }

        private ImageBrush CreateImageByPath(string stylePath)
        {
            ImageBrush styleImage = new ImageBrush();
            BitmapImage bitmapImage = new BitmapImage(new Uri(stylePath));
            styleImage.ImageSource = bitmapImage;

            return styleImage;
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            gridCustomizeProfile.Visibility = Visibility.Visible;
            gridCustomizeCharacter.Visibility = Visibility.Collapsed;
            btnProfile.Background = colorButtonPressed;
            btnCharacter.Background = colorButtonNotPressed;

        }

        private void BtnCharacter_Click(object sender, RoutedEventArgs e)
        {
            gridCustomizeCharacter.Visibility = Visibility.Visible;
            gridCustomizeProfile.Visibility = Visibility.Collapsed;
            btnCharacter.Background = colorButtonPressed;
            btnProfile.Background = colorButtonNotPressed;
        }

        private void RectangleStyle_Click(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            SelectStyle(clickedRectangle);
        }

        private void SelectStyle(Rectangle rectangleSelected)
        {
            const char SPLIT_SYMBOL = '_';
            const int INDEX_ID_COLOR_PART = 1;
            string[] nameParts = rectangleSelected.Name.ToString().Split(SPLIT_SYMBOL);
            int idStyle = int.Parse(nameParts[INDEX_ID_COLOR_PART]);

            Server.PlayerCustomizationManagerClient playerCustomizationManagerClient = new Server.PlayerCustomizationManagerClient();
            playerCustomizationManagerClient.SelectMyStyle(playerLoggedIn.IdPlayer, idStyle);
            playerLoggedIn.IdStyleSelected = idStyle;

            MarkAsSelectedStyle(rectangleSelected);
        }

        private void MarkAsSelectedStyle(Rectangle rectangleSelected)
        {
            SolidColorBrush blackColor = Utilities.CreateColorFromHexadecimal(SELECTED_STROKE_STYLE_HEXADECIMAL);
            rectangleSelected.Stroke = blackColor;
            rectangleSelected.StrokeThickness = 2;
            ClearOtherStylesSelections(rectangleSelected);
            // TODO: UpdatePlayerStyle(rectangleSelected);
        }

        private void ClearOtherStylesSelections(Rectangle rectangleSelected)
        {
            foreach (Rectangle styleRectangle in wrapPanelPlayerStyles.Children)
            {
                if (styleRectangle.Name != rectangleSelected.Name)
                {
                    styleRectangle.Stroke = null;
                }
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
