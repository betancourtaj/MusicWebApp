using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class CreatePlaylistModel : PageModel
    {
        [BindProperty]
        public string PlaylistTitle { get; set; }

        private ISession Session;

        public void OnGet()
        {
            Session = HttpContext.Session;
            if(Session.GetString("IsLoggedIn") == null || Session.GetString("IsLoggedIn") == "FALSE")
            {
                Response.Redirect("./Index");
            }
        }

        public IActionResult OnPostCreatePlaylist()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                MusicDataBase.CreatePlaylist((int) Session.GetInt32("UserID"), PlaylistTitle);
                return RedirectToPage("./UserProfile");
            }

            return RedirectToPage("./Error");
        }
    }
}