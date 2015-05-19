using PlaylistManager;
using Symphonia.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
//using ThemeControllerHelper;

namespace Symphonia
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private CollectionViewSource _ArtistList;
        private CollectionViewSource _AlbumList;
        private CollectionViewSource _TrackList;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private Boolean istapped;
        private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        double sw = Window.Current.Bounds.Width;
        public CollectionViewSource AlbumList 
        {
            get
            {
                if (this._AlbumList == null && App.musicdata.Albums != null)
                {
                    List<AlbumData> list = App.musicdata.Albums.ToList<AlbumData>();
                    this._AlbumList = new CollectionViewSource() { Source = list };
                }
                return this._AlbumList;
            }
        }
        public CollectionViewSource ArtistList
        {
            get
            {
                if (this._ArtistList == null && App.musicdata.Artists != null)
                {
                    List<ArtistData> list = App.musicdata.Artists.ToList<ArtistData>();
                    this._ArtistList = new CollectionViewSource() { Source = list };
                }
                return this._ArtistList;
            }
        }
        public CollectionViewSource TrackList
        {
            get
            {
                if (this._TrackList == null && App.musicdata.Tracks != null)
                {
                    List<TrackData> list = App.musicdata.Tracks.ToList<TrackData>();
                    this._TrackList = new CollectionViewSource() { Source = list };

                }
                return this._TrackList;
            }
        }
        public ObservableCollection<PlayList> PlaylistList { get { return App.musicdata.Playlists; } }
        private void TrackSelected(Object sender, TappedRoutedEventArgs e)
        {
            if (!this.istapped)
            {
                List<TrackData> list = this.TrackList.View.Cast<TrackData>().ToList<TrackData>();
                App.playmusic.PlayMusic(list, this.TrackListView.SelectedIndex, false);
                if (!this.localSettings.Values.ContainsKey("show_nowplaying_when_tapping_song"))
                {
                    this.istapped = true;
                }
            }
        }
        private void PlayPause(Object sender, RoutedEventArgs e)
        {
            if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
            {
                BackgroundMediaPlayer.Current.Pause();
                return;
            }
            BackgroundMediaPlayer.Current.Play();
        }

        bool _viewMoved = false;

        void MoveViewWindow(double top)
        {
            _viewMoved = true;

            moveAnimation.SkipToFill();
            damove.To = top;

            moveAnimation.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var top = Canvas.GetTop(LayoutRoot);
            if (top > -100)
            {
                MoveViewWindow(-325);
            }
            else
            {
                MoveViewWindow(0);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "Normal", false);
            base.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
