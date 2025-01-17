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

        [BindProperty]
        public int PageUserID { get; private set; }

        [BindProperty]
        public string ArtistName { get; private set; }

        private ISession Session;

        public void OnGet(int userId, string playlist, int playlistid)
        {
            Session = HttpContext.Session;

            UserID = userId;
            Session.SetInt32("PlaylistUserID", UserID);
            PlaylistName = playlist;
            Session.SetString("PlaylistName", PlaylistName);
            PageUserID = userId;
            Session.SetInt32("PlaylistID", playlistid);
            
            ArtistName = Session.GetString("PageUserName");
            if(ArtistName == null)
            {
                ArtistName = Session.GetString("ArtistName");
            }

            QuerySongsAndComments();

            string[] playlists = MusicDataBase.GetPlaylistNamesForUserID(UserID);
            if(playlists != null && !playlists.Contains(playlist))
            {
                Response.Redirect("./Error");
            }
        }

        private void QuerySongsAndComments()
        {
            Songs = MusicDataBase.GetSongsFromPlaylist((int) Session.GetInt32("PlaylistUserID"), Session.GetString("PlaylistName"));
            Comments = MusicDataBase.GetCommentsForPlaylist((int) Session.GetInt32("PlaylistUserID"), Session.GetString("PlaylistName"));
        }

        public IActionResult OnPostEdit(string data)
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "TRUE");
                QuerySongsAndComments();
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

                int commentid = 0;
                try {
                    commentid = Convert.ToInt32(Request.Form["comment-id-texta"]);
                } catch (InvalidCastException e)
                {
                    Console.WriteLine("Incorrect type of id" + e.Message);
                    return Page();
                }

                QuerySongsAndComments();

                if(IsValidCommentID(commentid))
                {
                    MusicDataBase.ChangeComment(commentString, commentid);
                    return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={Session.GetString("PlaylistName")}");
                }

                return Page();
            }
            return RedirectToPage("./Error");
        }

        public IActionResult OnPostDeleteSong()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;

                int songID = Convert.ToInt32(Request.Form["song-id"]);

                QuerySongsAndComments();

                if(IsValidSong(songID))
                {
                    MusicDataBase.DeleteSongFromPlaylist((int) Session.GetInt32("PlaylistID"), songID);
                    return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={Session.GetString("PlaylistName")}");
                }

                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={Session.GetString("PlaylistName")}");
            }

            return RedirectToPage("./Error");
        }

        private bool IsValidSong(int songID)
        {
            foreach(var v in Songs)
            {
                if(v.SongID == songID)
                {
                    return true;
                }
            }
            return false;
        }

        public IActionResult OnPostDeletePlaylist()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;

                QuerySongsAndComments();

                MusicDataBase.DeleteEntirePlaylist((int) Session.GetInt32("PlaylistID"));

                return RedirectToPage("./UserProfile");
            }

            return RedirectToPage("./Error");
        }

        private bool IsValidCommentID(int commentid)
        {
            Comment foundComment = null;
            foreach(var v in Comments)
            {
                if(v.CommentID == commentid)
                {
                    foundComment = v;
                    break;
                }
            }

            if(foundComment == null) return false;

            if(foundComment.UserID == Session.GetInt32("UserID"))
            {
                return true;
            }

            return false;
        }

        
    }
}