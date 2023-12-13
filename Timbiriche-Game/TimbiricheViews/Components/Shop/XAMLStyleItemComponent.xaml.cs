using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TimbiricheViews.Player;
using TimbiricheViews.Utils;
using Path = System.IO.Path;


namespace TimbiricheViews.Components.Shop
{
    public partial class XAMLStyleItemComponent : UserControl
    {
        private Server.ShopStyle _style;

        public XAMLStyleItemComponent(Server.ShopStyle style)
        {
            InitializeComponent();
            this._style = style;
            LoadStyleData();
        }

        private void LoadStyleData()
        {
            string stylePath = _style.StylePath;
            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, stylePath);

            BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
            imgStyle.Source = bitmapImage;

            lbColorCost.Content = _style.StyleCost;

            if (_style.OwnedStyle)
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
            if(PlayerSingleton.Player.Coins >= _style.StyleCost)
            {
                bool purchaseCompleted = BuyStyle();

                if (purchaseCompleted)
                {
                    PlayerSingleton.Player.Coins -= _style.StyleCost;
                    gridOwnedSyle.Visibility = Visibility.Visible;
                    EmergentWindows.CreateEmergentWindow(Properties.Resources.lbPurchaseCompleteTitle,
                        Properties.Resources.tbkPurchaseStyleDescription);
                }
                else
                {
                    EmergentWindows.CreateEmergentWindow(Properties.Resources.lbErrorPurchaseTitle,
                        Properties.Resources.tbkErrorPurchaseStyleDescription);
                }
            }
            else
            {
                EmergentWindows.CreateEmergentWindow(Properties.Resources.lbInsufficientCoins, Properties.Resources.lbInsufficientCoinsMessage);
            }
        }

        private bool BuyStyle()
        {
            bool purchaseCompleted = false;
            Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();

            try
            {
                purchaseCompleted = shopManagerClient.BuyStyle(_style, PlayerSingleton.Player.IdPlayer);
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
