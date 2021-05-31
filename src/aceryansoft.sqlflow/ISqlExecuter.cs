using System;
using System.Collections.Generic;
using System.Data.Common;

namespace aceryansoft.sqlflow
{ 
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlExecuter
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryParameters"></param>
        /// <param name="isStoreProcedure"></param>
        void ExecuteNonQuery(string query, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);          
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="queryParameters"></param>
        /// <param name="isStoreProcedure"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string query, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="actionOnEachRow"></param>
        /// <param name="queryParameters"></param>
        /// <param name="isStoreProcedure"></param>
        void ExecuteReader(string query, Action<DbDataReader> actionOnEachRow
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false); 
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertiesMapping"></param>
        /// <param name="queryParameters"></param>
        /// <param name="isStoreProcedure"></param>
        /// <returns></returns>
        List<T> ExecuteReaderAndMap<T>(string query, Dictionary<string, string> propertiesMapping=null
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="actionOnEachRow"></param>
        /// <param name="queryParameters"></param>
        /// <param name="isStoreProcedure"></param>
        void ExecuteReaderOnMultipleResultsSet(string query, Action<DbDataReader, int> actionOnEachReaderAndRows
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);
    }
     
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlTransactExecuter : ISqlExecuter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionAction"></param>
        void RunTransaction(Action<ISqlExecuter, DbConnection, DbTransaction> transactionAction);
    }
    

    /// <summary>
    ///  bulk insert is only implemented on Oracle and Sql server
    /// </summary>
    public interface ISqlServerExecuter : ISqlTransactExecuter
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetTable"></param>
        /// <param name="data"></param>
        /// <param name="allowedColumnsMapping"></param>
        /// <param name="batchSize"></param>
        void BulkInsert<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping = null, int batchSize = 100);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IOracleExecuter : ISqlTransactExecuter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetTable"></param>
        /// <param name="data"></param>
        /// <param name="allowedColumnsMapping"></param>
        /// <param name="batchSize"></param>
        void BulkInsert<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping = null, int batchSize = 100);
    }
}
