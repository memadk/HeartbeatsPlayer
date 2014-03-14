using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AudioSharedLibrary;
using HeartBeatsPlayerWP.ViewModels;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;

namespace HeartBeatsPlayerWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        readonly MainViewModel _model= new MainViewModel();
        DispatcherTimer playTimer;
        EventHandler playTimerTickEventHandler;
        EventHandler playStateChangedEventHandler;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            playTimer = new DispatcherTimer();
            playTimer.Interval = TimeSpan.FromMilliseconds(1000);
            playTimerTickEventHandler = new EventHandler(playTimer_Tick);
            playStateChangedEventHandler = new EventHandler(Instance_PlayStateChanged);
            this.DataContext = _model;

            //Shows the trial reminder message, according to the settings of the TrialReminder.
            (App.Current as App).trialReminder.Notify();

            //Shows the rate reminder message, according to the settings of the RateReminder.
            (App.Current as App).rateReminder.Notify();
            BackgroundAudioPlayer.Instance.Play();
        }

        /// <summary>
        /// Navigates to about page.
        /// </summary>
        private void GoToAbout(object sender, GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.RelativeOrAbsolute));
        }

        private void UIElement_OnTap(object sender, GestureEventArgs e)
        {
            if (BackgroundAudioPlayer.Instance.PlayerState != PlayState.Playing)
            {
                //BackgroundAudioPlayer.Instance.SkipNext();
                BackgroundAudioPlayer.Instance.Play();
            }
            else
            {
                BackgroundAudioPlayer.Instance.Stop();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            FlurryWP8SDK.Api.LogPageView();
            // register event handlers and start timer 
            BackgroundAudioPlayer.Instance.PlayStateChanged += playStateChangedEventHandler;
            playTimer.Tick += playTimerTickEventHandler;
            playTimer.Start();

            // force a UI refresh of the current state of the background audio 
            Instance_PlayStateChanged(this, null);            
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // stop timer and unregister event handlers 
            playTimer.Stop();
            playTimer.Tick -= playTimerTickEventHandler;
            BackgroundAudioPlayer.Instance.PlayStateChanged -= playStateChangedEventHandler;
            base.OnNavigatedFrom(e);
        }

        void playTimer_Tick(object sender, EventArgs e)
        {
            // check for errors 
            string errorString = BackgroundErrorNotifier.GetError();
            if (errorString != null)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(errorString, "Audio error", MessageBoxButton.OK);
                }
                else
                {
                    FlurryWP8SDK.Api.LogEvent("Audio error: " + errorString);
                }
                _model.SpinnerVisibility = Visibility.Collapsed;
            }

            // update UI if audio is active 
            var player = BackgroundAudioPlayer.Instance;
            AudioTrack currentTrack = player.Track;
            PlayState bapState = player.PlayerState;

            if (bapState == PlayState.Unknown)
            {
                txtState.Text = "Loading";
                FlurryWP8SDK.Api.LogEvent("Loading");
            }
            else
            {
                txtState.Text = bapState.ToString();
            }

            if ((bapState != PlayState.Unknown)
                && (currentTrack != null))
            {
                TimeSpan position = player.Position;
                txtPosition.Text = position.ToString(@"hh\:mm\:ss");
                if ((bapState == PlayState.Playing) &&
                    (currentTrack.Tag == "Buffering"))
                {
                    _model.SpinnerVisibility = Visibility.Visible;
                    txtState.Text = "Buffering";
                    FlurryWP8SDK.Api.LogEvent("Buffering");
                }
                else
                {
                    _model.SpinnerVisibility = Visibility.Collapsed;
                }
            }
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            var player = BackgroundAudioPlayer.Instance;
            PlayState bapState = player.PlayerState;
           
            switch (bapState)
            {
                case PlayState.Playing:
                    _model.PlayIconUri = "Assets/Media-Pause.png";
                    break;

                case PlayState.Stopped:
                    _model.PlayIconUri = "Assets/Media-Play.png";
                    player.Track = null;
                    break;

                default:
                    _model.PlayIconUri = "Assets/Media-Play.png";
                    break;
            }
        }
    }
}
