using HeartBeats.Models;
using System;
using System.Diagnostics;

namespace HeartBeats.Utils
{
    public static class DateTimeConverter
    {
        public static DateTimeDetail DateTimeToDateTimeDetail(DateTime dateTime)
        {
            var dateTimeDetail = new DateTimeDetail();
            dateTimeDetail.SelectedDate = dateTime.Date;
            dateTimeDetail.SelectedHour = dateTime.Hour % 12 == 0 ? 12 : dateTime.Hour % 12;
            dateTimeDetail.SelectedMinute = dateTime.Minute;
            dateTimeDetail.SelectedSecond = dateTime.Second;
            dateTimeDetail.SelectedAmPm = dateTime.Hour >= 12 ? "PM" : "AM";
            Debug.WriteLine(dateTimeDetail);
            Debug.WriteLine(dateTime);
            return dateTimeDetail;
        }
        public static DateTime DateTimeDetailToDateTime(DateTimeDetail dateTimeDetail)
        {
            int hour = ConvertTo24Hour(dateTimeDetail);
            return new DateTime(dateTimeDetail.SelectedDate.Value.Year, dateTimeDetail.SelectedDate.Value.Month, dateTimeDetail.SelectedDate.Value.Day, hour, dateTimeDetail.SelectedMinute, dateTimeDetail.SelectedSecond);
        }

        public static DateTime ConvertTimeZone(DateTime dateTime, string sourceTimeZone = Constants.TimeZone.UTC, string destinationTimeZone = Constants.TimeZone.UTC)
        {
            return ConvertFromUTC(ConvertToUTC(dateTime, sourceTimeZone), destinationTimeZone);
        }

        private static DateTime ConvertToUTC(DateTime dateTime, string timeZoneId = Constants.TimeZone.UTC)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // If DST should not be considered, adjust the time manually
            if (timeZoneId.Equals(Constants.TimeZone.EST) && !AppSettingsReader.ReadValue<bool>("DayLights"))
            {
                // Check if the date falls within DST period
                if (timeZone.IsDaylightSavingTime(dateTime))
                {
                    // Add one hour to adjust for DST
                    dateTime = dateTime.AddHours(1);
                }
            }

            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        private static DateTime ConvertFromUTC(DateTime dateTime, string timeZoneId = Constants.TimeZone.UTC)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            DateTime convertedTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);

            // If DST should not be considered, adjust the time manually
            if (timeZoneId.Equals(Constants.TimeZone.EST) && !AppSettingsReader.ReadValue<bool>("DayLights"))
            {
                // Check if the date falls within DST period
                if (timeZone.IsDaylightSavingTime(convertedTime))
                {
                    // Add one hour to adjust for DST
                    convertedTime = convertedTime.AddHours(-1);
                }
            }

            return convertedTime;
        }

        private static int ConvertTo24Hour(DateTimeDetail dateTimeDetail)
        {
            int hour = dateTimeDetail.SelectedAmPm == "PM" && dateTimeDetail.SelectedHour != 12 ? dateTimeDetail.SelectedHour + 12 : dateTimeDetail.SelectedHour;
            hour = dateTimeDetail.SelectedAmPm == "AM" && dateTimeDetail.SelectedHour == 12 ? 0 : hour;
            return hour;
        }
    }
}
