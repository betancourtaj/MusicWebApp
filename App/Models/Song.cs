using System;

namespace App.Models
{
    public class Song
    {
        public string Title { private get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public DateTime Date{ get; set; }
        public int Length { get; set; }

        public Song(string title, string album, string artist, DateTime date, int length)
        {
            Title = title;
            Album = album;
            Artist = artist;
            Date = date;
            Length = length;
        }
    }
}