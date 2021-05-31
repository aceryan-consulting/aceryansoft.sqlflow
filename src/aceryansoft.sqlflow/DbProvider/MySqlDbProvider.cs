using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System;


namespace aceryansoft.sqlflow
{
    internal class MySqlDbProvider : IDbProvider
    {
        public void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100)
        {
            throw new System.NotImplementedException();
        }

        public DbConnection CreateDbConnexion(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public DbParameter CreateDbParameter(string name, object value, bool isOutputParameter = false)
        {
            return new MySqlParameter() { ParameterName = name, Value = value, Direction = isOutputParameter ? ParameterDirection.Output : ParameterDirection.Input };
        }
    }
}
