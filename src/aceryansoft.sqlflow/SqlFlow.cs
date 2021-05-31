using aceryansoft.sqlflow.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace aceryansoft.sqlflow
{
    /// <summary>
    /// base sql flow class
    /// </summary>
    public class SqlFlow : BaseSqlFlow, ISqlFlow , ISqlExecuter, ISqlServerExecuter, ISqlTransactExecuter, IOracleExecuter
    { 
        private SqlFlow(string connectionString, Action<Exception, string, string> onError = null) : base(connectionString, onError)
        { 
        }

        public static ISqlFlow Create(string connectionString, Action<Exception, string, string> onError = null)
        {
            return new SqlFlow(connectionString, onError);
        }

        #region ISqlFlow implementation 
        public ISqlServerExecuter WithSqlServerExecuter()
        {
            _dataBaseProvider = new SqlServerDbProvider();
            return this;
        }
        public IOracleExecuter WithOracleExecuter()
        {
            _dataBaseProvider = new OracleDbProvider();
            return this;
        }
        public ISqlTransactExecuter WithSybaseExecuter()
        {
            _dataBaseProvider = new SybaseDbProvider();
            return this;
        }

        public ISqlTransactExecuter WithPostGreSqlExecuter()
        {
            _dataBaseProvider = new PostGreSqlDbProvider();
            return this;
        }
        //
        public ISqlTransactExecuter WithMySqlExecuter()
        {
            _dataBaseProvider = new MySqlDbProvider();
            return this;
        }

        public void BulkInsert<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping = null, int batchSize = 100)
        {
            using (var connexion = _dataBaseProvider.CreateDbConnexion(_connectionString))
            {
                connexion.Open();
                var dt = ReflectionHelper.ConvertListToDataTable<T>(data, allowedColumnsMapping);
                _dataBaseProvider.BulkInsert(connexion, targetTable, dt , batchSize);
            }  
        }
        #endregion

        #region ISqlExecuter implementation
         
        public void ExecuteNonQuery(string query, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            ExecuteOnDbCommand(
               (command) =>
               {
                   command.ExecuteNonQuery();
               }, query, queryParameters, isStoreProcedure);
        } 

        public void ExecuteReader(string query, Action<DbDataReader> actionOnEachRow, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            ExecuteOnDbCommand(
            (command) =>
            {
                using (var reader = command.ExecuteReader())
                { 
                    while (reader.Read())
                    {                       
                        actionOnEachRow(reader);
                    }
                }
            }, query, queryParameters, isStoreProcedure);
        }

        public void ExecuteReaderOnMultipleResultsSet(string query, Action<DbDataReader, int> actionOnEachReaderAndRows, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            ExecuteOnDbCommand(
            (command) =>
            {
                using (var reader = command.ExecuteReader())
                {
                    int readerIndex = 0;
                    do
                    {
                        while (reader.Read())
                        {
                            actionOnEachReaderAndRows(reader, readerIndex);
                        }
                        readerIndex++;
                    } while(reader.NextResult());                    
                }
            }, query, queryParameters, isStoreProcedure);
        }

        public List<T> ExecuteReaderAndMap<T>(string query, Dictionary<string, string> propertiesMapping=null, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            var result = new List<T>();
            ExecuteOnDbCommand(
           (command) =>
           {
               using (var reader = command.ExecuteReader())
               {
                   var mapper = GetResultMapper<T>(reader, propertiesMapping);
                   while (reader.Read())
                   {
                       var dataRow = (T)Activator.CreateInstance(typeof(T));
                       mapper(dataRow, reader);
                       result.Add(dataRow);
                   }
               }
           }, query, queryParameters, isStoreProcedure);

           return result;
        }
    
        public T ExecuteScalar<T>(string query, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            T result = default(T);
            ExecuteOnDbCommand(
                (command) =>
                {
                    result = (T)command.ExecuteScalar();
                }, query, queryParameters, isStoreProcedure);
            return result;
        }
        #endregion

        #region
        public void RunTransaction(Action<ISqlExecuter, DbConnection, DbTransaction> transactionAction)
        {
            RunTransactionInternal((db,tran)=> transactionAction(this,db, tran));
        }

        #endregion
    }
}
