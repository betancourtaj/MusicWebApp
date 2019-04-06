using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages
{
    public class AddSongModel : PageModel
    {
        [BindProperty]
        public DateTime songDate { get; set; }
        [BindProperty]
        public string songTitle { get; set; }
        [BindProperty]
        public string songArtist { get; set; }
        [BindProperty]
        public string songAlbum { get; set; }
        [BindProperty]
        public double songLength { get; set; }


        public void OnGet()
        {
        }

        public IActionResult OnPostSubmitButton()
        {
            if(ModelState.IsValid)
            {
            MusicDataBase.AddSong(songTitle, songAlbum, songArtist, songDate.ToString("MM:dd:yyyy"), songLength.ToString());

            Console.WriteLine($"Date: {songDate}");
            Console.WriteLine($"Title: {songTitle}");
            Console.WriteLine($"Album: {songAlbum}");
            Console.WriteLine($"Artist: {songArtist}");
            Console.WriteLine($"Length: {songLength}");
            
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Error");
        }
    }
}