using System;

namespace App.Models
{
    public class Artist : User {
        public string StageName {get; set;}

        public Artist(string userName, string password, string email, string displayName, string stageName) : base(userName, password, email, displayName)
        {
            StageName = stageName;
        }
    }
}