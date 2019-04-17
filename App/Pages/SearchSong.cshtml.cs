using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages
{
    public class SearchSongModel : PageModel
    {

        [BindProperty]
        public string[] Test { get ; set; } = new string[] { "1", "2", "3" };
`
        [BindProperty]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] SearchResults { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] SearchKeys { get; set; }

        public void OnPost(string data)
        {
            Console.WriteLine($"Post: {data}");
        }

        public IActionResult OnGet(string data)
        {
            var results = MusicDataBase.GetSearchResults(data);
            SearchResults = results.Keys.ToArray();
            SearchKeys = results.Values.ToArray();
            SearchString = data;
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