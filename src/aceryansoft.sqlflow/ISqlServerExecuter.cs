using System.Collections.Generic;

namespace aceryansoft.sqlflow
{
    /// <summary>
    ///  bulk insert is only implemented on Oracle and Sql server
    /// </summary>
    public interface ISqlServerExecuter : IBatchExecuter
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetTable"></param>
        /// <param name="data"></param>
        /// <param name="allowedColumnsMapping">require the same column order defined in database table, ignore table columns with default values</param>
        /// <param name="batchSize"></param>
        void BulkInsert<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping, int batchSize = 100);
    }
}
