using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using App.Models;

namespace App.Pages
{
    public class ViewPlaylistModel : PageModel
    {
        [BindProperty]
        public int UserID { get; private set; }

        [BindProperty]
        public string PlaylistName { get; private set; }

        [BindProperty]
        public Song[] Songs { get; private set; }

        [BindProperty]
        public Comment[] Comments { get; private set; }

        private ISession Session;

        public void OnGet(int userId, string playlist)
        {
            Session = HttpContext.Session;

            UserID = userId;
            PlaylistName = playlist;
            Session.SetString("PlaylistName", playlist);

            GetSongsAndComments();
        }

        private void GetSongsAndComments()
        {
            Songs = MusicDataBase.GetSongsFromPlaylist(UserID, PlaylistName);
            Comments = MusicDataBase.GetCommentsForPlaylist(UserID, PlaylistName);
        }

        public IActionResult OnPostEdit()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "TRUE");
                GetSongsAndComments();
                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("UserID")}&playlist={Session.GetString("PlaylistName")}");
            }
            
            return RedirectToPage("./Error");
        }

        public IActionResult OnPostSubmit()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");
                GetSongsAndComments();
                
                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("UserID")}&playlist={Session.GetString("PlaylistName")}");
            }
            return RedirectToPage("./Error");
        }
    }
}