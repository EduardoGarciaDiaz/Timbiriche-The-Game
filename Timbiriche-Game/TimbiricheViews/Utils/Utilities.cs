using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TimbiricheViews.Components;
using TimbiricheViews.Components.Lobby;
using Path = System.IO.Path;

namespace TimbiricheViews.Utils
{
    public class Utilities
    {
        public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
        {
            SolidColorBrush solidColorBrush = null;
            try
            {
                solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));

            }
            catch (FormatException ex)
            {
                // TODO: Log:
            }
            return solidColorBrush;
        }

        public static Image CreateImageByPath(string imagePath)
        {
            string absolutePath = BuildAbsolutePath(imagePath);

            Image styleImage = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
            styleImage.Source = bitmapImage;

            return styleImage;
        }

        public static string BuildAbsolutePath(string relativePath)
        {
            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            return absolutePath;
        }
    }

}