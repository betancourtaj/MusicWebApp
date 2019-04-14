using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class RegisterArtistModel : PageModel
    {

        [BindProperty]
        public string Email { get; set; }

        private ISession Session;

        public void OnGet()
        {
            Session = HttpContext.Session;
            if(Session.GetString("IsLoggedIn") == null)
            {
                Response.Redirect("./Index");
            }
        }

        public IActionResult OnPostRegister()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                
                if(!MusicDataBase.GetUsernameForEmail(Email).Equals(Session.GetString("SessionUser")))
                {
                    return Page();
                }
                else
                {
                    MusicDataBase.AddArtist( Session.GetInt32("UserID") );
                    Session.SetString("IsArtist", "TRUE");
                    return RedirectToPage("./Index");
                }
            }

            return RedirectToPage("./Error");
        }
    }
}