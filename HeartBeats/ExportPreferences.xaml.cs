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
    /// Interaction logic for ExportPreferences.xaml
    /// </summary>
    public partial class ExportPreferences : UserControl
    {
        public ExportPreferences()
        {
            InitializeComponent();
        }

        public void UpdateDataContext(ExportControls exportControls)
        {
            DataContext = exportControls;
        }
    }
}
