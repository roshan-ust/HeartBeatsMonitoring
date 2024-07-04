using System.ComponentModel;

namespace HeartBeats.Models
{
    public class NumericValue: INotifyPropertyChanged
    {
        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                if(_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
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
