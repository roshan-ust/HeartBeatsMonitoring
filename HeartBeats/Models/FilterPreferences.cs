using HeartBeats.Utils;
using System;
using System.ComponentModel;

namespace HeartBeats.Models
{
    public class FilterPreferences : INotifyPropertyChanged
    {
        private Constants.TimeZone _timeZone = Constants.TimeZone.EST;
        private DateTime _startDateTime = Utils.DateTimeConverter.ConvertTimeZone(DateTime.UtcNow, Constants.TimeZone.UTC, Constants.TimeZone.EST).Date;
        private DateTime _endDateTime = Utils.DateTimeConverter.ConvertTimeZone(DateTime.UtcNow, Constants.TimeZone.UTC, Constants.TimeZone.EST).Date.AddDays(1).AddSeconds(-1);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Constants.TimeZone TimeZone {
            get => _timeZone;
            set
            {
                if(_timeZone != value)
                {
                    _timeZone = value;
                    OnPropertyChanged(nameof(TimeZone));
                }
            }
        }

        public DateTime StartDateTime
        {
            get => _startDateTime;
            set
            {
                if (_startDateTime != value)
                {
                    _startDateTime = value;
                    OnPropertyChanged(nameof(StartDateTime));
                }
            }
        }

        public DateTime EndDateTime
        {
            get => _endDateTime;
            set
            {
                if (_endDateTime != value)
                {
                    _endDateTime = value;
                    OnPropertyChanged(nameof(EndDateTime));
                }
            }
        }
    }
}
