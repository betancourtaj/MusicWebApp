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
        string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] SearchResults { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] SearchKeys { get; set; }

        private ISession Session;

        public void OnPostSearch() {
            SearchString = Request.Form["search-bar"];
            Albums = MusicDataBase.GetSearchResultsAlbum(SearchString);
            Users = MusicDataBase.GetSearchResultsUsers(SearchString);
            Songs = MusicDataBase.GetSearchResultsSong(SearchString);
            if (Songs != null){
                for (int i=0; i<Songs.Length; i++) {
                    int artistId = MusicDataBase.FindArtistID(Songs[i].SongID);
                    string artistName = MusicDataBase.GetUserNameForID(artistId);
                    ArtistNames.Add(artistName);
                }
            }

            Session = HttpContext.Session;
            PageUserID = (int) Session.GetInt32("UserID");
            Playlists = MusicDataBase.GetPlaylistNamesForUserID(PageUserID);
        }

        public void OnPostAdd() {
            string playlistName = Request.Form["playlistDisplay"];
            
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