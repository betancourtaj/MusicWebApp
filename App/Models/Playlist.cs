using System;

namespace App.Models
{
    public class Playlist
    {
        public int ID { get; private set; }
        public string Title { get; private set; }
        public int UserID { get; private set; }

        public Playlist(int id, string title, int userid)
        {
            ID = id;
            Title = title;
            UserID = userid;
        }
    }
}