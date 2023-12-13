using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace TimbiricheViews.Utils
{
    public static class Utilities
    {
        public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
        {
            SolidColorBrush solidColorBrush = null;

            if (hexadecimalColor != null)
            {
                try
                {
                    solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));
                }
                catch (FormatException ex)
                {
                    HandlerExceptions.HandleComponentErrorException(ex);
                }
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