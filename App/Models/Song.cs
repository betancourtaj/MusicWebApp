using System;

namespace App.Models
{
    public class Song
    {
        public string Title { get; set; } = null;
        public string Album { get; set; } = null;
        public string Artist { get; set; } = null;
        public string sDate{ get; set; } = null;
        public string sLength { get; set; }

        public Song(string title, string album, string artist, string date, string length)
        {
            Title = title;
            Album = album;
            Artist = artist;
            sDate = date;
            sLength = length;
        }
    }
}