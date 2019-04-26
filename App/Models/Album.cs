namespace App.Models
{
    public class Album
    {
        public int AlbumID { get; private set; }
        public string AlbumTitle { get; private set; }
        public string ReleaseDate { get; private set; }
        public string ArtistName { get; private set; }

        public Album(int albumid, string albumTitle, string releaseDate, string artistName)
        {
            AlbumID = albumid;
            AlbumTitle = albumTitle;
            ReleaseDate = releaseDate;
            ArtistName = artistName;
        }
    }
}