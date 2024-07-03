using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeats.Models
{
    public class SyncPreferences: INotifyPropertyChanged
    {
        private bool _manual = true;
        public bool Manual
        {
            get { return _manual; }
            set
            {
                _manual = value;
                OnPropertyChanged(nameof(Manual));
            }
        }

        private int _interval = 5;
        public int Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }

        public int[] Intervals { get; } = { 5, 10, 15, 30, 60, 120 };

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
