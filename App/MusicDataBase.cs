using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.OracleClient;
using App.Models;

namespace App
{
    public static class MusicDataBase
    {
        private static OracleConnection connection;

        public static void Connect()
        {
            connection = new OracleConnection(Constants.ConnectionString);
        }

        public static Playlist[] GetPlaylistsForUser(int userid)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetAllPlaylistsForUser.sql");
                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = userid;

                Playlist[] comments = ReadPlaylists(command);

                if(comments != null)
                {
                    Close();
                    return comments;
                }

                Close();
                return null;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static Comment[] GetCommentsForPlaylist(int userid, string playlist)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                int playlistid = GetPlaylistIDForUser(userid, playlist);
                command.CommandText = Constants.ReadSqlTextFromFile("GetCommentsForPlaylistID.sql");
                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = playlistid;

                Comment[] comments = ReadComments(command);

                if(comments != null)
                {
                    Close();
                    return comments;
                }

                Close();
                return null;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        private static Comment[] ReadComments(OracleCommand command)
        {
            List<Comment> commentList = new List<Comment>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                if(!reader.IsDBNull(0))
                {
                    commentList.Add(new Comment(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), GetUserNameForID(reader.GetInt32(3))));
                }
            }
            reader.Dispose();

            if(commentList.Count == 0)
            return null;

