using System;
using System.Data.SqlClient;

namespace otel
{
    internal class DatabaseHelper
    {
        // Bağlantı dizesi (connection string) burada tanımlanır
        private static string connectionString = "Data Source=LAPTOP-T3CP9FH3\\SQLEXPRESS05;Initial Catalog=Otel;Integrated Security=True;";

        // Bağlantıyı döndüren statik bir metot
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
