using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class UserProfileModel : PageModel
    {

        private ISession Session;

        public void OnGet()
        {
            Session = HttpContext.Session;

            if(Session.GetString("IsLoggedIn") == null)
            {
                Response.Redirect("./Index");
            }
        }
    }
}