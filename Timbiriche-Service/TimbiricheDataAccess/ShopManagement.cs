using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public static class ShopManagement
    {
        public static List<Colors> GetColors()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var colors = context.Colors.ToList();

                return colors;
            }
        }

        public static List<Styles> GetStyles()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var styles = context.Styles.ToList();

                return styles;
            }
        }
    }
}
