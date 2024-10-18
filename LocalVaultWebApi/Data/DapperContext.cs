using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SQLite;

namespace LocalVaultWebApi.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _secretValue;
        public DapperContext(IConfiguration configuration, string secretValue)
        {
            _configuration = configuration;
            _secretValue = secretValue;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Initialize SQLiteCipher (this can be done here or in a more central location)
            SQLitePCL.Batteries_V2.Init();  // This ensures SQLiteCipher is ready
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _connectionString,  // Path to your SQLite database file
                Mode = SqliteOpenMode.ReadWriteCreate  // Allow reading and writing to the DB
            }.ToString();

            var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Set the encryption key (PRAGMA key) after opening the connection
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"PRAGMA key = '{_secretValue}';";  // Set your encryption password
                command.ExecuteNonQuery();
            }

            return connection;  // Return the opened, encrypted connection ready for Dapper
        }
    }
}
