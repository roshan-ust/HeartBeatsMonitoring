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
    /// Interaction logic for FolderControls.xaml
    /// </summary>
    public partial class FolderControls : UserControl
    {
        private FolderPreferences _folderControls;
        public FolderControls()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler ShowAllFoldersChange;

        public void UpdateDataContext(FolderPreferences folderControls)
        {
            _folderControls = folderControls;
            DataContext = _folderControls;
        }

        private void CheckboxChanged(object sender, RoutedEventArgs args)
        {
            ShowAllFoldersChange?.Invoke(sender, args);
        }

        private void FolderCheckboxChanged(object sender, RoutedEventArgs args)
        {
            _folderControls.IsAnyChecked = _folderControls.Folders.Any(folder => folder.Selected);
            DataContext = _folderControls;
        }
    }
}
