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

            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            command.Dispose();

            Close();
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

        // FIXME: Needs fixing.
        public static void AddSong(string title, string albumTitle, int? artistID, string releaseDate)
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

                /*command.Parameters.Add(new OracleParameter("songTitle", OracleDbType.Varchar2).Value = title);
                command.Parameters.Add(new OracleParameter("releaseDate", OracleDbType.Varchar2).Value = releaseDate);
                command.Parameters.Add(new OracleParameter("albumID", OracleDbType.Int32).Value = albumID);
                command.Parameters.Add(new OracleParameter("artistID", OracleDbType.Int32).Value = artistID);
                 */
                Console.WriteLine(ReadInt(command)[0]);
            }
            catch (OracleException e)
            {
                Console.WriteLine($"AddSong Error: {e.Message}");
            }

            Close();
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
                list.Add(reader.GetString(0));
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