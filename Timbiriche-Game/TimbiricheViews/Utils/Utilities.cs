using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TimbiricheViews.Components;
using TimbiricheViews.Components.Lobby;

namespace TimbiricheViews.Utils
{
    public class Utilities
    {
        public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));
        }
    }

}