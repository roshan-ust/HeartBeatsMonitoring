using HeartBeats.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HeartBeats.Utils
{
    public class DateTimeDateTimeDetailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTimeValue)
            {
                if(parameter?.ToString() == "string")
                {
                    return dateTimeValue.ToString();
                }
                return DateTimeConverter.DateTimeToDateTimeDetail(dateTimeValue);
            }
            else if( value is DateTimeDetail dateTimeDetail)
            {
                return DateTimeConverter.DateTimeDetailToDateTime(dateTimeDetail);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTimeValue)
            {
                if (parameter?.ToString() == "string")
                {
                    return dateTimeValue.ToString();
                }
                return DateTimeConverter.DateTimeToDateTimeDetail(dateTimeValue);
            }
            else if (value is DateTimeDetail dateTimeDetail)
            {
                return DateTimeConverter.DateTimeDetailToDateTime(dateTimeDetail);
            }

            return null;
        }
    }
}
