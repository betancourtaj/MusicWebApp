using System;

namespace App.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { private get; set; }
        public string Email {get; set;}
        public string DisplayName {get; set;}

        public User(string userName, string password, string email, string displayName) {
            UserName = userName;
            Password = password;
            Email = email;
            DisplayName = displayName;
        }

    }
}