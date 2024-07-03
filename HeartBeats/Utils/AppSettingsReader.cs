using System;
using System.ComponentModel;
using System.Linq;

namespace HeartBeats.Utils
{
    public static class AppSettingsReader
    {
        public static T[] ReadArray<T>(string key)
        {
            var settingValue = Properties.Settings.Default[key]?.ToString();
            if (settingValue == null)
                return new T[0];

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
                throw new InvalidOperationException($"No type converter for type {typeof(T)}");

            return settingValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => (T)converter.ConvertFromString(s.Trim()))
                               .ToArray();
        }

        public static T ReadValue<T>(string key)
        {
            var settingValue = Properties.Settings.Default[key]?.ToString();
            if (settingValue == null)
                return default;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
                throw new InvalidOperationException($"No type converter for type {typeof(T)}");

            return (T)converter.ConvertFromString(settingValue);
        }
    }
}
