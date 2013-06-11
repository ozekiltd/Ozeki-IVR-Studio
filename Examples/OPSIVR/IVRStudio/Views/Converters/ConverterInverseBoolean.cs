using System;
using System.Windows.Data;

namespace OPS_IVR_Studio.Views.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    class ConverterInverseBoolean : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
          System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
