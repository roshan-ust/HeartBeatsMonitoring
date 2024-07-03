using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace HeartBeats.Utils
{
    public class WarningMailStatusToBooleanConverter : IValueConverter
    {
        private static readonly Dictionary<string, Constants.WarningMailActions> ContentToEnumMap = new Dictionary<string, Constants.WarningMailActions>
    {
        { "IgnoreWarning", Constants.WarningMailActions.Ignore },
        { "ConsiderWarningAsError", Constants.WarningMailActions.ConsiderAsError }
    };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (ContentToEnumMap.TryGetValue(parameter.ToString(), out var enumValue))
            {
                return value.Equals(enumValue);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            if (value.Equals(true) && ContentToEnumMap.TryGetValue(parameter.ToString(), out var enumValue))
            {
                return enumValue;
            }

            return Binding.DoNothing;
        }
    }
}
