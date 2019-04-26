using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using App.Models;

namespace App.Pages
{
    public class ViewAlbumModel : PageModel
    {
        [BindProperty]
        public string AlbumName { get; private set; }

        [BindProperty]
        public Song[] Songs { get; private set; }

        [BindProperty]
        public string ArtistName { get; private set; }

        [BindProperty]
        public Playlist[] UserPlaylists { get; private set; }

        private int AlbumID;

        private ISession Session;

        public void OnGet(string album, int albumId)
        {
            Session = HttpContext.Session;

            AlbumName = album;
            Session.SetString("AlbumName", AlbumName);
            AlbumID = albumId;
            Session.SetInt32("AlbumID", AlbumID);

            if(Session.GetString("IsLoggedIn") == "TRUE")
            {
                UserPlaylists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));
            }

            ArtistName = Session.GetString("PageUserName");
            if(ArtistName == null)
            {
                ArtistName = Session.GetString("ArtistName");
            }

            QuerySongs();
        }

        private void QuerySongs()
        {
            Session = HttpContext.Session;
            Songs = MusicDataBase.GetSongsFromAlbum((int) Session.GetInt32("AlbumID"));
            if(Session.GetString("IsLoggedIn") == "TRUE")
            {
                UserPlaylists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));
            }
        }

        public IActionResult OnPostAddSongsToPlaylist()
        {
            if(ModelState.IsValid)
            {
                int playlistID = Convert.ToInt32(Request.Form["playlist-id"]);

                QuerySongs();

                if(IsValidPlaylist(playlistID))
                {
                    foreach(var v in Songs)
                    {
                        MusicDataBase.AddSongToPlaylist(v.SongID, playlistID);
                    }

                    return RedirectToPage("./UserProfile");
                }

                return Page();
            }

            return RedirectToPage("./Error");
        }

        private bool IsValidPlaylist(int playlistID)
        {
            foreach(var v in UserPlaylists)
            {
                if(v.PlaylistId == playlistID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}