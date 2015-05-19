using Symphonia;
using PlaylistManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using TagLib;

public class MusicData : INotifyPropertyChanged
{
    private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
    private ApplicationDataContainer localsettings = ApplicationData.Current.LocalSettings;
    private StorageFile global_albumart;
    private ObservableCollection<ArtistData> artists;
    private ObservableCollection<AlbumData> albums;
    private ObservableCollection<TrackData> tracks;
    private ObservableCollection<PlayList> playlists;
    private String _amountoftracks;
    public event PropertyChangedEventHandler PropertyChanged;
    private async void NotifyPropertyChanged(String p)
    {
        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
        if (this.PropertyChanged != null)
        {
            CoreDispatcher coreDispatcher = this.dispatcher;
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.PropertyChanged(this, new PropertyChangedEventArgs(p)));
        }
    }
    public String amountoftracks { get { return this._amountoftracks; } set { this._amountoftracks = value; this.NotifyPropertyChanged("amountoftracks"); } }
    public ObservableCollection<ArtistData> Artists 
    { 
        get { return this.artists; } 
        set { this.artists = value; this.NotifyPropertyChanged("Artists"); this.NotifyPropertyChanged("Albums"); this.NotifyPropertyChanged("Tracks"); } 
    }
    public ObservableCollection<AlbumData> Albums
    {
        get
        {
            this.albums = new ObservableCollection<AlbumData>();
            foreach (ArtistData artist in this.Artists)
            {
                foreach (AlbumData album in artist.albums)
                {
                    this.albums.Add(album);
                }
            }
            ObservableCollection<AlbumData> observableCollection = this.albums;
            this.albums = new ObservableCollection<AlbumData>(observableCollection.OrderBy(AlbumData => AlbumData.title));
            return this.albums;
        }
    }
    public ObservableCollection<PlayList> Playlists
    {
        get
        {
            return this.playlists;
        }
        set
        {
            if (this.playlists != value)
            {
                this.playlists = value;
            }
            this.NotifyPropertyChanged("Playlists");
        }
    }
    public ObservableCollection<TrackData> Tracks
    {
        get
        {
            this.tracks = new ObservableCollection<TrackData>();
            foreach (ArtistData artist in this.Artists)
            {
                foreach (AlbumData album in artist.albums)
                {
                    foreach (TrackData track in album.tracks)
                    {
                        this.tracks.Add(track);
                    }
                }
            }
            ObservableCollection<TrackData> observableCollection = this.tracks;
            this.tracks = new ObservableCollection<TrackData>(observableCollection.OrderBy<TrackData, String>(TrackData => TrackData.title));
            return this.tracks;
        }
    }
    private async Task RetrieveFiles(IStorageItem item, String subfolder)
    {
        String str, str1;
        if (!(item is StorageFile))
        {
            IReadOnlyList<IStorageItem> itemsAsync = await (item as StorageFolder).GetItemsAsync();
            this.global_albumart = null;
            foreach(IStorageItem storageItem in itemsAsync)
            {
                if(!(storageItem is StorageFile)) { continue; }
                String path = (storageItem as StorageFile).Path;
                String[] strArray = path.Split(new Char[] { '.' });
                if (!(strArray.Last<String>().ToLower() == "jpg") && !(strArray.Last<String>().ToLower() == "png")) { continue; }
                this.global_albumart = storageItem as StorageFile;
            }
            newtrackscount count = App.newtracks;
            count.totaltracks = count.totaltracks + itemsAsync.Count;
            foreach (IStorageItem storageitem1 in itemsAsync)
            {
                await this.RetrieveFiles(storageitem1, (item as StorageFolder).DisplayName);
            }
        }
        else
        {
            try
            {
                Char chr = "\\".First<Char>();
                String path1 = (item as StorageFile).Path;
                String[] strArray1 = path1.Split(new Char[] {chr});
                String str2 = strArray1.Last<String>();
                chr = '.';
                String str3 = strArray1.Last<String>();
                strArray1 = str3.Split(new Char[] { chr });
                String lower = strArray1.Last<String>().ToLower();
                if( lower == "mp3" || lower == "wma" || lower == "m4a" || lower == "wav")
                {
                    Stream stream = await (item as StorageFile).OpenStreamForReadAsync();
                    File file = File.Create(new StreamFileAbstraction((item as StorageFile).Name, stream, stream));
                    Tag tag = file.Tag;
                    str = (tag.Title == null || tag.Title == String.Empty ? (item as StorageFile).DisplayName:tag.Title);
                    str1 = (tag.JoinedPerformers == null || tag.JoinedPerformers == String.Empty ? "Unknown artist" : tag.JoinedPerformers);
                    String str4 = (tag.JoinedAlbumArtists == null || tag.JoinedAlbumArtists == String.Empty || tag.JoinedAlbumArtists.Length == 0 ? str1 : tag.JoinedAlbumArtists);
                    String str5 = (tag.Album == null || tag.Album == String.Empty ? "Unknown album" : tag.Album);
                    String empty = String.Empty;
                    if (tag.Lyrics != null || tag.Lyrics != String.Empty)
                    {
                        empty = tag.Lyrics;
                    }
                    String str6 = String.Concat(str2.Substring(0, str2.Length - 4), ".jpg");
                    String empty1 = String.Empty;
                    if((Int32)tag.Pictures.Length>0)
                    {
                        try
                        {
                            Byte[] data = tag.Pictures[0].Data.Data;
                            StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(str6, CreationCollisionOption.ReplaceExisting);
                            StorageFile storageFile1 = storageFile;
                            using (Stream stream1 = await storageFile1.OpenStreamForWriteAsync())
                            {
                                stream1.Write(data, 0, data.Count<Byte>());
                            }
                            empty1 = storageFile1.Path;
                        }
                        catch (Exception exception) { }
                    }
                    else if (this.global_albumart != null && subfolder != String.Empty)
                    {
                        StorageFile storageFile2 = await this.global_albumart.CopyAsync(ApplicationData.Current.LocalFolder, str6, NameCollisionOption.ReplaceExisting);
                        empty1 = storageFile2.Path;
                    }
                    TrackData trackdata = new TrackData()
                    {
                        filepath = (item as StorageFile).Path,
                        title = str,
                        artist = str1,
                        albumartist = str4,
                        album = str5,
                        lyrics = empty,
                        albumart = empty1,
                        tracknumber = Convert.ToInt32(tag.Track),
                        created = (item as StorageFile).DateCreated.DateTime
                    };
                    
                    if (subfolder != "podcasts")
                    {
                        trackdata.podcast = false;
                    }
                    else
                    {
                        trackdata.podcast = true;
                    }
                    ArtistData artdata = this.Artists.Where(ArtistData => ArtistData.name == trackdata.albumartist).FirstOrDefault<ArtistData>();
                    if( artdata != null)
                    {
                        List<AlbumData> albumdata = artdata.albums;
                        AlbumData albumdatas = albumdata.Where(AlbumData => AlbumData.title == trackdata.album).FirstOrDefault<AlbumData>();
                        if(albumdatas != null)
                        {
                            albumdatas.tracks.Add(trackdata);
                        }
                        else
                        {
                            AlbumData albumdatas1 = new AlbumData()
                            {
                                title = str5,
                                artist = str4,
                                albumart = empty1,
                                tracks = new List<TrackData>()
                            };
                            albumdatas1.tracks.Add(trackdata);
                            artdata.albums.Add(albumdatas1);
                        }
                    }
                    else
                    {
                        artdata = new ArtistData()
                        {
                            name = str4,
                            albums = new List<AlbumData>()
                        };
                        AlbumData albumdatas2 = new AlbumData() 
                        {
                            title = str5,
                            artist = str4,
                            albumart = empty1,
                            tracks = new List<TrackData>()
                        };
                        albumdatas2.tracks.Add(trackdata);
                        artdata.albums.Add(albumdatas2);
                        this.Artists.Add(artdata);
                    }
                    App.newtracks.addedtracks = this.Tracks.Count;
                }

            }
            catch (Exception exception1) { }
        }
    }

    private async Task RetreivePlaylist(IStorageItem item)
    {
        if (item is StorageFile)
        {
            Char chr = '.';
            String path = (item as StorageFile).Path;
            Char[] chrArray = new Char[] { chr };
            if (path.Split(chrArray).Count<String>() != 1)
            {
                return;
            }
            else
            {
                IInputStream inputStream = await (item as StorageFile).OpenSequentialReadAsync();
                try
                {
                    Stream stream = inputStream.AsStreamForRead();
                    (new StreamReader(stream)).ReadToEnd();
                }
                finally
                {
                    if (inputStream != null)
                    {
                        inputStream.Dispose();
                    }
                }
            }
        }
    }

    private async Task PopulatePlaylists()
    {
        await KnownFolders.MusicLibrary.GetFolderAsync("Playlist");
        foreach (IStorageItem itemsAsync in await KnownFolders.MusicLibrary.GetItemsAsync())
        {
            await this.RetreivePlaylist(itemsAsync);
        }
    }

    public async Task PopulateAsync()
    {
        try
        {
            this.Artists = new ObservableCollection<ArtistData>();
            this.albums = new ObservableCollection<AlbumData>();
            this.tracks = new ObservableCollection<TrackData>();
            IReadOnlyList<IStorageItem> itemsAsync = await KnownFolders.MusicLibrary.GetItemsAsync();
            App.newtracks.totaltracks = itemsAsync.Count();
            foreach(IStorageItem storageItem in itemsAsync)
            {
                await this.RetrieveFiles(storageItem, String.Empty);
            }
            if (this.Playlists == null)
            {
                this.Playlists = new ObservableCollection<PlayList>();
                PlayList playList = new PlayList() 
                { 
                    title = "Last Added",
                    isdeletable = false
                };
                this.Playlists.Add(playList);
            }
            if (this.Playlists.Count() > 0)
            {
                PlayList list = this.Playlists.First<PlayList>();                                
                ObservableCollection<TrackData> tracks = this.Tracks;
                list.tracks = tracks.OrderByDescending(TrackData => TrackData.created).ToList<TrackData>();
            }
            if(await readwrite.SaveMusicData())
            {
                this.localsettings.Values["release_5"] =  true;
                this.localsettings.Values["amount_of_storageitems"] =  itemsAsync.Count;
            }



        }
        catch (Exception exception1)
        {
            Exception exception = exception1;
            App.ShowError(String.Concat("Error while Populating music in PopulateAsync", exception.Message.ToString()));
        }
    }
    public async Task<Boolean> CheckForNewMusic()
    {
        Boolean flag;
        IReadOnlyList<IStorageItem> itemsAsync = await KnownFolders.MusicLibrary.GetItemsAsync();
        flag = (!this.localsettings.Values.ContainsKey("amount_of_storageitems") || !this.localsettings.Values.ContainsKey("release_5") || (Int32)this.localsettings.Values["amount_of_storageitems"] != itemsAsync.Count() ? true : false);
        return flag;
    }
}



  

