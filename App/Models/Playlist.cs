<<<<<<< HEAD
using System;

=======
>>>>>>> c0174c89e2e241a727cc4f41400a4b7402c208b5
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