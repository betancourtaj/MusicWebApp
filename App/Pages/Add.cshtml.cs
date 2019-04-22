using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public string Album { get; set; }

        [BindProperty]
        public DateTime Date { get; set; }

        [BindProperty]
        public string Artist { get; set; }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string[] Albums { get; set; }

        [BindProperty]
        public bool HasSelectedAlbum { get; set; } = false;

        private ISession Session;

        public void OnGet()
        {
            Session = HttpContext.Session;

            Console.WriteLine($"SESSION: {Session.GetString("IsArtist")}");
            if(Session.GetString("IsArtist") == null)
            {
                Response.Redirect("./Index");
            }

            Albums = MusicDataBase.GetAlbumsForArtist(Session.GetInt32("UserID"));
        }

        public IActionResult OnPostChooseAlbum(string albumButton)
        {
            if(ModelState.IsValid)
            {
                if(albumButton == null || albumButton == String.Empty) return RedirectToPage("./Add");

                Session = HttpContext.Session;
                Albums = MusicDataBase.GetAlbumsForArtist(Session.GetInt32("UserID"));

                if(IsAlbumValid(albumButton))
                {
                    HasSelectedAlbum = true;
                    Album = albumButton;
                    return Page();
                }

                return Page();
            }
            return RedirectToPage("./Error");
        }

        private bool IsAlbumValid(string albumButton)
        {
            foreach(var v in Albums)
            {
                if(v.Equals(albumButton))
                {
                    return true;
                }
            }

            return false;
        }

        public IActionResult OnPostAddSong()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                MusicDataBase.AddSong(Title, Album, Session.GetInt32("UserID"), Date.ToString("MM/dd/yyyy"));
                return RedirectToPage("./Add");
            }

            return RedirectToPage("./Error");
        }

        public IActionResult OnPostSendToAddAlbum()
        {
            if(ModelState.IsValid)
            {
                return RedirectToPage("./AddAlbum");
            }

            return RedirectToPage("./Error");
        }
    }
}