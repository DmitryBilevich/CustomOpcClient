using System;
using System.Windows.Data;

namespace CustomOpcClient
{
    public class JogMotorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;       

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool) value;       
    }
}
