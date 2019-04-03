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
        public void OnGet()
        {
            RedirectToPage("Pages/Index.cshtml");
        }

        public IActionResult OnPostSubmitButton()
        {
            MusicDataBase.Connect();
            MusicDataBase.Generic();
            MusicDataBase.Close();
            
            Console.WriteLine("Hello from OnPostSubmitButton!");
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