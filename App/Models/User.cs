namespace App.Models
{
    public class User
    {
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }

        public User(int userid, string email, string username)
        {
            UserID = userid;
            Email = email;
            Username = username;
        }
    }
}