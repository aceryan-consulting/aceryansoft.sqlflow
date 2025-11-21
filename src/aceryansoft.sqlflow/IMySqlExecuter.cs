using System.Collections.Generic;

namespace aceryansoft.sqlflow
{
    /// <summary>
    /// use bulk insert instead of batch insert, insert into syntax is only enable for integration tests with portable db
    /// </summary>
    public interface IMySqlExecuter : IBatchExecuter
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
