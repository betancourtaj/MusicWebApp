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
    public class UserProfileModel : PageModel
    {

        [BindProperty]
        public string Username { get; private set; }

        [BindProperty]
        public Playlist[] Playlists { get; private set; }

        [BindProperty]
        public string Bio { get; private set; }

        [BindProperty]
        public int PageUserID { get; private set; }

        [BindProperty]
        public Album[] Albums { get; private set; }

        private ISession Session;

        public void OnGet(string userId)
        {
            Session = HttpContext.Session;
            Session.SetString("IsEditMode", "FALSE");
            if(userId != null)
            {
                PageUserID = Convert.ToInt32((string) userId);
                string usrName = MusicDataBase.GetUserNameForID(PageUserID);
                Boolean isArtist = MusicDataBase.UserIsArtist( Session.GetInt32("UserID") );
                Session.SetInt32("PlaylistUserID", PageUserID);

                if(usrName != null)
                {
                    Username = usrName;
                    Playlists = MusicDataBase.GetPlaylistsForUser(PageUserID);
                    Bio = MusicDataBase.GetBioForUserID(PageUserID);
                }

                if(isArtist)
                {
                    Session.SetString("CurrentPageIsArtist", "TRUE");
                    Albums = MusicDataBase.GetAlbumsFromArtistID(PageUserID);
                }
                else
                {
                    Response.Redirect("./Index");
                }
            }
            else
            {
                if(Session.GetString("IsLoggedIn") != null)
                {
                    Username = Session.GetString("SessionUser");
                    PageUserID = (int) Session.GetInt32("UserID");
                    Session.SetInt32("PlaylistUserID", PageUserID);
                    Playlists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));
                    Bio = MusicDataBase.GetBioForUserID((int) Session.GetInt32("UserID"));
                }
                else
                {
                    Response.Redirect("./Index");
                }
            }
            
            // TODO: Query here for playlists using username!
        }

        public IActionResult OnPostViewPlaylist(string viewPlaylist)
        {
            if(ModelState.IsValid)
            {
                if(viewPlaylist == null || viewPlaylist == String.Empty) return Page();
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");

                Playlists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));

                if(IsValidPlaylist(viewPlaylist))
                {
                    return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={viewPlaylist}");
                }

                return Page();
            }
            return RedirectToPage("./Error");
        }

        private bool IsValidPlaylist(string viewPlaylist)
        {
            foreach(var v in Playlists)
            {
                if(v.PlaylistTitle.Equals(viewPlaylist))
                {
                    return true;
                }
            }

            return false;
        }

        public IActionResult OnPostCreateNewPlaylist()
        {
            if(ModelState.IsValid)
            {
                return RedirectToPage("./CreatePlaylist");
            }
            return RedirectToPage("./Error");
        }
        public IActionResult OnPostLogOut()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.Clear();
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Error");
        }
        public IActionResult OnPostEditBio()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "TRUE");
                Username = Session.GetString("SessionUser");
                PageUserID = (int) Session.GetInt32("UserID");
                Playlists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));
                Bio = MusicDataBase.GetBioForUserID((int) Session.GetInt32("UserID"));
                return Page();
            }
            return RedirectToPage("./Error");
        }
        public IActionResult OnPostSubmitEditedBio()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Bio = Request.Form["user-bio-text"];
                if(Bio == null)
                {
                    Bio = String.Empty;
                }

                if(Bio.Length <= 100)
                {
                    MusicDataBase.ChangeBio(Bio, Session.GetInt32("UserID"));
                    Session.SetString("IsEditMode", "FALSE");
                }
                Username = Session.GetString("SessionUser");
                PageUserID = (int) Session.GetInt32("UserID");
                Playlists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));
                Bio = MusicDataBase.GetBioForUserID((int) Session.GetInt32("UserID"));
                return Page();
            }
            return RedirectToPage("./Error");
        }
    }
}