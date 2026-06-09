using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.UIElements.Tools.ValueConverters
{

    public class MessageTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is EMessageType))
            {
                return null;
            }
            var val = (EMessageType)value;
            return Convert(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Brush Convert(EMessageType value)
        {
            switch (value)
            {
                case EMessageType.None:
                case EMessageType.Info:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFFFF");
                case EMessageType.Warning:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF00");
                case EMessageType.Error:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFF0000");
                case EMessageType.Debug:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFD3D3D3");
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
