using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace aceryansoft.sqlflow
{
    internal class OracleDbProvider : IDbProvider
    {
        public void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100)
        {
            using (var bulkCopy = new OracleBulkCopy((OracleConnection)dbconnexion))
            {
                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = targetTable;
                bulkCopy.WriteToServer(data);
            }
        }

        public DbConnection CreateDbConnexion(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        public DbParameter CreateDbParameter(string name, object value, bool isOutputParameter = false)
        {
            return new OracleParameter() { ParameterName = name, Value = value, Direction = isOutputParameter ? ParameterDirection.Output : ParameterDirection.Input };
        }
    }
}
