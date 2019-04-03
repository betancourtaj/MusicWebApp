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

        public static void AddSongCommand(Song song) {            
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = "insert into pr_songs :song";

                OracleParameter songParameter = new OracleParameter("song", song);
                command.Parameters.Add(songParameter);

                Read(command);

            } catch (OracleException ex)
            {
                Console.WriteLine($"ORACLE EXCEPTION: {ex.Message}");
            }
            command.Dispose();
        }

        public static void Generic()
        {
            OracleCommand command = connection.CreateCommand();
            try {
                connection.Open();
                command.BindByName = true;

                command.CommandText = "select first_name from employees where department_id = :id";

                OracleParameter id = new OracleParameter("id", 50);
                command.Parameters.Add(id);

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