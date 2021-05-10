using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace aceryansoft.sqlflow
{
    internal class SqlServerDbProvider : IDbProvider
    {
        public void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100)
        {
            using (var bulkCopy = new SqlBulkCopy((SqlConnection)dbconnexion))
            {
                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = targetTable;
                bulkCopy.WriteToServer(data);
            }
        }

        public DbConnection CreateDbConnexion(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public DbParameter CreateDbParameter(string name, object value)
        {
            return new SqlParameter() { ParameterName = name, Value = value };
        }
    }
}
