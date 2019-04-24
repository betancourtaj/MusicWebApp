using System;

namespace App.Models
{
    public class Playlist
    {
        public int PlaylistId {get; private set;}
        public string PlaylistTitle {get; private set;}
        public int UserId {get; private set;}

        public Playlist(int playlistId, string playlistTitle, int userId)
        {
            PlaylistId = playlistId;
            PlaylistTitle = playlistTitle;
            UserId = userId;
        }
    }
}