using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages
{
    public class AddSongModel : PageModel
    {
        public void OnGet()
        {
        }

        
        public IActionResult OnSubmitButtonClick()
        {
            Console.WriteLine("hello world!");
            return null;
        }
    }
}