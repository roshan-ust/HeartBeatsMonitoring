using HeartBeats.Models;
using HeartBeats.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for CriticalErrorWindow.xaml
    /// </summary>
    public partial class CriticalErrorWindow : Window
    {
        private MediaPlayer _player;
        public CriticalErrorWindow(IEnumerable<NotificationItem> criticalItems)
        {
            if (AppSettingsReader.ReadValue<bool>("PlayNotificationSound"))
            {
                PlaySound();
            }

            InitializeComponent();
            DataContext = criticalItems;
        }

        private void Close(object sender, RoutedEventArgs args)
        {
            _player?.Close();
            Close();
        }

        private void PlaySound()
        {
            _player = new MediaPlayer();
            string relativeFilePath = "Resources/CriticalError.wav";
            _player.Open(new Uri(relativeFilePath, UriKind.Relative));

            _player.MediaOpened += (s, e) => { Debug.WriteLine("Media opened successfully."); };
            _player.MediaFailed += (s, e) => {
                Debug.WriteLine("Media failed to open."); };
            _player.MediaEnded += (s, e) => { _player.Close(); };

            _player.Play();
        }

    }
}
