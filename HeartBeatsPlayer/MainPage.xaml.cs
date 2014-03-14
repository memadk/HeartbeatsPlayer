using System;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.PlayTo;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace HeartBeatsPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SystemMediaTransportControls systemControls;

        public MainPage()
        {
            this.InitializeComponent();
            
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += SystemControls_ButtonPressed;

            // Register to handle the following system transpot control buttons.
            systemControls.IsPlayEnabled = true;
            systemControls.IsPauseEnabled = true;
            musicPlayer.Stretch = Stretch.Fill;
            musicPlayer.AudioDeviceType = AudioDeviceType.Multimedia;
            musicPlayer.RealTimePlayback = true;
        }

        void MusicPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (musicPlayer.CurrentState)
            {
                case MediaElementState.Playing:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    PlayPauseButton.Source = new BitmapImage { UriSource = new Uri(PlayPauseButton.BaseUri, "Assets/Media-Pause.png") };
                    UpdateSongInfoManually();
                    break;
                case MediaElementState.Paused:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    PlayPauseButton.Source = new BitmapImage { UriSource = new Uri(PlayPauseButton.BaseUri, "Assets/Media-Play.png") };
                    break;
                case MediaElementState.Stopped:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    PlayPauseButton.Source = new BitmapImage { UriSource = new Uri(PlayPauseButton.BaseUri, "Assets/Media-Play.png") };
                    break;
                case MediaElementState.Closed:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                default:
                    break;
            }
        }

        void SystemControls_ButtonPressed(SystemMediaTransportControls sender,
            SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayMedia();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PauseMedia();
                    break;
                default:
                    break;
            }
        }

        async void PlayMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => musicPlayer.Play());
        }

        async void PauseMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => musicPlayer.Stop());
        }

        void UpdateSongInfoManually()
        {
            // Get the updater.
            SystemMediaTransportControlsDisplayUpdater updater = systemControls.DisplayUpdater;

            // Music metadata.
            updater.Type = MediaPlaybackType.Music;
            updater.MusicProperties.Artist = "Le Gammeltoft";
            updater.MusicProperties.Title = "HeartBeats Radio";

            // Set the album art thumbnail.
            // RandomAccessStreamReference is defined in Windows.Storage.Streams
            updater.Thumbnail =
               RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/logo100.png"));

            // Update the system media transport controls.
            updater.Update();
        }

        private void PlayPauseButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (musicPlayer.CurrentState == MediaElementState.Playing)
            {
                PauseMedia();
            }
            else
            {
                PlayMedia();
            }
        }
    }
}
