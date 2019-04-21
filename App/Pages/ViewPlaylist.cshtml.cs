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
            Session.SetInt32("PlaylistUserID", UserID);
            PlaylistName = playlist;
            Session.SetString("PlaylistName", PlaylistName);

            GetSongsAndComments();

            string[] playlists = MusicDataBase.GetPlaylistNamesForUserID(UserID);
            if(playlists != null && !playlists.Contains(playlist))
            {
                Response.Redirect("./Error");
            }
        }

        private void GetSongsAndComments()
        {
            Songs = MusicDataBase.GetSongsFromPlaylist(UserID, PlaylistName);
            Comments = MusicDataBase.GetCommentsForPlaylist(UserID, PlaylistName);
        }

        public IActionResult OnPostEdit(string data)
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "TRUE");
                GetSongsAndComments();
                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={Session.GetString("PlaylistName")}");
            }
            
            return RedirectToPage("./Error");
        }

        public IActionResult OnPostSubmit()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");
                string commentString = Request.Form["commentText"];
                string commentid = Request.Form["comment-id-texta"];
                GetSongsAndComments();
                MusicDataBase.ChangeComment(commentString, Convert.ToInt32(commentid));
                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={Session.GetString("PlaylistName")}");
            }
            return RedirectToPage("./Error");
        }
    }
}