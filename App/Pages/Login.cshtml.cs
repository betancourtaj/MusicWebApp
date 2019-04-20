using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
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

        public IActionResult OnPostLogin()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;

                if(MusicDataBase.UserExists(Email))
                {
                    if(!MusicDataBase.PasswordsMatch(Email, Password))
                    {
                        return Page();
                    }
                }
                
                Session.SetInt32("UserID", MusicDataBase.GetUserIDForEmail(Email));
                if(MusicDataBase.UserIsArtist(Session.GetInt32("UserID")))
                {
                    Session.SetString("IsArtist", "TRUE");
                }
                Session.SetString("SessionUser", MusicDataBase.GetUsernameForEmail(Email));
                Session.SetString("IsLoggedIn", "TRUE");
                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Error");
        }

        public IActionResult OnPostForgotPassword()
        {
            if(ModelState.IsValid)
            {
                return RedirectToPage("./ForgotPassword");
            }

            return RedirectToPage("./Error");
        }
    }
}