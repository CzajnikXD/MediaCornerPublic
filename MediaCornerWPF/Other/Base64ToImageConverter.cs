using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MediaCornerWPF.Other
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imageSource && !string.IsNullOrWhiteSpace(imageSource))
            {
                // Check if it's a pack URI (resource)
                if (imageSource.StartsWith("pack://"))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(imageSource, UriKind.Absolute); 
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
                }

                // Convert base64 string to image
                if (!imageSource.StartsWith("pack://"))
                {
                    byte[] imageBytes = System.Convert.FromBase64String(imageSource);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        return image;
                    }
                }
            }

            // Return null if no valid image is provided
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}