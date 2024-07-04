using System;
using System.ComponentModel;

namespace HeartBeats.Models
{
    public class DateTimeDetail : INotifyPropertyChanged
    {
        private DateTime? _selectedDate;
        private int _selectedHour;
        private int _selectedMinute;
        private int _selectedSecond;
        private string _selectedAmPm;

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        public int SelectedHour
        {
            get => _selectedHour;
            set
            {
                if (_selectedHour != value)
                {
                    _selectedHour = value;
                    OnPropertyChanged(nameof(SelectedHour));
                }
            }
        }

        public int SelectedMinute
        {
            get => _selectedMinute;
            set
            {
                if (_selectedMinute != value)
                {
                    _selectedMinute = value;
                    OnPropertyChanged(nameof(SelectedMinute));
                }
            }
        }

        public int SelectedSecond
        {
            get => _selectedSecond;
            set
            {
                if (_selectedSecond != value)
                {
                    _selectedSecond = value;
                    OnPropertyChanged(nameof(SelectedSecond));
                }
            }
        }

        public string SelectedAmPm
        {
            get => _selectedAmPm;
            set
            {
                if (_selectedAmPm != value)
                {
                    _selectedAmPm = value;
                    OnPropertyChanged(nameof(SelectedAmPm));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
