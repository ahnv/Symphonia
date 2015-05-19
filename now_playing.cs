using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

public class now_playing : INotifyPropertyChanged
{
    private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    public Action albumartchanged { get; set; } 

    public String album
    {
        get
        {
            if(!this.localSettings.Values.ContainsKey("currentalbum"))
            {
                return String.Empty;
            }
            return (String)this.localSettings.Values["currentalbum"];
        }
    }

    public String albumart
    {
        get
        {
            if (!this.localSettings.Values.ContainsKey("currentalbumart"))
            {
                return String.Empty;
            }
            return (String)this.localSettings.Values["currentalbumart"];
        }
    }

    public String artist
    {
        get
        {
            if (!this.localSettings.Values.ContainsKey("currentartist"))
            {
                return String.Empty;
            }
            return (String)this.localSettings.Values["currentartist"];
        }
    }

    public Visibility backgroundtaskactive
    {
        get
        {
            Boolean item = false;
            if(this.localSettings.Values.ContainsKey("backgroundtaskactive"))
            {
                item = (Boolean)this.localSettings.Values["backgroundtaskactive"];
            }
            if (item)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }

    public String passedtime
    {
        get { return BackgroundMediaPlayer.Current.Position.ToString("mm\\:ss"); }
    }

    public BitmapImage playpause
    {
        get
        {
            if(BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
            {
                if (localSettings.Values.ContainsKey("theme") && (String)this.localSettings.Values["theme"] == "Light")
                {
                    return new BitmapImage(new Uri("ms-appx:/Icons/pause_black"));
                }
                return new BitmapImage(new System.Uri("ms-appx:/Icons/pause_white.png"));
			}
			if (this.localSettings.Values.ContainsKey("theme") && (String)this.localSettings.Values["theme"] == "Light")
			{
				return new BitmapImage(new System.Uri("ms-appx:/Icons/play_black.png"));
			}
			return new BitmapImage(new System.Uri("ms-appx:/Icons/play_white.png"));
            
        }
    }

    public String remainingtime
    {
        get
        {
            TimeSpan naturalDuration = BackgroundMediaPlayer.Current.NaturalDuration - BackgroundMediaPlayer.Current.Position;
            return String.Concat("-", naturalDuration.ToString("mm\\:ss"));
        }
    }

    public BitmapImage repeatimage
    {
        get
        {
            Int32 item = 0;
            if (this.localSettings.Values.ContainsKey("repeatvalue"))
            {
                item = (Int32)this.localSettings.Values["repeatvalue"];
            }
            if (item == 0)
            {
                if (this.localSettings.Values.ContainsKey("theme") && (String)this.localSettings.Values["theme"] == "Light")
                {
                    return new BitmapImage(new System.Uri("ms-appx:/Icons/repeat-opacity_black.png"));
                }
                return new BitmapImage(new System.Uri("ms-appx:/Icons/repeat-opacity_white.png"));
            }
            if (item == 1)
            {
                if (this.localSettings.Values.ContainsKey("theme") && (String)this.localSettings.Values["theme"] == "Light")
                {
                    return new BitmapImage(new System.Uri("ms-appx:/Icons/repeat_black.png"));
                }
                return new BitmapImage(new System.Uri("ms-appx:/Icons/repeat_white.png"));
            }
            if (this.localSettings.Values.ContainsKey("theme") && (String)this.localSettings.Values["theme"] == "Light")
            {
                return new BitmapImage(new System.Uri("ms-appx:/Icons/repeat-one_black.png"));
            }
            return new BitmapImage(new System.Uri("ms-appx:/Icons/repeat-one_white.png"));
        }
    }

    public Boolean running;
    public Double screenwidth { get { return Window.Current.Bounds.Width - 40; } }

    public Double shuffle
    {
        get
        {
            Boolean item = false;
            if (this.localSettings.Values.ContainsKey("shufflevalue"))
            {
                item = (Boolean)this.localSettings.Values["shufflevalue"];
            }
            if (item)
            {
                return 1;
            }
            return 0.2;
        }
    }

    public Double slider_position
    {
        get
        {
            Double totalSeconds = BackgroundMediaPlayer.Current.Position.TotalSeconds;
            TimeSpan naturalDuration = BackgroundMediaPlayer.Current.NaturalDuration;
            return totalSeconds / naturalDuration.TotalSeconds * 100;
        }
    }

    public TimeSpan TotalDuration
    {
        get
        {
            return BackgroundMediaPlayer.Current.NaturalDuration;
        }
    }

    public String tracktitle
    {
        get
        {
            if (!this.localSettings.Values.ContainsKey("currenttracktitle"))
            {
                return String.Empty;
            }
            return (String)this.localSettings.Values["currenttracktitle"];
        }
    }

    public now_playing()
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        dispatcherTimer.Tick += new EventHandler<object>(UpdatePosition);
        dispatcherTimer.Start();
    }

    private void CurrentStateChanged(MediaPlayer sender, Object args)    { this.NotifyPropertyChanged("playpause"); }

    public void NotifyChange(Boolean imagestoo)
    {
        this.NotifyPropertyChanged("artist");
        this.NotifyPropertyChanged("album");
        this.NotifyPropertyChanged("tracktitle");
        this.NotifyPropertyChanged("albumart");
        this.NotifyPropertyChanged("backgroundtaskactive");
        this.NotifyPropertyChanged("playpause");
        if (imagestoo)
        {
            this.NotifyPropertyChanged("repeatimage");
        }
    }

    private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
    private async void NotifyPropertyChanged(String p)
    {
        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
        if (this.PropertyChanged != null)
        {
            CoreDispatcher coreDispatcher = this.dispatcher;
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(p));
                if (this.albumartchanged != null && p == "albumart" && this.albumart != String.Empty)
                {
                    this.albumartchanged.Invoke();
                }
            });
        }
    }

    public void NotifyRepeatChanged()
    {
        this.NotifyPropertyChanged("repeatimage");
    }

    public void NotifyShuffleChanged()
    {
        this.NotifyPropertyChanged("shuffle");
    }

    public void SetPosition(TimeSpan value)
    {
        BackgroundMediaPlayer.Current.Position = value;
        this.UpdatePosition(null, null);
    }

    public void Subscribe()
    {
        this.running = true;
        MediaPlayer current = BackgroundMediaPlayer.Current;
        current.CurrentStateChanged += new Windows.Foundation.TypedEventHandler<MediaPlayer, object>(this.CurrentStateChanged);
    }

    public void UnSubscribe()
    {
        this.running = false;
        BackgroundMediaPlayer.Current.CurrentStateChanged -= new Windows.Foundation.TypedEventHandler<MediaPlayer, object>(this.CurrentStateChanged);
    }

    public void UpdatePosition(Object sender, Object e)
    {
        this.NotifyPropertyChanged("passedtime");
        this.NotifyPropertyChanged("remainingtime");
        this.NotifyPropertyChanged("slider_position");
        this.NotifyPropertyChanged("TotalDuration");
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
