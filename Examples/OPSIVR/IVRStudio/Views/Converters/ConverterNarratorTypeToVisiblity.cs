using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OPSIVRSystem.IVRMenus;

namespace OPS_IVR_Studio.Views.Converters
{
    [ValueConversion(typeof (NarratorType), typeof (Visibility))]
    public class ConverterNarratorTypeToVisiblity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ApplicationIsInDesignMode)
                return Visibility.Visible;
            switch (((string) parameter))
            {
                case "text":
                    if ( value == Enum.GetName(typeof(NarratorType),NarratorType.TextToSpeech))
                        return Visibility.Visible;
                    break;
                case "audio":
                    if ( value == Enum.GetName(typeof(NarratorType),NarratorType.FilePlayback))
                        return Visibility.Visible;
                    break;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private static bool ApplicationIsInDesignMode
        {
            get
            {
                return (bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof (DependencyObject)).DefaultValue);
            }
        }
    }
}
