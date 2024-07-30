using HeartBeats.Models;
using System.Windows.Controls;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for FolderControls.xaml
    /// </summary>
    public partial class EmailControls : UserControl
    {
        private EmailPreferences _emailControls = new EmailPreferences();
        public EmailControls()
        {
            InitializeComponent();
        }

        public void UpdateDataContext(EmailPreferences emailControls)
        {
            _emailControls = emailControls;
            DataContext = _emailControls;
        }
    }
}
