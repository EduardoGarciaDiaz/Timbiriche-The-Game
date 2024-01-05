using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;
using TimbiricheViews.Views;

namespace TimbiricheViews.Components.Shop
{
    public partial class XAMLColorItemComponent : UserControl
    {
        private readonly Server.ShopColor _color;

        public XAMLColorItemComponent(Server.ShopColor color)
        {
            InitializeComponent();
            this._color = color;
            LoadColorData();
        }

        private void LoadColorData()
        {
            lbColorCost.Content = _color.ColorCost.ToString();
            rectangleColor.Fill = Utilities.CreateColorFromHexadecimal(_color.HexadecimalCode);

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
                    EmergentWindows.CreateEmergentWindow(Properties.Resources.lbPurchaseCompleteTitle,
                        Properties.Resources.tbkPurchaseColorDescription);
                }
                else
                {
                    EmergentWindows.CreateEmergentWindow(Properties.Resources.lbErrorPurchaseTitle,
                        Properties.Resources.tbkErrorPurchaseColorDescription);
                }
            }
            else
            {
                EmergentWindows.CreateEmergentWindow(Properties.Resources.lbInsufficientCoins,
                    Properties.Resources.lbInsufficientCoinsMessage);
            }
        }

        private bool BuyColor()
        {
            bool purchaseCompleted = false;
            ShopManagerClient shopManagerClient = new ShopManagerClient();
        
            try
            {
                purchaseCompleted = shopManagerClient.BuyColor(_color, PlayerSingleton.Player.IdPlayer);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleComponentErrorException(ex);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleComponentErrorException(ex);
            }
            catch (FaultException<TimbiricheServerExceptions>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerExceptions.HandleComponentErrorException(ex);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleComponentFatalException(ex);
            }

            return purchaseCompleted;
        }
    }
}
