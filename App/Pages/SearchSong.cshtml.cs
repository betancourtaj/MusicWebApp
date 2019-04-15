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
        public string SearchString { get; set; }

        public bool Requested = false;

        public void OnPost(string data)
        {
            Console.WriteLine($"Post: {data}");
        }

        public IActionResult OnGet(string data)
        {
            
            Console.WriteLine($"My: {data}");
            return Page();
        }
    }
}