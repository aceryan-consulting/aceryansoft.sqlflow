using MySql.Data.MySqlClient;
using Npgsql;
using System.Data;
using System.Data.Common;


namespace aceryansoft.sqlflow
{
    internal class PostGreSqlDbProvider : IDbProvider
    {
        public void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100)
        {
            throw new System.NotImplementedException();
        }

        public DbConnection CreateDbConnexion(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        public DbParameter CreateDbParameter(string name, object value)
        {
            return new NpgsqlParameter() { ParameterName = name, Value = value };
        }
    }
}
