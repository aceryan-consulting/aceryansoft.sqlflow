using aceryansoft.sqlflow.Helpers;
using aceryansoft.sqlflow.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace aceryansoft.sqlflow
{
    public class BaseSqlFlow  
    {
        protected readonly string _connectionString;
        protected readonly Action<Exception, string, string> _onError;
        protected DbConnection _currentDbConnexion;
        protected DbTransaction _currentTransaction;
        protected IDbProvider _dataBaseProvider;
        protected bool _useTransaction;
        protected string _lastQuery = "";
        protected string _lastParameters="";
        protected delegate void MapReaderToObject<T>(T target, DbDataReader reader);

        internal BaseSqlFlow(string connectionString, Action<Exception,string,string> onError=null)
        {
            _connectionString = connectionString;
            _onError = onError;
        }

        protected DbCommand CreateCommand(string query, DbConnection connexion
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            var command = connexion.CreateCommand();
            command.CommandText = query;
            _lastQuery = query;
            _lastParameters = queryParameters == null ? "no parameters" : string.Join(" ", queryParameters.Select(elt => $"{elt.Key}:{elt.Value ?? "null"}"));
            if (_useTransaction)
            {
                command.Transaction = _currentTransaction;
            }

            if (isStoreProcedure)
            {
                command.CommandType = CommandType.StoredProcedure;
            }

            if (queryParameters != null && queryParameters.Any())
            {
                foreach (var parameter in queryParameters)
                {
                    if (parameter.Value is IQueryParameter queryParam)
                    {
                        var queryDbParam = _dataBaseProvider.CreateDbParameter(parameter.Key, queryParam.Value ?? queryParam.GetDefaultValue(), queryParam.IsOuputParameter);
                        command.Parameters.Add(queryDbParam);
                    }
                    else
                    {
                        command.Parameters.Add(_dataBaseProvider.CreateDbParameter(parameter.Key, parameter.Value ??  DBNull.Value));
                    }
                }
            }
            return command;
        }

        protected void ExecuteOnDbCommand( Action<DbCommand> actionOnDbCommand, string query
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false)
        {
            try
            {
                if (!_useTransaction)
                {
                    using (var connexion = _dataBaseProvider.CreateDbConnexion(_connectionString))
                    {
                        connexion.Open();
                        using (var command = CreateCommand(query, connexion, queryParameters, isStoreProcedure))
                        {
                            actionOnDbCommand(command);
                            GetOutputParametersValuesIfRequired(command, queryParameters);
                        }
                    }
                }
                else
                {
                    using (var command = CreateCommand(query, _currentDbConnexion, queryParameters, isStoreProcedure))
                    {
                        actionOnDbCommand(command);
                        GetOutputParametersValuesIfRequired(command, queryParameters);
                    }
                }
            }
            catch(Exception ex)
            {
                _onError?.Invoke(ex, _lastQuery, _lastParameters);
                throw; //rethrow the exception to be caught by the caller
            }           
        }

        private void GetOutputParametersValuesIfRequired(DbCommand command, Dictionary<string, object> queryParameters)
        {
            if (queryParameters == null || !queryParameters.Any())
            {
                return;
            }

            foreach (var parameter in queryParameters)
            {
                if (parameter.Value is IQueryParameter queryParam)
                {
                    queryParam.GetOutputParameterValue?.Invoke(  command.Parameters[parameter.Key].Value  );
                }
            }
        }
        
        protected void RunTransactionInternal(Action<DbConnection, DbTransaction> transactionAction)
        {
            _useTransaction = true;
            using (var _currentDbConnexion = _dataBaseProvider.CreateDbConnexion(_connectionString))
            {
                _currentDbConnexion.Open(); 
                using (var _currentTransaction = _currentDbConnexion.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                { 
                    try
                    {
                        transactionAction(_currentDbConnexion, _currentTransaction);
                    }
                    catch(TransactionException trex)
                    {
                        RollbackAndLogTransactionException(trex);
                        throw; // Rollback and rethrow the exception to be caught by the caller
                    }
                    catch (Exception ex)
                    {
                        RollbackAndLogTransactionException(ex);
                        throw; // Rollback and rethrow the exception to be caught by the caller
                    }
                    _currentTransaction.Commit();
                }
            }
            ResetTransaction();
        }

        private void RollbackAndLogTransactionException(Exception ex)
        {
            _currentTransaction.Rollback();
            ResetTransaction();
            _onError?.Invoke(ex, _lastQuery, _lastParameters);
        }

        protected void ResetTransaction()
        {
            _useTransaction = false;
            _currentDbConnexion = null;
        }

        protected MapReaderToObject<T> GetResultMapper<T>(DbDataReader reader, Dictionary<string, string> propertiesMapping = null) 
        {
            propertiesMapping = propertiesMapping ?? new Dictionary<string, string>();
            propertiesMapping = propertiesMapping.ToDictionary(x => x.Key.ToLower(), x => x.Value.ToLower());
            MapReaderToObject<T> mapper = (tar, rea) => { };            

            var typeProperties = typeof(T).GetProperties().ToList();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i).ToLower();
                var matchingProperty = typeProperties.FirstOrDefault(x => x.Name.ToLower() == columnName);                 
                var currentIndex = i;
                if (matchingProperty != null) 
                {
                    mapper += (tar, rea) =>
                    {
                        if (!rea.IsDBNull(currentIndex))
                        {
                            matchingProperty.SetValue(tar, Convert.ChangeType(rea.GetValue(currentIndex), matchingProperty.PropertyType));
                        }
                    };
                    continue;
                }

                if (propertiesMapping.ContainsKey(columnName))
                {
                    var matchColumnName = propertiesMapping[columnName];
                    var overrideProperty = typeProperties.FirstOrDefault(x => x.Name.ToLower() == matchColumnName);
                    if (overrideProperty != null)
                    {
                        mapper += (tar, rea) =>
                        {
                            if (!rea.IsDBNull(currentIndex))
                            {
                                overrideProperty.SetValue(tar, Convert.ChangeType(rea.GetValue(currentIndex), overrideProperty.PropertyType));
                            }
                        };
                    }
                    else if (overrideProperty == null && matchColumnName.Contains('.'))
                    {
                        mapper += (tar, rea) =>
                        {
                            if (!rea.IsDBNull(currentIndex))
                            {
                                ReflectionHelper.SetInnerPropertyValue(tar, rea.GetValue(currentIndex), matchColumnName); 
                            }
                        };
                    }
                }
            }

            if (mapper.GetInvocationList().Length < 2)
            {
                throw new ArgumentException("can't map any object properties to the data reader");
            }

            return mapper;
        }
            
    }
}
