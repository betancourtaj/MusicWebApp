using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Username { get; set; }

        // TODO: Minimum password length.
        [BindProperty]
        public string Password { get; set; }

        private ISession Session;

        public void OnGet()
        {
            Session = HttpContext.Session;
            if(Session.GetString("IsLoggedIn") == "TRUE")
            {
                Response.Redirect("./Index");
            }
        }

        public IActionResult OnPostRegister()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                if(!MusicDataBase.UserExists(Email))
                {
                    MusicDataBase.AddUser(Email, Username, Password);
                }
                else
                {
                    Session.SetString("IsRegistered", "TRUE");
                    Session.SetString("SessionUser", Username);
                    Session.SetString("IsArtist", "FALSE");

                    return RedirectToPage("./Login");
                }
                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Error");
        }
    }
}