using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
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
using TimbiricheViews.Utils;

namespace TimbiricheViews.Components.Shop
{
    public partial class XAMLColorItemComponent : UserControl
    {
        private Server.ShopColor _color;

        public XAMLColorItemComponent(Server.ShopColor color)
        {
            InitializeComponent();
            this._color = color;
            LoadColorData();
        }

        private void LoadColorData()
        {
            lbColorCost.Content = _color.ColorCost.ToString();
            rectangleColor.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color.HexadecimalCode));

            if (_color.OwnedColor)
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
            if (PlayerSingleton.Player.Coins >= _color.ColorCost)
            {
                bool purchaseCompleted = BuyColor();

                if (purchaseCompleted)
                {
                    PlayerSingleton.Player.Coins -= _color.ColorCost;
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

        private bool BuyColor()
        {
            bool purchaseCompleted = false;
            Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();
        
            try
            {
                purchaseCompleted = shopManagerClient.BuyColor(_color, PlayerSingleton.Player.IdPlayer);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleComponentErrorException(ex);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleComponentErrorException(ex);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleComponentErrorException(ex);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleComponentFatalException(ex);
            }

            return purchaseCompleted;
        }
    }
}
