using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.OracleClient;

namespace App
{
    public static class MusicDataBase
    {
        private static OracleConnection connection;

        public static void Connect()
        {
            connection = new OracleConnection(Constants.ConnectionString);
        }

        public static string GetBioForUserID(int userid)
        {
            string[] array = null;
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetBioForUserID.sql");

                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = userid;

                array = Read(command);

                if(array != null)
                {
                    Close();
                    return array[0];
                }
            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static void AddArtist(int? id) {
            if (id == null)
            return;

            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("AddArtist.sql");
                command.Parameters.Add(new OracleParameter("id", id));

                command.ExecuteNonQuery();

                Close();
            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            command.Dispose();

            Close();
        }

        public static string[] GetPlaylistNamesForUserID(int userid)
        {
            string[] array = null;
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetPlaylistsForUserID.sql");

                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = userid;

                array = Read(command);

                if(array != null)
                {
                    Close();
                    return array;
                }
            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static string GetUserNameForID(int userId)
        {   
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetUsernameForID.sql");
                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = userId;

                string[] names = Read(command);

                if(names == null)
                {
                    Close();
                    return null;
                }

                Close();
                return names[0];

            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static Dictionary<string, string> GetSearchResults(string data)
        {
            if(data == null) return new Dictionary<string, string>();

            Dictionary<string, string> results = new Dictionary<string, string>();

            string[] albums = GetSearchResultsAlbum(data);
            string[] songs = GetSearchResultsSong(data);

            if(albums != null) 
            {
                foreach(string name in albums)
                {
                    results.Add(name, "Album");
                }
            }
            if(songs != null)
            {
                foreach(string name in songs)
                {
                    results.Add(name, "Song");
                }
            }

            if(albums == null && songs == null)
            {
                return new Dictionary<string, string>();
            }
            // TODO: ADD PLAYLIST SEARCH
            return results;
        }

        private static string[] GetSearchResultsSong(string data)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                // FIXME: THIS IS VERY DANGEROUS AND SQL INJECTION CAN BE USED.
                command.CommandText = $"select title from p_song where title like '%{data}%'";

                string[] array = Read(command);

                if(array == null)
                {
                    Close();
                    return null;
                }

                Close();
                return array;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        private static string[] GetSearchResultsAlbum(string data)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                // FIXME: THIS IS VERY DANGEROUS AND SQL INJECTION CAN BE USED.
                command.CommandText = $"select title from p_album where title like '%{data}%'";
                
                string[] array = Read(command);

                if(array == null)
                {
                    Close();
                    return null;
                }

                Close();
                return array;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static void AddAlbum(string albumName, int? artistID, string releaseDate)
        {
            if(artistID == null) return;

            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("AddAlbum.sql");
                command.Parameters.Add("albumID", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters.Add("albumTitle", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add("releaseDate", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add("artistID", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters.Add("albumID", OracleDbType.Int32, ParameterDirection.Input);
                int nextAlbumID = GetNextAlbumID();
                command.Parameters[0].Value = nextAlbumID;
                command.Parameters[1].Value = albumName;
                command.Parameters[2].Value = releaseDate;
                command.Parameters[3].Value = artistID;   
                command.Parameters[4].Value = nextAlbumID;

                command.ExecuteNonQuery();
                Close();
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
        }

        private static int GetNextAlbumID()
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetNewAlbumID.sql");

                command.Parameters.Add("albumIdValue", OracleDbType.Int32, ParameterDirection.Output);
                command.ExecuteNonQuery();

                if(command.Parameters[0].Value != null)
                {
                    Close();
                    return Convert.ToInt32(command.Parameters[0].Value.ToString());
                }
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return -1;
        }

        public static string[] GetAlbumsForArtist(int? artistID)
        {
            if(artistID == null)
            return null;

            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();

                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("FindAlbums.sql");
                command.Parameters.Add(new OracleParameter("id", artistID));

                string[] array = Read(command);

                if(array == null)
                {
                    Close();
                    return null;
                }

                Close();
                return array;
            }
            catch(OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static void AddSong(string songTitle, string albumTitle, int? artistID, string releaseDate)
        {
            if (artistID == null){
                return;
            }
            int albumID = GetAlbumID(albumTitle, artistID);
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("AddSong.sql");
                //command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(":songID", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add(":songTitle", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add(":releaseDate", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add(":albumID", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters.Add(":artistID", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters.Add(":songID", OracleDbType.Varchar2, ParameterDirection.Input);
                int songID = GetCurrentSongID();
                command.Parameters[0].Value = songID;
                command.Parameters[1].Value = songTitle;
                command.Parameters[2].Value = releaseDate;
                command.Parameters[3].Value = albumID;
                command.Parameters[4].Value = artistID;
                command.Parameters[5].Value = songID;

                command.ExecuteNonQuery();
                Close();
            }
            catch (OracleException e)
            {
                Console.WriteLine($"AddSong Error: {e.Message}");
            }

            Close();
        }

        private static int GetCurrentSongID()
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetCurrentSongID.sql");

                command.Parameters.Add("songIdValue", OracleDbType.Int32, ParameterDirection.Output);
                command.ExecuteNonQuery();

                if(command.Parameters[0].Value != null)
                {
                    Close();
                    return Convert.ToInt32(command.Parameters[0].Value.ToString());
                }
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return -1;
        }

        private static int GetAlbumID(string title, int? artistID) {

            if(artistID == null)
            return 0;

            int[] array = null;
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetAlbumID.sql");

                command.Parameters.Add(new OracleParameter("title", title));
                command.Parameters.Add(new OracleParameter("artistid", artistID));

                array = ReadInt(command);
                Close();

            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return Convert.ToInt32(array[0]);
        }

        public static bool UserIsArtist(int? artistid)
        {

            if(artistid == null)
            {
                return false;
            }

            Connect();
            
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("UserIsArtist.sql");
                
                command.Parameters.Add("artistid", artistid);

                int[] array = ReadInt(command);

                if(array == null)
                {
                    Close();
                    return false;
                }

                Close();
                return true;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return false;
        }

        public static int GetUserIDForEmail(string email)
        {
            int[] array = null;
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetUserIDForEmail.sql");

                command.Parameters.Add(new OracleParameter("email", email));

                array = ReadInt(command);
                Close();

            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return Convert.ToInt32(array[0]);
        }

        public static string GetUsernameForEmail(string email)
        {
            string[] array = null;
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetUserForEmail.sql");

                command.Parameters.Add(new OracleParameter("email", email));

                array = Read(command);
                Close();
                
            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return array[0];
        }

        public static bool PasswordsMatch(string email, string password)
        {
            password = Constants.HashMe(password);
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;
                command.CommandText = Constants.ReadSqlTextFromFile("CheckPasswords.sql");
                command.Parameters.Add("email", email);
                string[] array = Read(command);

                if(array == null)
                return false;

                if(password == array[0])
                {
                    Close();
                    return true;
                }
            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return false;
        }

        private static string[] Read(OracleCommand command)
        {
            List<string> list = new List<string>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                if(!reader.IsDBNull(0))
                {
                    list.Add((string) reader.GetString(0));
                }
            }
            reader.Dispose();

            if(list.Count == 0)
            return null;

            return list.ToArray();
        }

        private static int[] ReadInt(OracleCommand command)
        {
            List<int> list = new List<int>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            reader.Dispose();

            if(list.Count == 0)
            return null;

            return list.ToArray();
        }

        public static bool UserExists(string email)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("CheckExistingEmail.sql");

                command.Parameters.Add(new OracleParameter("email", email));
                
                if(Read(command) != null) {
                    Close();
                    return true;
                }
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            Close();
            return false;
        }

        public static void AddUser(string email, string username, string password)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("AddUser.sql");

                command.Parameters.Add(new OracleParameter("email", email));
                command.Parameters.Add(new OracleParameter("username", username));
                command.Parameters.Add(new OracleParameter("passwd", Constants.HashMe(password)));

                command.ExecuteNonQuery();

            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            Close();
        }

        public static void Close()
        {
            connection.Dispose();
        }
    }
}