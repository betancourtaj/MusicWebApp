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

                Album[] albums = MusicDataBase.GetAlbumsFromArtistID((int) Session.GetInt32("UserID"));

                if(!IsDuplicate(albums))
                {

                    MusicDataBase.AddAlbum(AlbumName, Session.GetInt32("UserID"), ReleaseDate.ToString("MM/dd/yyyy"));
                    return RedirectToPage("./Add");
                }

                return Page();
            }

            return RedirectToPage("./Error");
        }

        private bool IsDuplicate(Album[] albums)
        {
            if(albums == null) return false;

            foreach(var v in albums)
            {
                if(v.AlbumTitle.Equals(AlbumName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}