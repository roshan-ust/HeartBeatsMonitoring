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
            Debug.WriteLine("Hour: " + hour);
            return new DateTime(dateTimeDetail.SelectedDate.Value.Year, dateTimeDetail.SelectedDate.Value.Month, dateTimeDetail.SelectedDate.Value.Day, hour, dateTimeDetail.SelectedMinute, dateTimeDetail.SelectedSecond);
        }

        public static DateTime ConvertTimeZone(DateTime dateTime, Constants.TimeZone sourceTimeZone = Constants.TimeZone.UTC, Constants.TimeZone destinationTimeZone = Constants.TimeZone.UTC)
        {
            return ConvertFromUTC(ConvertToUTC(dateTime, sourceTimeZone), destinationTimeZone);
        }

        private static DateTime ConvertToUTC(DateTime dateTime, Constants.TimeZone timeZone = Constants.TimeZone.UTC)
        {
            switch (timeZone)
            {
                case Constants.TimeZone.EST:
                    return ConvertESTtoUTC(dateTime);
                case Constants.TimeZone.IST:
                    return ConvertISTtoUTC(dateTime);
                default:
                    return dateTime;
            }
        }

        private static DateTime ConvertFromUTC(DateTime dateTime, Constants.TimeZone timeZone = Constants.TimeZone.UTC)
        {
            switch (timeZone)
            {
                case Constants.TimeZone.EST:
                    return ConvertUTCtoEST(dateTime);
                case Constants.TimeZone.IST:
                    return ConvertUTCtoIST(dateTime);
                default:
                    return dateTime;
            }
        }

        private static DateTime ConvertISTtoUTC(DateTime dateTimeIST)
        {
            // Define the EST time zone
            TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Indian Standard Time");

            return TimeZoneInfo.ConvertTimeToUtc(dateTimeIST, istTimeZone);
        }

        private static DateTime ConvertUTCtoIST(DateTime dateTimeUTC)
        {
            // Define the EST time zone
            TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Indian Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUTC, istTimeZone);
        }

        private static DateTime ConvertESTtoUTC(DateTime dateTimeEST)
        {
            // Define the EST time zone
            TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            // If DST should not be considered, adjust the time manually
            if (!AppSettingsReader.ReadValue<bool>("DayLights"))
            {
                // Check if the date falls within DST period
                if (estTimeZone.IsDaylightSavingTime(dateTimeEST))
                {
                    // Add one hour to adjust for DST
                    dateTimeEST = dateTimeEST.AddHours(1);
                }
            }

            // Convert the EST/EDT time to UTC
            return TimeZoneInfo.ConvertTimeToUtc(dateTimeEST, estTimeZone);
        }

        private static DateTime ConvertUTCtoEST(DateTime dateTimeUTC)
        {
            // Define the EST time zone
            TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            // Convert the UTC time to EST (considering DST)
            DateTime estDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUTC, estTimeZone);

            // If DST should not be considered, adjust the time manually
            if (!AppSettingsReader.ReadValue<bool>("DayLights"))
            {
                // Check if the date falls within DST period
                if (estTimeZone.IsDaylightSavingTime(estDateTime))
                {
                    // Subtract one hour to adjust for DST
                    estDateTime = estDateTime.AddHours(-1);
                }
            }

            return estDateTime;
        }

        private static int ConvertTo24Hour(DateTimeDetail dateTimeDetail)
        {
            int hour = dateTimeDetail.SelectedAmPm == "PM" && dateTimeDetail.SelectedHour != 12 ? dateTimeDetail.SelectedHour + 12 : dateTimeDetail.SelectedHour;
            hour = dateTimeDetail.SelectedAmPm == "AM" && dateTimeDetail.SelectedHour == 12 ? 0 : hour;
            return hour;
        }
    }
}
