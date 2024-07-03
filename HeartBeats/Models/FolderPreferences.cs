using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HeartBeats.Models
{
    public class FolderPreferences : INotifyPropertyChanged
    {
        private bool _searchAllFolders = true;
        public bool SearchAllFolders
        {
            get
            {
                return _searchAllFolders;
            }

            set
            {
                _searchAllFolders = value;
                OnPropertyChanged(nameof(SearchAllFolders));
            }
        }

        private List<Folder> _folders = new List<Folder>();
        public List<Folder> Folders
        {
            get
            {
                return _folders;
            }
            set
            {
                _folders = value;
                OnPropertyChanged(nameof(Folders));
            }
        }

        private bool _isAnyChecked = false;

        public bool IsAnyChecked
        {
            get => _isAnyChecked;
            set
            {
                if (_isAnyChecked != value)
                {
                    _isAnyChecked = value;
                    OnPropertyChanged(nameof(IsAnyChecked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
