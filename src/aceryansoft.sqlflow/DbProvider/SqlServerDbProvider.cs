using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
//todo yannick migrate from System.Data.SqlClient to Microsoft.Data.SqlClient=> require Trust Server Certificate=True, or Encrypt =false to work

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

        public DbParameter CreateDbParameter(string name, object value, bool isOutputParameter = false)
        {
            return new SqlParameter() { ParameterName = name, Value = value, Direction = isOutputParameter ? ParameterDirection.Output : ParameterDirection.Input };
        }
    }
}
