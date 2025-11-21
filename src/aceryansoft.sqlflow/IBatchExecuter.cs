using System.Collections.Generic;

namespace aceryansoft.sqlflow
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBatchExecuter : ISqlTransactExecuter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetTable"></param>
        /// <param name="data"></param>
        /// <param name="allowedColumnsMapping">Map and insert all Fields if null</param>
        /// <param name="batchSize"></param>
        void BatchInsertRows<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping = null, int batchSize = 100);
    }
}
