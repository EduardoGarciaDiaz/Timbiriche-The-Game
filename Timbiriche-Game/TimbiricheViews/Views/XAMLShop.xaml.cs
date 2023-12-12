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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Components.Shop;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLShop : Page
    {
        public XAMLShop()
        {
            InitializeComponent();
            LoadColors();
            LoadStyles();
            LoadCoins();
        }

        private void LoadCoins()
        {
            lbCoins.Content = PlayerSingleton.Player.Coins;
        }

        private ShopColor[] GetColors()
        {
            Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();
            ShopColor[] colors = null;

            try
            {
                colors = shopManagerClient.GetColors(PlayerSingleton.Player.IdPlayer);
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
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
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

            return colors;
        }

        private void LoadColors()
        {
            ShopColor[] colors = GetColors();

            foreach(var color in colors)
            {
                AddColorItem(color);
            }
        }

        private void AddColorItem(Server.ShopColor color)
        {
            var lastGrid = stackPanelColors.Children.OfType<Grid>().LastOrDefault();

            if (lastGrid != null && lastGrid.Children.Count < 2)
            {
                XAMLColorItemComponent colorItemComponent = new XAMLColorItemComponent(color);
                colorItemComponent.Margin = new Thickness(0, 95, 0, 0);
                lastGrid.Children.Add(colorItemComponent);
            }
            else
            {
                var newGrid = new Grid
                {
                    Width = 82,
                    Height = 173,
                    Margin = new Thickness(0, 10, 15, 15)
                };

                XAMLColorItemComponent colorItemComponent = new XAMLColorItemComponent(color);
                colorItemComponent.Margin = new Thickness(0, 0, 0, 95);
                newGrid.Children.Add(colorItemComponent);
                stackPanelColors.Children.Add(newGrid);
            }
        }

        private ShopStyle[] GetStyles()
        {
            Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();
            ShopStyle[] styles = null;

            try
            {
                shopManagerClient.GetStyles(PlayerSingleton.Player.IdPlayer);
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
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
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

            return styles;
        }

        private void LoadStyles()
        {
            Server.ShopStyle[] styles = GetStyles();

            foreach (var style in styles)
            {
                AddStyleItem(style);
            }
        }

        private void AddStyleItem(Server.ShopStyle style)
        {
            var lastGrid = stackPanelStyles.Children.OfType<Grid>().LastOrDefault();

            if (lastGrid != null && lastGrid.Children.Count < 2)
            {
                XAMLStyleItemComponent styleItemComponent = new XAMLStyleItemComponent(style);
                styleItemComponent.Margin = new Thickness(0, 95, 0, 0);
                lastGrid.Children.Add(styleItemComponent);
            }
            else
            {
                var newGrid = new Grid
                {
                    Width = 82,
                    Height = 173,
                    Margin = new Thickness(0, 10, 15, 15)
                };

                XAMLStyleItemComponent styleItemComponent = new XAMLStyleItemComponent(style);
                styleItemComponent.Margin = new Thickness(0, 0, 0, 95);
                newGrid.Children.Add(styleItemComponent);
                stackPanelStyles.Children.Add(newGrid);
            }
        }

        private void BtnLobby_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
