using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HeartBeatsPlayerWP.Annotations;
using Microsoft.Phone.BackgroundAudio;

namespace HeartBeatsPlayerWP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Visibility SpinnerVisibility
        {
            get { return _spinnerVisibility; }
            set
            {
                if (value == _spinnerVisibility) return;
                _spinnerVisibility = value;
                OnPropertyChanged();
            }
        }

        public string PlayIconUri
        {
            get { return _playIconUri; }
            set
            {
                if (value == _playIconUri) return;
                _playIconUri = value;
                OnPropertyChanged();
            }
        }

        public string PlayText
        {
            get { return _playText; }
            set
            {
                if (value == _playText) return;
                _playText = value;
                OnPropertyChanged();
            }
        }

        private Visibility _spinnerVisibility;
        private string _playIconUri;
        private string _playText;

        public MainViewModel()
        {
            SpinnerVisibility = Visibility.Collapsed;
            PlayIconUri = "Assets/Media-Play.png";
            PlayText = "Play";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
