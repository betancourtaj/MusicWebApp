using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class AccountHandlerModel : PageModel
    {

        private ISession Session;

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
            
        }

        public IActionResult OnPostRegister()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsRegistered", "TRUE");
                Session.SetString("SessionUser", Username);
                Console.WriteLine($"Session: {Session.GetString("IsRegistered")}");
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}