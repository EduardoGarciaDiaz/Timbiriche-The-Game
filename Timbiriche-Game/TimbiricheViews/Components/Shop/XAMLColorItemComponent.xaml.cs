using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace TimbiricheViews.Components.Shop
{
    public partial class XAMLColorItemComponent : UserControl
    {
        private Server.ShopColor color;

        public XAMLColorItemComponent(Server.ShopColor color)
        {
            InitializeComponent();
            this.color = color;
            LoadColorData();
        }

        private void LoadColorData()
        {
            lbColorCost.Content = color.ColorCost.ToString();
            rectangleColor.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color.HexadecimalCode));

            if (color.OwnedColor)
            {
                gridOwnedColor.Visibility = Visibility.Visible;
            }
        }

        private void RectangleColor_MouseEnter(object sender, MouseEventArgs e)
        {
            gridBuyColor.Visibility = Visibility.Visible;
        }

        private void GridBuyColor_MouseLeave(object sender, MouseEventArgs e)
        {
            gridBuyColor.Visibility = Visibility.Collapsed;
        }

        private void BtnBuyColor_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerSingleton.Player.Coins >= color.ColorCost)
            {
                Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();
                bool purchaseCompleted = shopManagerClient.BuyColor(color, PlayerSingleton.Player.IdPlayer);

                if (purchaseCompleted)
                {
                    PlayerSingleton.Player.Coins -= color.ColorCost;
                    gridOwnedColor.Visibility = Visibility.Visible;
                    Utils.EmergentWindows.CreateEmergentWindow("Compra Completada", "Compraste el color correctamente.");
                }
                else
                {
                    Utils.EmergentWindows.CreateEmergentWindow("Error al realizar la compra", "No fue posible comprar el color. Intentálo mas tarde.");
                }
            }
            else
            {
                Utils.EmergentWindows.CreateEmergentWindow(Properties.Resources.lbInsufficientCoins, Properties.Resources.lbInsufficientCoinsMessage);
            }
        }
    }
}
