using System;

namespace App.Models
{
    public class Comment
    {
        public string Text { get; private set; }
        public int CommentID { get; private set; }
        public int UserID { get; private set; }
        public int PlaylistID { get; private set; }
        public string Username { get; private set; }

        public Comment(int commentid, string text, int playlistid, int userid, string userName)
        {
            CommentID = commentid;
            Text = text;
            PlaylistID = playlistid;
            UserID = userid;
            Username = userName;
        }
    }
}