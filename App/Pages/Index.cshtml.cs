using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace App.Pages
{
    public class IndexModel : PageModel
    {
        ISession session;

        public void OnGet()
        {
            session = HttpContext.Session;
            Console.WriteLine($"SESSION: {session.GetString("IsRegistered")}");
        }
    }
}
