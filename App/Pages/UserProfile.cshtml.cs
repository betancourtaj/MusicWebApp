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

        public int PlaylistChosenID { get; private set; }

        private int AlbumChosenID;

        private ISession Session;

        public void OnGet(string userId)
        {
            Session = HttpContext.Session;
            Session.SetString("IsEditMode", "FALSE");
            if(userId != null)
            {
                PageUserID = Convert.ToInt32((string) userId);
                string usrName = MusicDataBase.GetUserNameForID(PageUserID);
                Session.SetInt32("PlaylistUserID", PageUserID);
                Session.SetInt32("AlbumUserID", PageUserID);

                if(usrName != null)
                {
                    Username = usrName;
                    Session.SetString("PageUserName", Username);
                    Playlists = MusicDataBase.GetPlaylistsForUser(PageUserID);
                    Bio = MusicDataBase.GetBioForUserID(PageUserID);
                }

                if(Session.GetString("IsArtist") == "TRUE")
                {
                    Albums = MusicDataBase.GetAlbumsFromArtistID(PageUserID);
                }
            }
            else
            {
                if(Session.GetString("IsLoggedIn") != null)
                {
                    PageUserID = (int) Session.GetInt32("UserID");
                    Session.SetInt32("PlaylistUserID", PageUserID);
                    Session.SetInt32("AlbumUserID", PageUserID);
                    Username = Session.GetString("SessionUser");
                    Session.SetString("PageUserName", Username);
                    Playlists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("UserID"));
                    Bio = MusicDataBase.GetBioForUserID((int) Session.GetInt32("UserID"));

                    if(Session.GetString("IsArtist") == "TRUE")
                    {
                        Albums = MusicDataBase.GetAlbumsFromArtistID(PageUserID);
                    }
                }
                else
                {
                    Response.Redirect("./Index");
                }
            }
        }

        public IActionResult OnPostViewPlaylist(string viewPlaylist)
        {
            if(ModelState.IsValid)
            {
                if(viewPlaylist == null || viewPlaylist == String.Empty) return Page();
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");

                Playlists = MusicDataBase.GetPlaylistsForUser((int) Session.GetInt32("PlaylistUserID"));

                if(IsValidPlaylist(viewPlaylist))
                {
                    return Redirect($"./ViewPlaylist?userId={Session.GetInt32("PlaylistUserID")}&playlist={viewPlaylist}&playlistid={PlaylistChosenID}");
                }

                return RedirectToPage("./UserProfile");
            }
            return RedirectToPage("./Error");
        }

        public IActionResult OnPostViewAlbum(string viewAlbum)
        {
            if(ModelState.IsValid)
            {
                if(viewAlbum == null || viewAlbum == string.Empty) return Page();
                Session = HttpContext.Session;
                Session.SetString("IsEditMode", "FALSE");

                Albums = MusicDataBase.GetAlbumsFromArtistID((int) Session.GetInt32("AlbumUserID"));

                if(IsValidAlbum(viewAlbum))
                {
                    return Redirect($"./ViewAlbum?album={viewAlbum}&albumId={AlbumChosenID}");
                }

                return RedirectToPage("./UserProfile");
            }

            return RedirectToPage("./Error");
        }

        private bool IsValidAlbum(string viewAlbum)
        {
            foreach(var v in Albums)
            {
                if(v.AlbumTitle.Equals(viewAlbum))
                {
                    AlbumChosenID = v.AlbumID;
                    return true;
                }
            }

            return false;
        }

        private bool IsValidPlaylist(string viewPlaylist)
        {
            foreach(var v in Playlists)
            {
                if(v.PlaylistTitle.Equals(viewPlaylist))
                {
                    PlaylistChosenID = v.PlaylistId;
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
                return RedirectToPage("./UserProfile");
            }
            return RedirectToPage("./Error");
        }
    }
}