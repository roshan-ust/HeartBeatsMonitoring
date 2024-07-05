using HeartBeats.Models;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace HeartBeats.Controls
{
    public partial class DateTimePickerControl : UserControl
    {
        public DateTimeDetail _dateTimeDetail { get; set; }

        public event EventHandler<DateTime> DateTimePicked;
        public DateTimePickerControl()
        {
            InitializeComponent();
        }

        public void UpdateDataContext(DateTimeDetail dateTimeDetail)
        {
            _dateTimeDetail = dateTimeDetail;
            DataContext = _dateTimeDetail;
            ((DateTimeDetail)DataContext).PropertyChanged += NotifyDateTimePicked;
        }

        private void NotifyDateTimePicked(object sender, PropertyChangedEventArgs e)
        {
            var updatedDate = Utils.DateTimeConverter.DateTimeDetailToDateTime(sender as DateTimeDetail);
            UpdateDataContext(sender as DateTimeDetail);
            DateTimePicked?.Invoke(this, updatedDate);
        }
    }
}
