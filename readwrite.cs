using Symphonia;
using PlaylistManager;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

public class readwrite
{
    private static String filename;
    private static String playlistpath;
    private static String shuffleplaylistpath;

    static readwrite()
    {
        readwrite.filename = "musicdata.xml";
        readwrite.playlistpath = "playlistpath.xml";
        readwrite.shuffleplaylistpath = "shuffleplaylistpath.xml";
    }

    public static async Task WriteNewPlaylist(PlayList playlist)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            (new DataContractSerializer(typeof(PlayList))).WriteObject(memoryStream, playlist);
            StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(readwrite.playlistpath, CreationCollisionOption.ReplaceExisting);
            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            {
                memoryStream.Seek((Int64)0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(stream);
                await stream.FlushAsync();
            }
        }
        catch (Exception exception) { }
    }
    public static async Task WriteShufflePlaylist(PlayList playlist)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            (new DataContractSerializer(typeof(PlayList))).WriteObject(memoryStream, playlist);
            StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(readwrite.shuffleplaylistpath, CreationCollisionOption.ReplaceExisting);
            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            {
                memoryStream.Seek((Int64)0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(stream);
                await stream.FlushAsync();
            }
        }
        catch (Exception exception) { }
    }

    public static async Task<Boolean> SaveMusicData()
    {
        Boolean flag;
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(MusicData));
            serializer.WriteObject(memoryStream, App.musicdata);
            StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(readwrite.filename, CreationCollisionOption.ReplaceExisting);
            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            {
                memoryStream.Seek((Int64)0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(stream);
                await stream.FlushAsync();
            }
            flag = true;
        }
        catch (Exception exception)
        {
            App.ShowError(String.Concat("Error while writing music data: ", exception.Message.ToString()));
            flag = false;
        }
        return flag;
    }
    public static async Task<Int32> ReadMusicData()
    {
        Int32 num;
        try
        {
            StorageFile fileAsync = await ApplicationData.Current.LocalFolder.GetFileAsync(readwrite.filename);
            IInputStream inputStream = await fileAsync.OpenSequentialReadAsync();
            try
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(MusicData));
                App.musicdata = dataContractSerializer.ReadObject(WindowsRuntimeStreamExtensions.AsStreamForRead(inputStream)) as MusicData;
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Dispose();
                }
            }
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            App.musicdata = new MusicData();
            num = 1;
            return num;
        }
        catch (Exception exception1)
        {
            Exception exception = exception1;
            
            App.ShowError(String.Concat("Error while reading music data: ", exception.Message.ToString()));
            App.musicdata = new MusicData();
            num = 1;
            return num;
        }
        Boolean flag = await App.musicdata.CheckForNewMusic();
        num = (!flag ? 0 : 2);
        return num;
    }

}