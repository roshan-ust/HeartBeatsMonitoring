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

namespace HeartBeats.Controls
{
    /// <summary>
    /// Interaction logic for CountdownTimer.xaml
    /// </summary>
    public partial class CountdownTimer : UserControl
    {
        public event EventHandler IntervalReached;

        public CountdownTimer()
        {
            InitializeComponent();
            DataContext = new Models.CountdownTimer();
            ((Models.CountdownTimer)DataContext).IntervalReached += IntervalReachedEvent;
        }

        private void IntervalReachedEvent(object sender, EventArgs e)
        {
            IntervalReached?.Invoke(sender, e);
        }

        public void UpdateTimer(int time)
        {
            ((Models.CountdownTimer)DataContext).UpdateCountdown(time);
        }

        public void StartTimer()
        {
            ((Models.CountdownTimer)DataContext).StartTimer();
        }

        public void StopTimer()
        {
            ((Models.CountdownTimer)DataContext).StopTimer();
        }
    }
}
