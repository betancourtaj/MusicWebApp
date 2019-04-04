using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using App.Models;

namespace App.Pages
{
    public class AddSongModel : PageModel
    {
        public void OnGet()
        {
            RedirectToPage("Pages/Index.cshtml");
        }

        public IActionResult OnPostSubmitButton()
        {
            //Song song = new Song(Request.Form["title-input"],
             //                    Request.Form["album-input"],
             //                    Request.Form["artist-input"],
             //                    Convert.ToDateTime(Request.Form["date-input"]).ToString(), 
             //                    Convert.ToDouble(Request.Form["length-input"].ToString()));

            //MusicDataBase.Connect();
            //MusicDataBase.SearchSong(song);
            //MusicDataBase.Close();
            // TODO: Figure me out.
            Console.WriteLine($"Date: {Request.Form["date-input"].ToString()}");
            
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostGetSearchText(int id, string searchString)
        {
            Console.WriteLine($"REQUESTED: {Request.Form["searchString"]}");
            return Page();
        }

/*
        [HttpPost]
        public IActionResult OnPost()
        {
            Console.WriteLine("Hello from OnPost!");
            return RedirectToPage("./Index");
        }

         */
    }
}