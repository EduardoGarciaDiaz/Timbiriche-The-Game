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
using Path = System.IO.Path;


namespace TimbiricheViews.Components.Shop
{
    public partial class XAMLStyleItemComponent : UserControl
    {
        private Server.ShopStyle style;

        public XAMLStyleItemComponent(Server.ShopStyle style)
        {
            InitializeComponent();
            this.style = style;
            LoadStyleData();
        }

        private void LoadStyleData()
        {
            string stylePath = style.StylePath;
            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stylePath);

            BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
            imgStyle.Source = bitmapImage;

            lbColorCost.Content = style.StyleCost;

            if (style.OwnedStyle)
            {
                gridOwnedSyle.Visibility = Visibility.Visible;
            }
        }

        private void ImageStyle_MouseEnter(object sender, MouseEventArgs e)
        {
            gridBuySyle.Visibility = Visibility.Visible;
        }

        private void GridBuyStyle_MouseLeave(object sender, MouseEventArgs e)
        {
            gridBuySyle.Visibility = Visibility.Collapsed;
        }

        private void BtnBuyStyle_Click(object sender, RoutedEventArgs e)
        {
            if(PlayerSingleton.Player.Coins >= style.StyleCost)
            {
                Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();
                bool purchaseCompleted = shopManagerClient.BuyStyle(style, PlayerSingleton.Player.IdPlayer);

                if (purchaseCompleted)
                {
                    PlayerSingleton.Player.Coins -= style.StyleCost;
                    gridOwnedSyle.Visibility = Visibility.Visible;
                    Utils.EmergentWindows.CreateEmergentWindow("Compra Completada", "Compraste el estilo correctamente.");
                }
                else
                {
                    Utils.EmergentWindows.CreateEmergentWindow("Error al realizar la compra", "No fue posible comprar el estilo. Intentálo mas tarde.");
                }
            }
            else
            {
                Utils.EmergentWindows.CreateEmergentWindow(Properties.Resources.lbInsufficientCoins, Properties.Resources.lbInsufficientCoinsMessage);

            }
        }
    }
}
