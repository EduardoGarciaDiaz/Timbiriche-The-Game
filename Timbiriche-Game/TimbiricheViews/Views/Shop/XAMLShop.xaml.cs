using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TimbiricheViews.Components.Shop;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLShop : Page
    {
        private const float WIDTH_GRID = 82;
        private const float HEIGHT_GRID = 173;

        public XAMLShop()
        {
            InitializeComponent();
            this.Loaded += Lobby_Loaded;
        }

        private void Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            LoadShopItems();
        }

        private void LoadShopItems()
        {
            try
            {
                LoadColors();
                LoadStyles();
                LoadCoins();
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerExceptions>)
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
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }
        }

        private void LoadCoins()
        {
            lbCoins.Content = PlayerSingleton.Player.Coins;
        }

        private ShopColor[] GetColors()
        {
            ShopManagerClient shopManagerClient = new ShopManagerClient();
            ShopColor[] colors = null;

            //try
            //{
                colors = shopManagerClient.GetColors(PlayerSingleton.Player.IdPlayer);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    EmergentWindows.CreateConnectionFailedMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (TimeoutException ex)
            //{
            //    EmergentWindows.CreateTimeOutMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (FaultException<TimbiricheServerExceptions>)
            //{
            //    EmergentWindows.CreateDataBaseErrorMessageWindow();
            //    NavigationService.Navigate(new XAMLLogin());
            //}
            //catch (FaultException)
            //{
            //    EmergentWindows.CreateServerErrorMessageWindow();
            //    NavigationService.Navigate(new XAMLLogin());
            //}
            //catch (CommunicationException ex)
            //{
            //    EmergentWindows.CreateServerErrorMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (Exception ex)
            //{
            //    EmergentWindows.CreateUnexpectedErrorMessageWindow();
            //    HandlerExceptions.HandleFatalException(ex, NavigationService);
            //}

            return colors;
        }

        private void LoadColors()
        {
            ShopColor[] colors = GetColors();

            if (colors != null)
            {
                foreach (var color in colors)
                {
                    AddColorItem(color);
                }
            }
        }

        private void AddColorItem(ShopColor color)
        {
            int maxChildCount = 2;
            var lastGrid = stackPanelColors.Children.OfType<Grid>().LastOrDefault();

            if (lastGrid != null && lastGrid.Children.Count < maxChildCount)
            {
                XAMLColorItemComponent colorItemComponent = new XAMLColorItemComponent(color)
                {
                    Margin = new Thickness(0, 95, 0, 0)
                };

                lastGrid.Children.Add(colorItemComponent);
            }
            else
            {
                var newGrid = new Grid
                {
                    Width = WIDTH_GRID,
                    Height = HEIGHT_GRID,
                    Margin = new Thickness(0, 10, 15, 15)
                };

                XAMLColorItemComponent colorItemComponent = new XAMLColorItemComponent(color)
                {
                    Margin = new Thickness(0, 0, 0, 95)
                };

                newGrid.Children.Add(colorItemComponent);
                stackPanelColors.Children.Add(newGrid);
            }
        }

        private ShopStyle[] GetStyles()
        {
            ShopManagerClient shopManagerClient = new ShopManagerClient();
            ShopStyle[] styles = null;

            //try
            //{
                styles = shopManagerClient.GetStyles(PlayerSingleton.Player.IdPlayer);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    EmergentWindows.CreateConnectionFailedMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (TimeoutException ex)
            //{
            //    EmergentWindows.CreateTimeOutMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (FaultException<TimbiricheServerExceptions>)
            //{
            //    EmergentWindows.CreateDataBaseErrorMessageWindow();
            //    NavigationService.Navigate(new XAMLLogin());
            //}
            //catch (FaultException)
            //{
            //    EmergentWindows.CreateServerErrorMessageWindow();
            //    NavigationService.Navigate(new XAMLLogin());
            //}
            //catch (CommunicationException ex)
            //{
            //    EmergentWindows.CreateServerErrorMessageWindow();
            //    HandlerExceptions.HandleErrorException(ex, NavigationService);
            //}
            //catch (Exception ex)
            //{
            //    EmergentWindows.CreateUnexpectedErrorMessageWindow();
            //    HandlerExceptions.HandleFatalException(ex, NavigationService);
            //}

            return styles;
        }

        private void LoadStyles()
        {
            ShopStyle[] styles = GetStyles();

            if (styles != null)
            {
                foreach (var style in styles)
                {
                    AddStyleItem(style);
                }
            }
        }

        private void AddStyleItem(ShopStyle style)
        {
            var lastGrid = stackPanelStyles.Children.OfType<Grid>().LastOrDefault();
            int maxChildrenCount = 2;

            if (lastGrid != null && lastGrid.Children.Count < maxChildrenCount)
            {
                XAMLStyleItemComponent styleItemComponent = new XAMLStyleItemComponent(style)
                {
                    Margin = new Thickness(0, 95, 0, 0)
                };
                lastGrid.Children.Add(styleItemComponent);
            }
            else
            {
                var newGrid = new Grid
                {
                    Width = WIDTH_GRID,
                    Height = HEIGHT_GRID,
                    Margin = new Thickness(0, 10, 15, 15)
                };

                XAMLStyleItemComponent styleItemComponent = new XAMLStyleItemComponent(style)
                {
                    Margin = new Thickness(0, 0, 0, 95)
                };

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
