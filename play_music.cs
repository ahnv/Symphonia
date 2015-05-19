using PlaylistManager;
using Symphonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;

public class play_music : INotifyPropertyChanged
{
    private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
    private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    private Action bindingaction;
    private String playlistname = "playlistdata.xml";
    private String shuffleplaylistname = "shuffleplaylistdata.xml";
    private PlayList _internal_playlist;
    private PlayList _shuffle_internal_playlist;
    public event PropertyChangedEventHandler PropertyChanged;
    public Boolean running
    {
        get;
        set;
    }
    private async void NotifyPropertyChanged(String p)
    {
        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
        if (this.PropertyChanged != null)
        {
            CoreDispatcher coreDispatcher = this.dispatcher;
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.PropertyChanged(this, new PropertyChangedEventArgs(p)));
        }
    }

    private async void SetNewIndexUI()
    {
        if (this.bindingaction != null)
        {
            CoreDispatcher coreDispatcher = this.dispatcher;
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.bindingaction.Invoke());
        }
    }

    public Int32 internal_index
    {
        get
        {
            if (!this.localSettings.Values.ContainsKey("currentindex"))
            {
                return 0;
            }
            return (Int32)this.localSettings.Values["currentindex"];
        }
        set
        {
            this.localSettings.Values["currentindex"] = value;
            this.NotifyPropertyChanged("internal_index");
            this.SetNewIndexUI();
        }
    }
    public PlayList internal_playlist
    {
        get
        {
            return this._internal_playlist;
        }
        set
        {
            if (this._internal_playlist != value)
            {
                this._internal_playlist = value;
            }
        }
    }
    public PlayList shuffle_internal_playlist
    {
        get
        {
            return this._shuffle_internal_playlist;
        }
        set
        {
            if (this._shuffle_internal_playlist != value)
            {
                this._shuffle_internal_playlist = value;
            }
        }
    }
    public ObservableCollection<TrackData> internal_tracklist
    {
        get
        {
            if (App.nowplaying.shuffle == 1)
            {
                if (this.shuffle_internal_playlist.tracks == null)
                {
                    return null;
                }
                return new ObservableCollection<TrackData>(this.shuffle_internal_playlist.tracks);
            }
            if (this.internal_playlist.tracks == null)
            {
                return null;
            }
            return new ObservableCollection<TrackData>(this.internal_playlist.tracks);
        }
    }
    public void BindChange(Action todo) { this.bindingaction = todo; }
    private void ClearThumbnail() { App.nowplaying.UpdatePosition(null, null); }
    public void SendMessageToBackground(String x, String y)
    {
        ValueSet valueSet = new ValueSet();
        valueSet.Add(x, y);
        BackgroundMediaPlayer.SendMessageToBackground(valueSet);
    }
    public void MessageReceivedFromBackground(Object sender, MediaPlayerDataReceivedEventArgs e)
    {
        Int32 num;
        ValueSet data = e.Data;
        foreach (String key in data.Keys)
        {
            String str = key;
            String str1 = str;
            Dictionary<string, int> dict = new Dictionary<string, int>(7)
            {
                    { "ReadPlaylistComplete", 0 },
					{ "SetIndexComplete", 1 },
					{ "NewIndex", 2 },
					{ "NewInfo", 3 },
					{ "RepeatChanged", 4 },
					{ "ShuffleChanged", 5 },
					{ "Error", 6 }
            };
            if(dict.TryGetValue(str1,out num))
            {
                continue;
            }
            switch (num)
            {
                case 0:
                    {
                        this.SendMessageToBackground("SetNewIndex", this.internal_index.ToString());
                        continue;
                    }
                case 2:
                    {
                        this.SetNewIndexUI();
                        continue;
                    }
                case 3:
                    {
                        App.nowplaying.NotifyChange(false);
                        continue;
                    }
                case 4:
                    {
                        App.nowplaying.NotifyRepeatChanged();
                        continue;
                    }
                case 5:
                    {
                        App.nowplaying.NotifyShuffleChanged();
                        continue;
                    }
                case 6: 
                    {
                        this.ShowError(data[key].ToString());
                        continue;
                    }
                default:
                    {
                        continue;
                    }
            }
            
        }
    }
    public async void CreatePlaylists(List<TrackData> tracklist)
    {
        List<TrackData> trackDatas = new List<TrackData>();
        foreach (TrackData trackDatum in tracklist)
        {
            trackDatas.Add(trackDatum);
        }
        PlayList playList = new PlayList();
        playList.tracks = tracklist;
        (new PlayList()).tracks = trackDatas;
        PlayList playList1 = new PlayList();
        playList1.tracks = new List<TrackData>();
        Random random = new Random();
        while (trackDatas.Count > 0)
        {
            Int32 num = random.Next(0, trackDatas.Count);
            playList1.tracks.Add(trackDatas[num]);
            trackDatas.RemoveAt(num);
        }
        this.internal_playlist = playList;
        this.shuffle_internal_playlist = playList1;
        await readwrite.WriteShufflePlaylist(this.shuffle_internal_playlist);
        await readwrite.WriteNewPlaylist(this.internal_playlist);
        this.SendMessageToBackground("NewPlaylist", "");
    }
    private PlayList GenerateShufflePlaylist(PlayList tracklist)
    {
        PlayList playList = new PlayList();
        playList.tracks = new List<TrackData>();
        Random random = new Random();
        while (tracklist.tracks.Count > 0)
        {
            Int32 num = random.Next(0, tracklist.tracks.Count);
            playList.tracks.Add(tracklist.tracks[num]);
            tracklist.tracks.RemoveAt(num);
        }
        return playList;
    }
    private async void ReadPlaylist()
    {
        try
        {
            StorageFile fileAsync = await ApplicationData.Current.LocalFolder.GetFileAsync(this.playlistname);
            StorageFile storageFile = fileAsync;
            IInputStream inputStream = await storageFile.OpenSequentialReadAsync();
            try
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(PlayList));
                PlayList playList = dataContractSerializer.ReadObject(inputStream.AsStreamForRead()) as PlayList;
                this.internal_playlist = playList;
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Dispose();
                }
            }
            StorageFile fileAsync1 = await ApplicationData.Current.LocalFolder.GetFileAsync(this.shuffleplaylistname);
            storageFile = fileAsync1;
            IInputStream inputStream1 = await storageFile.OpenSequentialReadAsync();
            try
            {
                DataContractSerializer dataContractSerializer1 = new DataContractSerializer(typeof(PlayList));
                PlayList playList1 = dataContractSerializer1.ReadObject(inputStream1.AsStreamForRead()) as PlayList;
                this.shuffle_internal_playlist = playList1;
            }
            finally
            {
                if (inputStream1 != null)
                {
                    inputStream1.Dispose();
                }
            }
        }
        catch (Exception exception)
        {
        }
    }
    public void PlayTrackInstantly(TrackData track, Int32 index)
    {
        String[] _filepath = new String[] { track.filepath, "|", track.album, "|", track.artist, "|", track.title, "|", track.albumart, "|", index.ToString() };
        this.SendMessageToBackground("PlayTrackInstantly", String.Concat(_filepath));
    }

    public void PlayMusic(List<TrackData> tracklist, Int32 index, Boolean fromplayqueue)
    {
        if (!fromplayqueue)
        {
            this.PlayTrackInstantly(tracklist[index], index);
            this.internal_index = index;
            this.CreatePlaylists(tracklist);
            return;
        }
        if (App.nowplaying.shuffle != 1)
        {
            this.PlayTrackInstantly(this.internal_playlist.tracks[index], index);
        }
        else
        {
            this.PlayTrackInstantly(this.shuffle_internal_playlist.tracks[index], index);
        }
        this.internal_index = index;
        this.SendMessageToBackground("SetNewIndex", index.ToString());
    }

    private async void ShowError(String message)
    {
        try
        {
            MessageDialog messageDialog = new MessageDialog(message)
            {
                Title = "Error"
            };
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => messageDialog.ShowAsync());
        }
        catch (Exception exception)
        {
        }
    }

    public play_music()
    {
        this.internal_playlist = new PlayList();
        this.internal_playlist.tracks = new List<TrackData>();
        this.shuffle_internal_playlist = new PlayList();
        this.shuffle_internal_playlist.tracks = new List<TrackData>();
        this.ReadPlaylist();
    }

    public void Subscribe()
    {
        this.running = true;
        BackgroundMediaPlayer.MessageReceivedFromBackground += new EventHandler<MediaPlayerDataReceivedEventArgs>(this.MessageReceivedFromBackground);
        this.SendMessageToBackground("SendMusicInfo", "");
    }

    public void UnSubscribe()
    {
        this.running = false;
        BackgroundMediaPlayer.MessageReceivedFromBackground -= new EventHandler<MediaPlayerDataReceivedEventArgs>(this.MessageReceivedFromBackground);       
    }
}
