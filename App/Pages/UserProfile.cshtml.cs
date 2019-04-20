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

        private ISession Session;

        public void OnGet(string userId)
        {
            Session = HttpContext.Session;
            if(Session.GetString("IsLoggedIn") == null)
            {
                Response.Redirect("./Index");
            }
            Session.SetString("IsEditMode", "FALSE");
            if(userId != null)
            {
                int id = Convert.ToInt32((string) userId);
                string usrName = MusicDataBase.GetUserNameForID(id);

                if(usrName != null)
                {
                    Username = usrName;
                    Playlists = MusicDataBase.GetPlaylistNamesForUserID(id);
                    Bio = MusicDataBase.GetBioForUserID(id);
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

        public IActionResult OnPostViewPlaylist()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                return Redirect($"./ViewPlaylist?userName={Session.GetString("SessionUser")}");
                //return RedirectToPage("./ViewPlaylist", "OnGet", Session.GetString("SessionUser"));
                //return new RedirectToPageResult("./ViewPlaylist", "userName", Session.GetString("SessionUser"));
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
            return RedirectToPage("./Error");

        }
        public IActionResult OnPostEditBio()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "TRUE");
                return Page();
            }
            return RedirectToPage("./Error");
        }
        public IActionResult OnPostSubmitEditedBio()
        {
            if(ModelState.IsValid)
            {
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");
                return Page();
            }
            return RedirectToPage("./Error");
        }
    }
}