using Npgsql;
using System.Data;

namespace SocialAppViewer
{
    public class PostgresTableDataFactory : ITableDataFactory
    {
        private readonly string _connectionString;

        public PostgresTableDataFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable LoadTable(string tableName)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var query = $"SELECT * FROM \"{tableName}\"";
            using var adapter = new NpgsqlDataAdapter(query, conn);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        public void SaveTable(string tableName, DataTable table)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            string quotedTable = $"\"{tableName}\"";
            using var adapter = new NpgsqlDataAdapter($"SELECT * FROM {quotedTable}", conn);
            var builder = new NpgsqlCommandBuilder(adapter)
            {
                QuotePrefix = "\"",
                QuoteSuffix = "\""
            };

            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();

            adapter.Update(table);
        }
    }
}