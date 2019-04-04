using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
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

        public static void SearchSong(Song song)
        {
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = @"select s.title
                                        from p_songs
                                        where s.title = :title and
                                              s.album = :album and
                                              s.artist = :artist and 
                                              s.date = :date and 
                                              s.length = :length;";

                OracleParameter title = new OracleParameter("title", song.Title);
                OracleParameter album = new OracleParameter("album", song.Album);
                OracleParameter artist = new OracleParameter("artist", song.Artist);
                OracleParameter date = new OracleParameter("date", song.Date);
                OracleParameter length = new OracleParameter("length", song.Length);

                command.Parameters.Add(title);
                command.Parameters.Add(album);
                command.Parameters.Add(artist);
                command.Parameters.Add(date);
                command.Parameters.Add(length);

                Read(command);

            } catch (OracleException ex)
            {
                Console.WriteLine($"ORACLE EXCEPTION: {ex.Message}");
            }
            command.Dispose();
        }

        public static void AddSong(Song song)
        {
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = @"insert into p_songs 
                                        values (s.title = :title and
                                              s.album = :album and
                                              s.artist = :artist and 
                                              s.date = :date and 
                                              s.length = :length);";

                OracleParameter title = new OracleParameter("title", song.Title);
                OracleParameter album = new OracleParameter("album", song.Album);
                OracleParameter artist = new OracleParameter("artist", song.Artist);
                OracleParameter date = new OracleParameter("date", song.Date);
                OracleParameter length = new OracleParameter("length", song.Length);

                command.Parameters.Add(title);
                command.Parameters.Add(album);
                command.Parameters.Add(artist);
                command.Parameters.Add(date);
                command.Parameters.Add(length);

                Read(command);

            } catch (OracleException ex)
            {
                Console.WriteLine($"ORACLE EXCEPTION: {ex.Message}");
            }
            command.Dispose();
        }

        private static void Read(OracleCommand command)
        {
            OracleDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
            reader.Dispose();    
        }

        public static void Close()
        {
            connection.Dispose();
        }
    }
}