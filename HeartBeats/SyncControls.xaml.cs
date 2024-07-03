using HeartBeats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for SyncControls.xaml
    /// </summary>
    public partial class SyncControls : UserControl
    {
        private SyncPreferences _syncPreferences = new SyncPreferences();
        public SyncControls()
        {
            InitializeComponent();
            UpdateDataContext(_syncPreferences);
        }

        public void UpdateDataContext(SyncPreferences syncPreferences)
        {
            _syncPreferences = syncPreferences;
            DataContext = _syncPreferences;
        }

        private void SyncOptionChecked(object sender, RoutedEventArgs args)
        {
            RadioButton radio = sender as RadioButton;

            if (radio.Name == "Manual")
            {
                _syncPreferences.Manual = true;
            }
            else if (radio.Name == "Automatic")
            {
                _syncPreferences.Manual = false;
            }

            DataContext = _syncPreferences;
        }
    }
}
