using System;
using System.Globalization;
using System.Windows.Data;

namespace RIVS.ASAK.ARFA.GUI.Converters
{

    public class CurrentRecordToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not long)
            {
                return null;
            }
            var val = (long)value;
            return Convert(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string Convert(long value)
        {
            return value <= 0 ? "0" : value < 3 ? $"P{value}" : $"{value - 2}";
        }
    }
}
