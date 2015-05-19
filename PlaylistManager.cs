using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace PlaylistManager
{
    public class TrackData
    {
        public String album { get; set; }
        public String albumart { get; set; }
        public String albumartist { get; set; }
        public String artist { get; set; }
        public DateTime created { get; set; }
        public String filepath { get; set; }
        public String lyrics { get; set; }
        public Boolean podcast { get; set; }
        public String title { get; set; }
        public Int32 tracknumber { get; set; }
    }

    public class AlbumData
    {
        public String albumart { get; set; }
        public String artist { get; set; }
        public String title { get; set; }
        public List<TrackData> tracks { get; set; }
        public List<TrackData> sortedtracks
        {
            get
            {
                if (this.tracks == null)
                {
                    return new List<TrackData>();
                }
                List<TrackData> trackdatas = this.tracks;
                return trackdatas.OrderBy(TrackData => TrackData.title).ToList<TrackData>();
            }

        }
    }

    public class ArtistData
    {
        public List<AlbumData> albums { get; set; }
        public Int32 amountofalbums { get; set; }
        public String name { get; set; }
    }

    public class PlayList
    {
        public List<TrackData> firsttracks
        {
            get
            {
                if(this.tracks == null)
                {
                    return new List<TrackData>();
                }

                return this.tracks.Take<TrackData>(8).ToList<TrackData>();
            }
        }
        public List<TrackData> tracks {get; set;}
        public String title { get; set; }
        public Boolean isdeletable { get; set; }
    }
}
