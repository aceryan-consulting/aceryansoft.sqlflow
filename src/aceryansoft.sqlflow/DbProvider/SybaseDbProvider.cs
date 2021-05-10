using AdoNetCore.AseClient;
using System.Data;
using System.Data.Common;

namespace aceryansoft.sqlflow
{
    internal class SybaseDbProvider : IDbProvider
    {
        public void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100)
        {
            throw new System.NotImplementedException();
        }

        public DbConnection CreateDbConnexion(string connectionString)
        {
            return new AseConnection(connectionString);
        }

        public DbParameter CreateDbParameter(string name, object value)
        {
            return new AseParameter() { ParameterName = name, Value = value };
        }

    }
}
