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
    public class SearchSongModel : PageModel
    {

        [BindProperty]
        public Song[] Songs { get; private set; }

        [BindProperty]
        public List<string> ArtistNames { get; private set; }

        [BindProperty]
        public Album[] Albums { get; private set; }

        [BindProperty]
        public User[] Users { get; private set; }

        [BindProperty]
        public Playlist[] searchPlaylists {get; private set;}

        [BindProperty]
        public string[] CurrentUsersPlaylists { get; private set; }

        [BindProperty]
        public int PageUserID { get; private set; }

        [BindProperty]
        public string SearchString { get; set; }

        private ISession Session;

        public void OnPostSearch() 
        {
            SearchString = Request.Form["search-bar"];
            Session = HttpContext.Session;
            Session.SetString("SearchString", SearchString);
            GetData();
            if (Songs != null)
            {
                GetArtistData();
            }

            if(Session.GetString("IsLoggedIn") == "TRUE")
            {
                PageUserID = (int) Session.GetInt32("UserID");
                CurrentUsersPlaylists = MusicDataBase.GetPlaylistNamesForUserID(PageUserID);
            }
        }

        private void GetData() 
        {
            Session = HttpContext.Session;
            Albums = MusicDataBase.GetSearchResultsAlbum(Session.GetString("SearchString"));
            Users = MusicDataBase.GetSearchResultsUsers(Session.GetString("SearchString"));
            Songs = MusicDataBase.GetSearchResultsSong(Session.GetString("SearchString"));
            searchPlaylists = MusicDataBase.GetSearchResultsPlaylists(Session.GetString("SearchString"));
        }

        private void GetArtistData() 
        {
            ArtistNames = new List<string>();
            for (int i=0; i<Songs.Length; i++) 
                {
                    int artistId = MusicDataBase.FindArtistID(Songs[i].SongID);
                    string artistName = MusicDataBase.GetUserNameForID(artistId);
                    ArtistNames.Add(artistName);
                }
        }

        public IActionResult OnPostAdd() 
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                SearchString = Session.GetString("SearchString");
                int songId = -1;
                GetData();
                string playlistName = Request.Form["add-song-button"];

                if ( IsPlaylist(playlistName) == false )
                    return Redirect("./SearchSong");
                
                try 
                {
                    songId = Convert.ToInt32(Request.Form["song-id"]);
                    if(IsSongID(songId) == false || songId < 0)
                    {
                        return Redirect("./SearchSong");
                    }
                } catch (InvalidCastException e)
                {
                    Console.WriteLine("Incorrect type of id" + e.Message);
                    return Redirect("./SearchSong");
                }

                Session = HttpContext.Session;
                PageUserID = (int) Session.GetInt32("UserID");

                int playlistId = MusicDataBase.GetPlaylistIDForPlaylistNameAndUserID(playlistName, PageUserID);
                MusicDataBase.AddSongToPlaylist(songId, playlistId);
                return Redirect("./SearchSong");
            }
            return RedirectToPage("./Error");
        }

        private bool IsPlaylist(string playlistName) 
        {
            Session = HttpContext.Session;
            PageUserID = (int) Session.GetInt32("UserID");
            CurrentUsersPlaylists = MusicDataBase.GetPlaylistNamesForUserID(PageUserID);

            foreach(var playlist in CurrentUsersPlaylists)
            {
                if( string.Equals(playlistName, playlist) )
                    return true;
            }

            return false;
        }

        private bool IsSongID(int songId)
        {
            foreach(var song in Songs)
            {
                if(song.SongID == songId)
                {
                    return true;
                }
            }

            return false;
        }

        public IActionResult OnPostViewPlaylist()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");
                //add checks
                int userIdToView = Convert.ToInt32(Request.Form["user-id"]);
                string viewPlaylist = Request.Form["view-playlist-button"];
                int playlistid = Convert.ToInt32(Request.Form["playlist-id"]);

                GetData();

                if(IsValidPlaylist(playlistid))
                {
                    return Redirect($"./ViewPlaylist?userId={userIdToView}&playlist={viewPlaylist}");
                }

                return Page();
            }
            return RedirectToPage("./Error");
        }

        private bool IsValidPlaylist(int playlistID)
        {
            foreach(var v in searchPlaylists)
            {
                if(v.PlaylistId == playlistID)
                {
                    return true;
                }
            }
            return false;
        }

        public IActionResult OnPostViewProfile()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");
                //add checks
                int userIdToView = Convert.ToInt32(Request.Form["user-id"]);

                return Redirect($"./UserProfile?userId={userIdToView}");
            }
            return RedirectToPage("./Error");
        }


        public bool isValidUserId(int userId)
        {
            return false;
        }

        public IActionResult OnPostViewSelectedAlbum()
        {
            int albumTitle = Convert.ToInt32(Request.Form["album-id"]);

            if(ModelState.IsValid)
            {
                GetData();

                if(IsValidAlbum(albumTitle))
                {
                    return Redirect($"./ViewAlbum?album={albumTitle}&albumId={Request.Form["album-id"]}");
                }

                return Page();
            }

            return RedirectToPage("./Error");
        }

        private bool IsValidAlbum(int albumid)
        {
            Session = HttpContext.Session;
            foreach(var v in Albums)
            {
                if(v.AlbumID == albumid)
                {
                    Session.SetString("ArtistName", v.ArtistName);
                    return true;
                }
            }

            return false;
        }

        public bool isValidPlaylistForUserId(int userId, string playlistName){
            return false;
        }
    }
}