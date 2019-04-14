using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace App
{
    public static class Constants
    {
        public static string ConnectionString { get; set; }
        private static string SqlQueryPath = "SqlQueries/";

        static Constants()
        {
            byte[] bytes = File.ReadAllBytes("ConnectionFile");

            ConnectionString = Encoding.UTF8.GetString(bytes);
        }

        public static string ReadSqlTextFromFile(string filename)
        {
            return File.ReadAllText(SqlQueryPath + filename);
        }

        public static string HashMe(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);
            byte[] result;
            SHA1 sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(data);
            return Encoding.UTF8.GetString(result);
        }
    }
}