            return commentList.ToArray();
        }

        private static Playlist[] ReadPlaylists(OracleCommand command)
        {
            List<Playlist> playlistList = new List<Playlist>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                if(!reader.IsDBNull(0))
                {
                    playlistList.Add(new Playlist(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                }
            }
            reader.Dispose();

            if(playlistList.Count == 0)
            return null;

            return playlistList.ToArray();
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

        public static Song[] GetSongsFromPlaylist(int id, string playlist)
        {
            if(playlist == null) return null;

            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                int[] songids = GetSongIDsFromPlaylist(id, playlist);
                List<Song> songs = new List<Song>();

                if(songids != null)
                {
                    foreach(var sid in songids)
                    {
                        command.CommandText = Constants.ReadSqlTextFromFile("GetSongsFromPlaylist.sql");
                        command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                        command.Parameters[0].Value = sid;

                        songs.Add(ReadSong(command));
                    }

                    Close();
                    return songs.ToArray();
                }
                
                Close();
                return null;

            }
            catch(OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }



        internal static int[] GetSongIDsFromPlaylist(int id, string playlist)
        {
            if(playlist == null) return null;
            
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                int playlistid = GetPlaylistIDForUser(id, playlist);

                command.CommandText = Constants.ReadSqlTextFromFile("GetSongIDsForPlaylist.sql");
                command.Parameters.Add("id", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters[0].Value = playlistid;

                int[] array = ReadIntArray(command);
                if(array != null)
                {
                    Close();
                    return array;
                }

                Close();
                return null;
            }
            catch(OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        private static int GetPlaylistIDForUser(int id, string playlist)
        {
            if (playlist == null) return 0;

            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetPlaylistIDForUser.sql");
                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters.Add("albumtitle", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters[0].Value = id;
                command.Parameters[1].Value = playlist;

                int[] playlistIDs = ReadIntArray(command);

                if(playlistIDs != null)
                {
                    Close();
                    return playlistIDs[0];
                }

                Close();
                return 0;
            } catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            command.Dispose();

            Close();
            return 0;
        }

        private static Song ReadSong(OracleCommand command)
        {
            List<Song> songList = new List<Song>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                if(!reader.IsDBNull(0))
                {
                    songList.Add(new Song(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), GetAlbumNameForSongId(reader.GetInt32(3))));
                }
            }
            reader.Dispose();

            if(songList.Count == 0)
            return null;

            return songList[0];
        }

        public static string GetAlbumNameForSongId(int albumid)
        {
            Connect();
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetAlbumNameForSongId.sql");

                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = albumid;

                string[] array = Read(command);
                if(array != null)
                {
                    Close();
                    return array[0];
                }

                Close();
                return null;
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
            Song[] songs = GetSearchResultsSong(data);

            if(albums != null) 
            {
                foreach(string name in albums)
                {
                    results.Add(name, "Album");
                }
            }
            if(songs != null)
            {
                foreach(var name in songs)
                {
                    results.Add(name.Title, "Song");
                }
            }

            if(albums == null && songs == null)
            {
                return new Dictionary<string, string>();
            }
            // TODO: ADD PLAYLIST SEARCH
            return results;
        }

        internal static void ChangeBio(string bio, int? userid)
        {
            if(bio == null) return;
            if(userid == null) return;

            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("ChangeBio.sql");
                command.Parameters.Add("biotext", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = bio;
                command.Parameters[1].Value = userid;

                command.ExecuteNonQuery();
                Close();
            }
            catch(OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
        }

        internal static void ChangeComment(string comment, int commentid)
        {
            if(comment == null) return;

            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("UpdateComment.sql");
                command.Parameters.Add("commentText", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add("id", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = comment;
                command.Parameters[1].Value = commentid;

                command.ExecuteNonQuery();
                Close();
            }
            catch(OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
        }

        public static int FindArtistIDFromAlbumID(int albumID) {
            Connect();
            int artistID = 0;

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetArtistFromAlbumID.sql");
                command.Parameters.Add("albumid", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = albumID;

                artistID =  ReadSingleInt(command);
            
                Close();
                return artistID;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return -1;
        }

        public static int FindArtistID(int songID) 
        {
            Connect();
            int artistID = 0;

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetArtistFromSongID.sql");
                command.Parameters.Add("songid", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = songID;

                artistID =  ReadSingleInt(command);
            
                Close();
                return artistID;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return -1;
        }

        public static string[] GetSearchResultsUsers(string data) {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetUsersForUsernameLike.sql");
                command.Parameters.Add("dataString", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters[0].Value = data;

                string[] users = Read(command);

                if(users == null)
                {
                    Close();
                    return null;
                }

                Close();
                return users;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        public static Song[] GetSearchResultsSong(string data)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetSongForTitleLike.sql");
                command.Parameters.Add("dataString", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters[0].Value = data;

                Song[] songs = ReadSongArray(command);

                if(songs == null)
                {
                    Close();
                    return null;
                }

                Close();
                return songs;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return null;
        }

        private static Song[] ReadSongArray(OracleCommand command)
        {
            List<Song> songList = new List<Song>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                if(!reader.IsDBNull(0))
                {
                    songList.Add(new Song(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), GetAlbumNameForSongId(reader.GetInt32(3))));
                }
            }
            reader.Dispose();

            if(songList.Count == 0)
            return null;

            return songList.ToArray();
        }

        public static string[] GetSearchResultsAlbum(string data)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetAlbumForTitleLike.sql");
                command.Parameters.Add("dataString", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters[0].Value = data;
                
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

        public static int GetPlaylistIDForPlaylistNameAndUserID(string playlistName, int userId) {
            Connect();
            
            int playlistID = -1;

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("GetPlaylistIDForPlaylistNameAndUserID.sql");
                command.Parameters.Add("playlistName", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters.Add("userId", OracleDbType.Varchar2, ParameterDirection.Input);
                command.Parameters[0].Value = playlistName;
                command.Parameters[1].Value = userId;


                playlistID =  ReadSingleInt(command);
            
                Close();
                return playlistID;
            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }

            Close();
            return -1;
        }

        public static void AddSongToPlaylist(int songID, int playlistID) 
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("AddSong.sql");
                //command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("songID", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters.Add("playlistID", OracleDbType.Int32, ParameterDirection.Input);
                command.Parameters[0].Value = songID;
                command.Parameters[1].Value = playlistID;

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

                array = ReadIntArray(command);
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

                int[] array = ReadIntArray(command);

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

                array = ReadIntArray(command);
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

        private static int[] ReadIntArray(OracleCommand command)
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

        private static int ReadSingleInt(OracleCommand command)
        {
            List<int> list = new List<int>();

            OracleDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            reader.Dispose();

            if(list.Count == 0)
            return 0;

            return list[0];
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

        //TODO: implement this 
        public static void ChangePassword(string email, string username, string newPassword) {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("ChangePassword.sql");

                command.Parameters.Add(new OracleParameter("passwd", Constants.HashMe(newPassword)));
                command.Parameters.Add(new OracleParameter("email", email));

                command.ExecuteNonQuery();

            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            Close();
        }

        public static void ChangeUsername(string email, string newUsername, string password) {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("ChangeUsername.sql");

                command.Parameters.Add(new OracleParameter("email", email));
                command.Parameters.Add(new OracleParameter("username", newUsername));
                //command.Parameters.Add(new OracleParameter("passwd", Constants.HashMe(password)));

                command.ExecuteNonQuery();

            }
            catch (OracleException e)
            {
                Console.WriteLine(e.Message);
            }
            Close();
        }

        public static void addBio(string email, string bio) {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = Constants.ReadSqlTextFromFile("ChangeUsername.sql");

                command.Parameters.Add(new OracleParameter("bio", bio));
                command.Parameters.Add(new OracleParameter("email", email));
                //command.Parameters.Add(new OracleParameter("passwd", Constants.HashMe(password)));

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