using System;
using System.IO;
using System.Text;

namespace App
{
    public static class Constants
    {
        public static string ConnectionString { get; set; }

        public static void Generate()
        {
            byte[] bytes = File.ReadAllBytes("ConnectionFile");

            ConnectionString = Encoding.UTF8.GetString(bytes);

        }
    }
}