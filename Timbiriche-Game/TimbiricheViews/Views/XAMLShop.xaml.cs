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
using TimbiricheViews.Components.Shop;

namespace TimbiricheViews.Views
{
    public partial class XAMLShop : Page
    {
        public XAMLShop()
        {
            InitializeComponent();
            LoadColors();
        }

        private void LoadColors()
        {
            Server.ShopManagerClient shopManagerClient = new Server.ShopManagerClient();
            Server.ShopColor[] colors = shopManagerClient.GetColors();

            foreach(var color in colors)
            {
                AgregarElemento(color);
            }
        }

        private void AgregarElemento(Server.ShopColor color)
        {
            var lastGrid = stackPanelColors.Children.OfType<Grid>().LastOrDefault();

            if (lastGrid != null && lastGrid.Children.Count < 2)
            {
                XAMLColorItemComponent colorItemComponent = new XAMLColorItemComponent(color, 2);
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

                XAMLColorItemComponent colorItemComponent = new XAMLColorItemComponent(color, 1);
                newGrid.Children.Add(colorItemComponent);
                stackPanelColors.Children.Add(newGrid);
            }
        }


        private void BtnLobby_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
