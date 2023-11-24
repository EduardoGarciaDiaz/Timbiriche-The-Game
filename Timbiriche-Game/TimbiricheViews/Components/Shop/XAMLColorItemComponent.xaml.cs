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

namespace TimbiricheViews.Components.Shop
{
    public partial class XAMLColorItemComponent : UserControl
    {
        public XAMLColorItemComponent(Server.ShopColor color, int numberItemInGrid)
        {
            InitializeComponent();
            SetItemPosition(numberItemInGrid);
            LoadColorData(color);
        }

        private void SetItemPosition(int numberItemInGrid)
        {
            switch (numberItemInGrid)
            {
                case 1:
                    this.Margin = new Thickness(0, 0, 0, 95);
                    break;
                case 2:
                    this.Margin = new Thickness(0, 95, 0, 0);
                    break;
            }
        }

        private void LoadColorData(Server.ShopColor color)
        {
            lbColorCost.Content = color.ColorCost.ToString();
            rectangleColor.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color.HexadecimalCode));
        }

        private void GridBuyColor_MouseEnter(object sender, MouseEventArgs e)
        {
            gridBuyColor.Visibility = Visibility.Visible;
        }

        private void GridBuyColor_MouseLeave(object sender, MouseEventArgs e)
        {
            gridBuyColor.Visibility = Visibility.Collapsed;
        }
    }
}
