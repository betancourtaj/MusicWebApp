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
    public class ViewAlbumModel : PageModel
    {
        [BindProperty]
        public int UserID { get; private set; }

        [BindProperty]
        public string AlbumName { get; private set; }

        [BindProperty]
        public Song[] Songs { get; private set; }

        [BindProperty]
        public string ArtistName { get; private set; }

        private int AlbumID;

        private ISession Session;

        public void OnGet(int userId, string album, int albumId)
        {
            Session = HttpContext.Session;

            UserID = userId;
            Session.SetInt32("AlbumUserID", UserID);
            AlbumName = album;
            Session.SetString("AlbumName", AlbumName);
            AlbumID = albumId;
            Session.SetInt32("AlbumID", AlbumID);
            ArtistName = Session.GetString("PageUserName");
            
            QuerySongs();
        }

        private void QuerySongs()
        {
            Songs = MusicDataBase.GetSongsFromAlbum((int) Session.GetInt32("AlbumID"));
        }
    }
}