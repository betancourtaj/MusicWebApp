using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages
{
    public class ViewPlaylistModel : PageModel
    {
        [BindProperty]
        public string UserName { get; set; }

        public void OnGet(string userName)
        {
            UserName = userName;
        }
    }
}