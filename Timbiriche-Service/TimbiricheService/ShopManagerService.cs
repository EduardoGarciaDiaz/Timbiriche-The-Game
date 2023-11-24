using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    public partial class UserManagerService : IShopManager
    {
        public List<ShopColor> GetColors()
        {
            List<Colors> colorsFromDataBase = ShopManagement.GetColors();
            List<ShopColor> colors = new List<ShopColor>();

            foreach(Colors color in colorsFromDataBase)
            {
                ShopColor singleColor = new ShopColor();
                singleColor.IdColor = color.idColor;
                singleColor.ColorName = color.colorName;
                singleColor.ColorCost = (int) color.cost;
                singleColor.HexadecimalCode = color.hexadecimalCode;

                colors.Add(singleColor);
            }

            return colors;
        }

        public List<ShopStyle> GetStyles()
        {
            List<Styles> stylesFromDataBase = ShopManagement.GetStyles();
            List<ShopStyle> styles = new List<ShopStyle>();

            foreach (Styles style in stylesFromDataBase)
            {
                ShopStyle singleStyle = new ShopStyle();
                singleStyle.IdStyle = style.idStyle;
                singleStyle.StyleName = style.styleName;
                singleStyle.StyleCost = (int)style.cost;
                singleStyle.StylePath = style.path;

                styles.Add(singleStyle);
            }

            return styles;
        }
    }
}
