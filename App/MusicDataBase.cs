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
                                              s.length = :length";

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

        public static void AddSong(string title, string album, string artist, string sdate, string slength)
        {
            Connect();

            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = @"insert into test_songs (id, title, album, artist, sdate, slength) values (:id, :title, :album, :artist, :sdate, :slength)";

                OracleParameter id = new OracleParameter("id", 1);
                OracleParameter vtitle = new OracleParameter("title", title);
                OracleParameter valbum = new OracleParameter("album", album);
                OracleParameter vartist = new OracleParameter("artist", artist);
                OracleParameter vdate = new OracleParameter("sdate", sdate);
                OracleParameter vlength = new OracleParameter("slength", slength);

                command.Parameters.Add(id);
                command.Parameters.Add(vtitle);
                command.Parameters.Add(valbum);
                command.Parameters.Add(vartist);
                command.Parameters.Add(vdate);
                command.Parameters.Add(vlength);

                //Read(command);
                try {
                    command.ExecuteNonQuery();
                }
                catch (OracleException e)
                {
                    Console.WriteLine(e.Message);
                }

            } catch (OracleException ex)
            {
                Console.WriteLine($"ORACLE EXCEPTION: {ex.Message}");
                Console.WriteLine(ex.Errors);
                Console.WriteLine(command.CommandText);
            }
            command.Dispose();

            Close();
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