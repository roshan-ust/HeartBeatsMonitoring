using System.ComponentModel;
using System.Windows;
using Outlook = Microsoft.Office.Interop.Outlook;
namespace HeartBeats.Models
{
    public class BasicProps : INotifyPropertyChanged
    {
        private string _name;
        private string _email;
        private bool _hasReportItems;
        private bool _loading;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public bool HasReportItems
        {
            get { return _hasReportItems; }
            set
            {
                _hasReportItems = value;
                OnPropertyChanged(nameof(HasReportItems));
            }
        }
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }
        public Outlook.Store DefaultStore { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
