using HeartBeats.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HeartBeats.Controls
{
    public partial class DateTimePickerControl : UserControl
    {
        public DateTimeDetail ViewModel { get; set; }

        public event EventHandler<DateTime> DateTimePicked;
        public DateTimePickerControl()
        {
            InitializeComponent();
            ((DateTimeDetail)DataContext).PropertyChanged += NotifyDateTimePicked;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyPropertyChanged oldViewModel)
            {
                oldViewModel.PropertyChanged -= NotifyDateTimePicked;
            }

            if (e.NewValue is INotifyPropertyChanged newViewModel)
            {
                newViewModel.PropertyChanged += NotifyDateTimePicked;
            }
        }

        private void NotifyDateTimePicked(object sender, PropertyChangedEventArgs e)
        {
            var updatedDate = Utils.DateTimeConverter.DateTimeDetailToDateTime(sender as DateTimeDetail);
            DataContext = sender as DateTimeDetail;
            DateTimePicked?.Invoke(this, updatedDate);
        }
    }
}
