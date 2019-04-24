namespace App.Models
{
    public class Album
    {
        public int AlbumID { get; private set; }
        public string AlbumTitle { get; private set; }
        public string ReleaseDate { get; private set; }

        public Album(int albumid, string albumTitle, string releaseDate)
        {
            AlbumID = albumid;
            AlbumTitle = albumTitle;
            ReleaseDate = releaseDate;
        }
    }
}