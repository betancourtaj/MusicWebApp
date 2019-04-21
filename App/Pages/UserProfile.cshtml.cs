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

        [BindProperty]
        public string Username { get; private set; }

        [BindProperty]
        public string[] Playlists { get; private set; }

        [BindProperty]
        public string Bio { get; private set; }

        [BindProperty]
        public int PageUserID { get; private set; }

        private ISession Session;

        public void OnGet(string userId)
        {
            Session = HttpContext.Session;
            Session.SetString("IsEditMode", "FALSE");
            if(userId != null)
            {
                PageUserID = Convert.ToInt32((string) userId);
                string usrName = MusicDataBase.GetUserNameForID(PageUserID);

                if(usrName != null)
                {
                    Username = usrName;
                    Playlists = MusicDataBase.GetPlaylistNamesForUserID(PageUserID);
                    Bio = MusicDataBase.GetBioForUserID(PageUserID);
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
                    Playlists = MusicDataBase.GetPlaylistNamesForUserID((int) Session.GetInt32("UserID"));
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
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");

                return Redirect($"./ViewPlaylist?userId={Session.GetInt32("UserID")}&playlist={viewPlaylist}");
            }
            return RedirectToPage("./Error");
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
                Playlists = MusicDataBase.GetPlaylistNamesForUserID((int) Session.GetInt32("UserID"));
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
                Playlists = MusicDataBase.GetPlaylistNamesForUserID((int) Session.GetInt32("UserID"));
                Bio = MusicDataBase.GetBioForUserID((int) Session.GetInt32("UserID"));
                return Page();
            }
            return RedirectToPage("./Error");
        }
    }
}