using HeartBeats.Utils;
using System.ComponentModel;

namespace HeartBeats.Models
{
    public class ReportItemControlPreferences: INotifyPropertyChanged
    {
        private bool _consolidated = true;

        public bool Consolidated
        {
            get { return _consolidated; }
            set
            {
                if (_consolidated != value)
                {
                    _consolidated = value;
                    OnPropertyChanged(nameof(Consolidated)); 
                }
            }
        }

        private Constants.WarningMailActions _warningPreference = Constants.WarningMailActions.Ignore;

        public Constants.WarningMailActions WarningPreference
        {
            get { return _warningPreference; }
            set
            {
                if (_warningPreference != value)
                {
                    _warningPreference = value;
                    OnPropertyChanged(nameof(WarningPreference)); 
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
