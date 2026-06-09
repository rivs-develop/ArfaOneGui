using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RIVS.ASAK.ARFA.GUI.Enums;

namespace RIVS.ASAK.ARFA.GUI.Converters
{

    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not EState)
            {
                return null;
            }
            var val = (EState)value;
            return Convert(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Brush Convert(EState value)
        {
            switch (value)
            {
                case EState.None:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FF2D2D30");
                case EState.Ok:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FF469646");
                case EState.Warning:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF00");
                case EState.Error:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFF0000");
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
