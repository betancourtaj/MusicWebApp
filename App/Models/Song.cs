using System;

namespace App.Models
{
    public class Song
    {
        public string Title { get; set; } = null;
        public string Album { get; set; } = null;
        public string Artist { get; set; } = null;
        public string Date{ get; set; } = null;
        public double Length { get; set; } = 0.0d;

        public Song(string title, string album, string artist, string date, double length)
        {
            Title = title;
            Album = album;
            Artist = artist;
            Date = date;
            Length = length;
        }
    }
}