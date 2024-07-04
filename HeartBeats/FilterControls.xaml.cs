using HeartBeats.Models;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for FilterControls.xaml
    /// </summary>
    public partial class FilterControls : UserControl
    {
        private FilterPreferences _filterPreferences = new FilterPreferences();

        public FilterControls()
        {
            InitializeComponent();
            UpdateDataContext(_filterPreferences);
            StartDateTimePicker.DateTimePicked += UpdateStartDateTime;
            EndDateTimePicker.DateTimePicked += UpdateEndDateTime;
        }

        public void UpdateDataContext(FilterPreferences filterPreferences)
        {
            _filterPreferences = filterPreferences;
            DataContext = filterPreferences;
        }

        private void UpdateStartDateTime(object sender, DateTime dateTime)
        {
            _filterPreferences.StartDateTime = dateTime;
            UpdateDataContext(_filterPreferences);
        }

        private void UpdateEndDateTime(object sender, DateTime dateTime)
        {
            _filterPreferences.EndDateTime = dateTime;
            UpdateDataContext(_filterPreferences);
        }
    }
}
