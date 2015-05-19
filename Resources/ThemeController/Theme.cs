using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ThemeControllerHelper
{ 
    class Theme :INotifyPropertyChanged
    {

        private void NotifyPropertyChanged(String p)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private SolidColorBrush _Accent ;
        private ImageSource _newplaylistimage;
        private ImageSource _nexttrackimage;
        private ImageSource _previoustrackimage;
        private ImageSource _shuffleimage;
        private ImageSource _listimage;
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Color GreyColor { get; set; }
        public Color SecondBackgroundColor { get; set; }
        public SolidColorBrush Accent {
            get { return this._Accent; }
            set { if(this._Accent == value) { return; } this._Accent = value; this.NotifyPropertyChanged("Accent"); }
        }
        public SolidColorBrush Background { get { return new SolidColorBrush(this.BackgroundColor); } }
        public SolidColorBrush Foreground { get { return new SolidColorBrush(this.ForegroundColor); } }
        public SolidColorBrush Grey { get { return new SolidColorBrush(this.GreyColor); } }
        public SolidColorBrush SecondBackground { get { return new SolidColorBrush(this.SecondBackgroundColor);} }
        public ImageSource listimage
        {
            get { return this._listimage; }
            set
            {
                if (this._listimage == value) { return; }
                this._listimage = value;
                this.NotifyPropertyChanged("listimage");
            }
        }
        public ImageSource newplaylistimage
        {
            get { return this._newplaylistimage; }
            set
            {
                if (this._newplaylistimage == value) { return; }
                this._newplaylistimage = value;
                this.NotifyPropertyChanged("newplaylistimage");
            }
        }
        public ImageSource nexttrackimage
        {
            get { return this._nexttrackimage; }
            set
            {
                if (this._nexttrackimage == value) { return; }
                this._nexttrackimage = value;
                this.NotifyPropertyChanged("nexttrackimage");
            }
        }

        public ImageSource previoustrackimage
        {
            get { return this._previoustrackimage; }
            set
            {
                if (this._previoustrackimage == value) { return; }
                this._previoustrackimage = value;
                this.NotifyPropertyChanged("previoustrackimage");
            }
        }

        public ImageSource shuffleimage
        {
            get { return this._shuffleimage; }
            set
            {
                if (this._shuffleimage == value) { return; }
                this._shuffleimage = value;
                this.NotifyPropertyChanged("shuffleimage");
            }
        }
        public String ThemeName { get; set; }
    }

}
