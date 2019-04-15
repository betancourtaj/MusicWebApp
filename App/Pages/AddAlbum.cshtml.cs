using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class AddAlbumModel : PageModel
    {
        [BindProperty]
        public string AlbumName { get; set; }
        [BindProperty]
        public DateTime ReleaseDate { get; set; }

        private ISession Session;

        public void OnGet()
        {
        }

        public IActionResult OnPostCreateAlbum(string addAlbumButton)
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                MusicDataBase.AddAlbum(AlbumName, Session.GetInt32("UserID"), ReleaseDate.ToString("MM/dd/yyyy"));
                return RedirectToPage("./Add");
            }

            return RedirectToPage("./Error");
        }
    }
}