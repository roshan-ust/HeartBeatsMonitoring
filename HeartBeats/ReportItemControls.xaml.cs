using HeartBeats.Models;
using System.Windows;
using System.Windows.Controls;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for ReportItemControls.xaml
    /// </summary>
    public partial class ReportItemControls : UserControl
    {
        private ReportItemControlPreferences _reportItemControlPreferences = new ReportItemControlPreferences();
        public ReportItemControls()
        {
            InitializeComponent();
        }

        public void UpdateDataContext(ReportItemControlPreferences reportItemControlPreferences)
        {
            _reportItemControlPreferences = reportItemControlPreferences;
            DataContext = _reportItemControlPreferences;
        }

        private void ConsolidationOptionChecked(object sender, RoutedEventArgs args)
        {
            RadioButton radio = sender as RadioButton;

            if (radio.Name == "Consolidated")
            {
                _reportItemControlPreferences.Consolidated = true;
            }
            else if (radio.Name == "Individual")
            {
                _reportItemControlPreferences.Consolidated = false;
            }

            DataContext = _reportItemControlPreferences;
        }

        private void WarningMailOptionChanged(object sender, RoutedEventArgs args)
        {
            RadioButton radio = sender as RadioButton;

            if (radio.Name == "IgnoreWarning")
            {
                _reportItemControlPreferences.WarningPreference = Utils.Constants.WarningMailActions.Ignore;
            }
            else if (radio.Name == "ConsiderWarningAsError")
            {
                _reportItemControlPreferences.WarningPreference = Utils.Constants.WarningMailActions.ConsiderAsError;
            }

            DataContext = _reportItemControlPreferences;
        }
    }
}
