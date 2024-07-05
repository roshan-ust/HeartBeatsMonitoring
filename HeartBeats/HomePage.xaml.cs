using HeartBeats.Models;
using HeartBeats.Utils;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = SystemIcons.Warning, // You can set your own icon here
            Visible = true
        };
        private DispatcherTimer _timer;
        private HeartBeatReport _heartBeatReport = new HeartBeatReport();
        private BasicProps _user = new BasicProps();
        private FolderPreferences _folderPreferences = new FolderPreferences();
        private ReportItemControlPreferences _reportItemControlPreferences;
        private SyncPreferences _syncPreferences = new SyncPreferences();
        private FilterPreferences _filterPreferences = new FilterPreferences();
        private ExportControls _exportControls = new ExportControls();

        public HomePage(BasicProps user)
        {
            InitializeComponent();
            _user = user;
            DataContext = _user;
            _reportItemControlPreferences = _heartBeatReport.ReportItemControlPreferences;
            FolderControls.ShowAllFoldersChange += ShowAllFoldersStatusChange;
            FolderControls.UpdateDataContext(_folderPreferences);
            _heartBeatReport.PropertyChanged += HeartBeatReportDataChange;
            ReportItemControls.UpdateDataContext(_reportItemControlPreferences);
            SyncControls.UpdateDataContext(_syncPreferences);
            _filterPreferences = _heartBeatReport.FilterPreferences;
            FilterControls.UpdateDataContext(_filterPreferences);
            ExportControls.UpdateDataContext(_exportControls);
        }

        private void ShowHideLoader(bool show)
        {

            // Force an immediate UI update
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ((BasicProps)DataContext).Loading = show;
            }, DispatcherPriority.Render);
        }

        private void HeartBeatReportDataChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_heartBeatReport.ReportItems))
            {
                ShowHideLoader(false);
                HeartBeatsReportUI.DataContext = _heartBeatReport;
                ((BasicProps)DataContext).HasReportItems = _heartBeatReport.ReportItems.Count > 0;
                if (_heartBeatReport.ReportItems.Count > 0)
                {
                    NotifyCriticalErrors();
                }
            }
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander expandedExpander = sender as Expander;
            CollapseOtherExpanders(MainGrid, expandedExpander);
        }

        private void CollapseOtherExpanders(DependencyObject parent, Expander expandedExpander)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Expander expander && expander != expandedExpander)
                {
                    expander.IsExpanded = false;
                }
                else
                {
                    CollapseOtherExpanders(child, expandedExpander);
                }
            }
        }

        private void NotifyCriticalErrors()
        {
            var notifications = new List<NotificationItem>();
            int count = 0;

            foreach (string error in Constants.ErrorsTobeMonitored)
            {
                count = 0;
                if (_heartBeatReport.ReportItemControlPreferences.Consolidated)
                {
                    count = _heartBeatReport.ReportItems.FirstOrDefault(item => item.Name.Equals(error))?.Count ?? 0;
                }
                else
                {
                    count = _heartBeatReport.ReportItems.Where(item => item.Name.Equals(error) && item.Status == Constants.ErrorMailStatus).Count();
                }

                if (count > AppSettingsReader.ReadValue<int>("ErrorCriticalThreshold"))
                {
                    notifications.Add(new NotificationItem() { Name = error, Count = count });
                }
            }

            if (notifications.Count > 0)
            {
                CriticalErrorWindow notificationWindow = new CriticalErrorWindow(notifications);
                notificationWindow.Show();
            }
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }

        private void IntervalReachedEvent(object sender, EventArgs args)
        {
            FetchHeartBeatMails();
        }

        private void FetchHeartBeatMails(object sender, RoutedEventArgs args)
        {

            if (!_syncPreferences.Manual)
            {
                SyncControls.IntervalReached += IntervalReachedEvent;
            }

            FetchHeartBeatMails();
        }

        private async void FetchHeartBeatMails()
        {
            var utcEndDateTime = Utils.DateTimeConverter.ConvertTimeZone(_filterPreferences.EndDateTime, _filterPreferences.TimeZone);
            if (utcEndDateTime < DateTime.UtcNow)
            {
                _filterPreferences.EndDateTime = Utils.DateTimeConverter.ConvertTimeZone(DateTime.UtcNow.AddMinutes(_syncPreferences.Interval * 2), Constants.TimeZone.UTC, _filterPreferences.TimeZone);
                FilterControls.UpdateDataContext(_filterPreferences);
            }
            ShowHideLoader(true);
            _heartBeatReport.Mails = await Task.Run(() => GetHeartBeatMails());
            if (!_syncPreferences.Manual)
            {
                SyncControls.StartTimer();
            }
        }

        private async void ExportReport(object sender, RoutedEventArgs args)
        {
            ShowHideLoader(true);
            await Task.Run(() => WordDocumentExporter.ExportHeartBeatToWord(_heartBeatReport.ReportItems, _exportControls.ExportFilePath));
            ShowHideLoader(false);
        }

        private void ClearReports(object sender, RoutedEventArgs args)
        {
            _heartBeatReport.ClearReport();
            HeartBeatsReportUI.DataContext = _heartBeatReport;
            ((BasicProps)DataContext).HasReportItems = false;
        }

        private void ShowAllFoldersStatusChange(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox?.IsChecked == false && _folderPreferences.Folders.Count == 0)
            {
                ShowHideLoader(true);
                UpdateFolderPreferencesFolderList();
            }

            _folderPreferences.Folders.ForEach(preference => preference.Selected = false);
            _folderPreferences.IsAnyChecked = false;
        }

        private async void UpdateFolderPreferencesFolderList()
        {
            _folderPreferences.Folders = await Task.Run(() => FindAllFolderPaths().Select(folderPath => new Models.Folder(folderPath, false)).ToList());
            FolderControls.UpdateDataContext(_folderPreferences);
            ShowHideLoader(false);
        }

        private List<string> FindAllFolderPaths()
        {
            List<string> folders = new List<string>();
            if (_user.DefaultStore != null)
            {
                MAPIFolder rootFolder = _user.DefaultStore.GetRootFolder();
                TraverseFolders(rootFolder, folders, "");
            }

            return folders;
        }

        private void TraverseFolders(MAPIFolder folder, List<string> folderPaths, string currentPath)
        {
            if (!Constants.ExcludedFolders.Contains(folder.Name))
            {
                // Construct current folder path
                string folderPath = folder.FullFolderPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim()).ToList().Count > 1 ? currentPath.Length > 0 ? $"{currentPath}\\{folder.Name}" : folder.Name : "";

                if (!string.IsNullOrEmpty(folderPath))
                {
                    folderPaths.Add(folderPath);
                }

                // Recursively traverse subfolders
                foreach (MAPIFolder subFolder in folder.Folders)
                {
                    if (!Constants.ExcludedFolders.Contains(subFolder.Name))
                    {
                        // Check if the current subfolder has any subfolders
                        if (subFolder.Folders.Count > 0)
                        {
                            // If yes, continue traversing recursively with updated current path
                            TraverseFolders(subFolder, folderPaths, folderPath);
                        }
                        else
                        {
                            // If no further subfolders, add the current subfolder path directly
                            folderPaths.Add(string.IsNullOrEmpty(folderPath) ? subFolder.Name : $"{folderPath}\\{subFolder.Name}");
                        }
                    }
                }
            }
        }

        private MAPIFolder GetFolderByPath(MAPIFolder parentFolder, string[] folderPathParts)
        {
            MAPIFolder folder = null;

            if (folderPathParts != null)
            {
                if (folderPathParts.Length > 0)
                {
                    folder = parentFolder.Folders[folderPathParts.First()];

                    if (folderPathParts.Length > 1)
                    {
                        return GetFolderByPath(folder, folderPathParts.Skip(1).ToArray());
                    }
                }
            }

            return folder;
        }

        private List<MailDetail> GetHeartBeatMails()
        {
            List<MailDetail> filteredItems = new List<MailDetail>();
            var rootFolder = _user.DefaultStore?.GetRootFolder();
            if (rootFolder != null)
            {
                LookupHeartBeatMails(rootFolder, filteredItems, _folderPreferences.Folders.Where(folder => folder.Selected).ToList());
            }

            return filteredItems.OrderBy(item => item.ReceivedTime).ToList();
        }

        private void LookupHeartBeatMails(MAPIFolder folder, List<MailDetail> filteredItems, List<Models.Folder> selectedFolders)
        {
            // Check if the current folder is in the selected list or no selected folders provided
            if (selectedFolders.Count == 0 || selectedFolders.Any(selectedFolder => folder.FolderPath.EndsWith(selectedFolder.Name)))
            {
                // Check if the current folder is not in the excluded list
                if (!Constants.ExcludedFolders.Contains(folder.Name))
                {
                    string filter = $"@SQL=\"urn:schemas:httpmail:subject\" LIKE '%Heartbeat Application%' AND \"urn:schemas:httpmail:datereceived\" >= '{Utils.DateTimeConverter.ConvertTimeZone(_filterPreferences.StartDateTime, _filterPreferences.TimeZone):yyyy-MM-dd HH:mm:ss}' AND \"urn:schemas:httpmail:datereceived\" <= '{Utils.DateTimeConverter.ConvertTimeZone(_filterPreferences.EndDateTime, _filterPreferences.TimeZone):yyyy-MM-dd HH:mm:ss}'";

                    Items items = folder.Items.Restrict(filter);
                    if (items.Count > 0)
                    {
                        items.Sort("[ReceivedTime]");
                    }

                    foreach (object item in items)
                    {
                        if (item is MailItem mailItem)
                        {
                            filteredItems.Add(new MailDetail()
                            {
                                ReceivedTime = mailItem.ReceivedTime,
                                Subject = mailItem.Subject,
                                Body = mailItem.Body
                            });
                        }
                    }
                }
            }

            // Recursively traverse subfolders
            foreach (MAPIFolder subFolder in folder.Folders)
            {
                LookupHeartBeatMails(subFolder, filteredItems, selectedFolders);
            }
        }
    }
}
