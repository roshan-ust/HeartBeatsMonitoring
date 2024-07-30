using HeartBeats.Utils;
using System;
using System.ComponentModel;

namespace HeartBeats.Models
{
    public class FilterPreferences : INotifyPropertyChanged
    {
        private string _timeZone = Constants.TimeZone.EST;
        private DateTime _startDateTime = Utils.DateTimeConverter.ConvertTimeZone(DateTime.UtcNow, Constants.TimeZone.UTC, Constants.TimeZone.EST).Date;
        private DateTime _endDateTime = Utils.DateTimeConverter.ConvertTimeZone(DateTime.UtcNow, Constants.TimeZone.UTC, Constants.TimeZone.EST);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var dateAutoCorrect = false;
            if(propertyName == nameof(StartDateTime))
            {
                if(StartDateTime > EndDateTime)
                {
                    EndDateTime = StartDateTime.Date.AddDays(1).AddSeconds(-1);
                    dateAutoCorrect = true;
                }
            }
            else if(propertyName == nameof(EndDateTime))
            {
                if(EndDateTime < StartDateTime)
                {
                    StartDateTime = EndDateTime.AddDays(-1).Date;
                    dateAutoCorrect = true;
                }
            }

            if (dateAutoCorrect)
            {
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string TimeZone {
            get => _timeZone;
            set
            {
                if(_timeZone != value)
                {
                    UpdateSelectedTimeByZone(_timeZone, value);
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

        private void UpdateSelectedTimeByZone(string currentZone, string newZone)
        {
            StartDateTime = Utils.DateTimeConverter.ConvertTimeZone(StartDateTime, currentZone, newZone);
            var currDateTime = Utils.DateTimeConverter.ConvertTimeZone(DateTime.UtcNow, Constants.TimeZone.UTC, newZone);
            var requestedZoneDateTime = Utils.DateTimeConverter.ConvertTimeZone(EndDateTime, currentZone, newZone);
            EndDateTime = requestedZoneDateTime > currDateTime ? currDateTime : requestedZoneDateTime;
        }
    }
}
