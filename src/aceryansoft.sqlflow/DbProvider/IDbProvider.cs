using System.Data;
using System.Data.Common;

namespace aceryansoft.sqlflow
{
    /// <summary>
    /// base db provider
    /// </summary>
    public interface IDbProvider
    {
        /// <summary>
        /// create data base connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        DbConnection CreateDbConnexion(string connectionString);
        /// <summary>
        /// create data base parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateDbParameter(string name, object value);

        /// <summary>
        /// Bulk insert multiple data records
        /// </summary>
        /// <param name="dbconnexion"></param>
        /// <param name="targetTable"></param>
        /// <param name="data"></param>
        /// <param name="batchSize"></param>
        void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100);
    }
}
