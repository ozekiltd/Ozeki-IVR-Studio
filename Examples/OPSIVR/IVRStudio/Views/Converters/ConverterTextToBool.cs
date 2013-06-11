using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace OPS_IVR_Studio.Views.Converters
{
    [ValueConversion(typeof(String), typeof(bool))]
    class ConverterTextToBool : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (String.IsNullOrEmpty((string)value) || String.IsNullOrWhiteSpace((string)value))
                    return false;
                return true;
            }
            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
