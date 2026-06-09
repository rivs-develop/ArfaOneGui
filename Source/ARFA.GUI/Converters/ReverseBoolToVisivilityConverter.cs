using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RIVS.ASAK.ARFA.GUI.Converters
{
    public class ReverseBoolToVisibilityConverter : IValueConverter
    {
        // Преобразует bool в Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        // Преобразует Visibility обратно в bool (опционально)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed;
            }
            return false;
        }
    }
}
