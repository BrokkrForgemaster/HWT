using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HWT.Presentation.Converters
{
    public class KillTypeToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a kill Type ("FPS", "Air", "Info") into a SolidColorBrush.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string type)
                return new SolidColorBrush(Color.FromRgb(80, 80, 80)); // dark gray

            return type switch
            {
                "FPS"  => new SolidColorBrush(Color.FromRgb(34, 139, 34)),   // forest green
                "Air"  => new SolidColorBrush(Color.FromRgb(178, 34, 34)),   // firebrick red
                "Info" => new SolidColorBrush(Color.FromRgb(80, 80, 80)),    // dark gray
                _      => new SolidColorBrush(Color.FromRgb(80, 80, 80)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}