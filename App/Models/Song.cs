using System;

namespace App.Models
{
    public class Song
    {
        public int SongID { get; private set; }
        public int AlbumID { get; private set; }
        public string ReleaseDate { get; private set; }
        public string Title { get; private set; }
        public string AlbumName { get; private set; }

        public Song(int songid, string title, string releaseDate, int albumid, string albumName)
        {
            SongID = songid;
            Title = title;
            ReleaseDate = releaseDate;
            AlbumID = albumid;
            AlbumName = albumName;

        }
    }
}