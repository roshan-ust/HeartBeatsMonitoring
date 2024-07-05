using HeartBeats.Models;
using System;
using System.ComponentModel;
using System.Windows;
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
        }

        public void UpdateDataContext(FilterPreferences filterPreferences)
        {
            _filterPreferences = filterPreferences;
            DataContext = filterPreferences;
            ((FilterPreferences)DataContext).PropertyChanged -= UpdateFilterPreferences;
            ((FilterPreferences)DataContext).PropertyChanged += UpdateFilterPreferences;
            StartDateTimePicker.UpdateDataContext(Utils.DateTimeConverter.DateTimeToDateTimeDetail(_filterPreferences.StartDateTime));
            EndDateTimePicker.UpdateDataContext(Utils.DateTimeConverter.DateTimeToDateTimeDetail(_filterPreferences.EndDateTime));
        }

        private void UpdateFilterPreferences(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(FilterPreferences.StartDateTime))
            {
                UpdateStartDateTime(sender, _filterPreferences.StartDateTime);
            }
            else if (args.PropertyName == nameof(FilterPreferences.EndDateTime))
            {
                UpdateEndDateTime(sender, _filterPreferences.EndDateTime);
            }
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
