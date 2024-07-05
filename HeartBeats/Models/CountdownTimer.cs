using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HeartBeats.Models
{
    public class CountdownTimer : INotifyPropertyChanged
    {
        private DispatcherTimer timer;
        private TimeSpan _countdown;
        private TimeSpan countdown;
        private string timerDisplay;

        public string TimerDisplay
        {
            get { return timerDisplay; }
            set
            {
                timerDisplay = value;
                OnPropertyChanged(nameof(TimerDisplay));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler IntervalReached;

        public void UpdateCountdown(int newCountdown)
        {
            _countdown = TimeSpan.FromMinutes(newCountdown);
            StopTimer();
        }

        public void StartTimer()
        {
            countdown = _countdown;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void StopTimer()
        {
            timer?.Stop();
            TimerDisplay = _countdown.ToString(@"hh\:mm\:ss");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (countdown.TotalSeconds > 0)
            {
                countdown = countdown.Subtract(TimeSpan.FromSeconds(1));
                TimerDisplay = countdown.ToString(@"hh\:mm\:ss");
            }
            else
            {
                StopTimer();
                IntervalReached?.Invoke(sender, new EventArgs());
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
