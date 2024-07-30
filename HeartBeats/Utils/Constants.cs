using System;

namespace HeartBeats.Utils
{
    public static class Constants
    {
        public static readonly int ErrorCriticalThreshold;

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

        public static class TimeZone
        {
            public const string EST = "Eastern Standard Time";
            public const string UTC = "UTC";
            public const string IST = "India Standard Time";
        }

        static Constants()
        {
            ErrorCriticalThreshold = AppSettingsReader.ReadValue<int>(nameof(ErrorCriticalThreshold));
            ErrorsTobeMonitored = AppSettingsReader.ReadArray<string>(nameof(ErrorsTobeMonitored));
            ExcludedFolders = AppSettingsReader.ReadArray<string>(nameof(ExcludedFolders));
        }
    }
}
