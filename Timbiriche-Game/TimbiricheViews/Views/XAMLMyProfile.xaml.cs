using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Player;

namespace TimbiricheViews.Views
{
    public partial class XAMLMyProfile : Page
    {
        private Server.Player playerLoggedIn = PlayerSingleton.Player;
        private string _initialPlayerNameLetter;

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
            // TODO: GetSylesOfPlayer
            // TODO: GetColorsOfPlayer
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
        
        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
