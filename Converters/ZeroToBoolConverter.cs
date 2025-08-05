using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace pra.Converters
{
    public class ZeroToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count == 0; // Muestra el control si la lista está vacía
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
