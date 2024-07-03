using System;

namespace HeartBeats.Utils
{
    public static class Constants
    {
        public static readonly int ErrorCriticalThreshold;
        public static readonly TimeSpan TimeZoneDifference;

        public static readonly string[] ErrorsTobeMonitored;

        public static readonly string[] ExcludedFolders;

        public const string ErrorMailStatus = "Error";
        public const string WarningMailStatus = "Warning";
        public const string SuccessMailStatus = "Working Fine";

        public enum WarningMailActions
        {
            Ignore,
            ConsiderAsError
        }

        static Constants()
        {
            ErrorCriticalThreshold = AppSettingsReader.ReadValue<int>(nameof(ErrorCriticalThreshold));
            ErrorsTobeMonitored = AppSettingsReader.ReadArray<string>(nameof(ErrorsTobeMonitored));
            TimeZoneDifference = AppSettingsReader.ReadValue<TimeSpan>(nameof(TimeZoneDifference));
            if (AppSettingsReader.ReadValue<bool>("DayLights") == true)
            {
                TimeZoneDifference = TimeZoneDifference.Add(new TimeSpan(-1, 0, 0));
            }
            ExcludedFolders = AppSettingsReader.ReadArray<string>(nameof(ExcludedFolders));
        }
    }
}
