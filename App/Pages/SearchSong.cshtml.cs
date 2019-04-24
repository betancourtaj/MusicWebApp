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
        public string[] Test { get ; set; } = new string[] { "1", "2", "3" };
        [BindProperty]
        public Song[] Songs { get; set; }
        [BindProperty]
        public List<string> ArtistNames { get; set; }
        [BindProperty]
        public string[] Albums { get; set; }
        [BindProperty]
        public string[] Users { get; set; }
        [BindProperty]
        public string[] Playlists { get; private set; }
        [BindProperty]
        public int PageUserID { get; private set; }
        [BindProperty]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] SearchResults { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] SearchKeys { get; set; }

        private ISession Session;

        public void OnPostSearch() 
        {
            SearchString = Request.Form["search-bar"];
            Session = HttpContext.Session;
            Session.SetString("SearchString", SearchString);
            getData(SearchString);
            if (Songs != null)
            {
                getArtistData();
            }

            if(Session.GetString("IsLoggedIn") == "TRUE")
            {
                PageUserID = (int) Session.GetInt32("UserID");
                Playlists = MusicDataBase.GetPlaylistNamesForUserID(PageUserID);
            }
        }

        private void getData(string searchString) 
        {
            Albums = MusicDataBase.GetSearchResultsAlbum(searchString);
            Users = MusicDataBase.GetSearchResultsUsers(searchString);
            Songs = MusicDataBase.GetSearchResultsSong(searchString);
        }

        private void getArtistData() 
        {
            for (int i=0; i<Songs.Length; i++) 
                {
                    int artistId = MusicDataBase.FindArtistID(Songs[i].SongID);
                    string artistName = MusicDataBase.GetUserNameForID(artistId);
                    ArtistNames.Add(artistName);
                }
        }

        //needs fixing
        public IActionResult OnPostAdd() 
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                SearchString = Session.GetString("SearchString");
                int songId = -1;
                getData(SearchString) ;
                string playlistName = Request.Form["add-song-button"];

                if ( isPlaylist(playlistName) == false )
                    return Redirect("./SearchSong");
                
                try 
                {
                    songId = Convert.ToInt32(Request.Form["song-id"]);
                    if(isSongId(songId) == false || songId < 0)
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

        private Boolean isPlaylist(string playlistName) 
        {
            Session = HttpContext.Session;
            PageUserID = (int) Session.GetInt32("UserID");
            Playlists = MusicDataBase.GetPlaylistNamesForUserID(PageUserID);

            foreach(var playlist in Playlists)
            {
                if( string.Equals(playlistName, playlist) )
                    return true;
            }

            return false;
        }

        private Boolean isSongId(int songId)
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

        public IActionResult OnPostViewPlaylist(string viewPlaylist)
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");

                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={viewPlaylist}");
            }
            return RedirectToPage("./Error");
        }

        public IActionResult OnGet(string data)
        {
            var results = MusicDataBase.GetSearchResults(data);
            SearchResults = results.Keys.ToArray();
            SearchKeys = results.Values.ToArray();
            //SearchString = data;
            Console.WriteLine($"My: {data}");
            return Page();
        }

        public IActionResult OnPostGetAlbumArray()
        {
            Console.WriteLine("CALLED ALBUMS");
            return new JsonResult(SearchResults);
        }

        public IActionResult OnPostGetSongArray()
        {
            Console.WriteLine("CALLED SONGS");
            return new JsonResult(SearchKeys);
        }
    }
}