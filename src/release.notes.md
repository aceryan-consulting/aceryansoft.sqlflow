
## *** Release note : version 1.22.12.07 ***
###  What's new 

- Add nuget package for netstandard2.1 
- implement bulkinsert method on mysql db provider using bulk load with temp file. 
- Update nuget packages (security and bug fix)
     - System.Data.SqlClient.4.8.3 -> System.Data.SqlClient.4.8.5
     - netstandard2.0
         - Oracle.ManagedDataAccess.Core.2.19.140 -> Oracle.ManagedDataAccess.Core.2.19.170
     - netstandard2.1 
         - Oracle.ManagedDataAccess.Core.2.19.140 -> Oracle.ManagedDataAccess.Core.3.21.80
     - Npgsql.6.0.3 -> Npgsql.7.0.0
     - MySql.Data.8.0.28 -> MySql.Data.8.0.31

     
## *** Release note : version 1.22.02.09 ***
###  What's new 

- Add batchinsert method on sqlserver, mysql and postgresql to insert multiple rows. For sql server it is recommend to use bulkInsert
``` c#
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="targetTable"></param>
/// <param name="data"></param>
/// <param name="allowedColumnsMapping">Map and insert all Fields if null</param>
/// <param name="batchSize"></param>
void BatchInsertRows<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping = null, int batchSize = 100);
```
###  Breaking changes 
- Change Executer interfaces for sybase, postgresql, mysql   
    - Before 
        ``` c#
         interface ISqlFlow     
            ISqlServerExecuter WithSqlServerExecuter(); 
            IOracleExecuter WithOracleExecuter(); 

            ISqlTransactExecuter WithSybaseExecuter(); 
            ISqlTransactExecuter WithPostGreSqlExecuter(); 
            ISqlTransactExecuter WithMySqlExecuter();  
        ```   
    - After 
        ``` c#
         interface ISqlFlow     
            ISqlServerExecuter WithSqlServerExecuter(); 
            IOracleExecuter WithOracleExecuter(); 

            ISybaseExecuter WithSybaseExecuter(); 
            IPostGreSqlExecuter WithPostGreSqlExecuter(); 
            IMySqlExecuter WithMySqlExecuter();  
        ```   
- Update nuget packages (no changes expected)
     - System.Data.SqlClient.4.8.2 -> System.Data.SqlClient.4.8.3
     - Oracle.ManagedDataAccess.Core.2.19.110 -> Oracle.ManagedDataAccess.Core.2.19.140
         - Bug Fixes since Oracle.ManagedDataAccess.Core NuGet Package 2.19.131 (Bug 33576541 - bulk copy cannot insert more than 255 columns)
     - Castle.Core.4.4.0 -> Castle.Core.4.4.1
     - NSubstitute.4.2.2 -> NSubstitute.4.3.0
     - Npgsql.6.0.3 -> Npgsql.5.0.12
     - MySql.Data.8.0.24 -> MySql.Data.8.0.28


## *** Release note : version 1.21.06.01 ***
###  What's new 

- Manage output parameters on queries and store procedures.
``` c#
 {"@customerId", new QueryParameter<int>()  
	{
		IsOuputParameter = true,
		GetOutputParameterValue = (val)=>{newCustomerId = (int) val; }
	}
 }
```
- Manage multiple resultsets   
    - void ExecuteReaderOnMultipleResultsSet(string query, Action<DbDataReader, int> actionOnEachReaderAndRows, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);    
  

## *** Release note : version 1.21.05.11 ***
###  What's new 
c# light and fluent wrapper for easily interacting with database 
- Query support for databases (Sql server,Oracle,PostgreSql,Mysql,Sybase).
- ISqlFlow interface with common opérations   
    - void ExecuteNonQuery(string query, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);          
    - T ExecuteScalar<T>(string query, Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);        
    - void ExecuteReader(string query, Action<DbDataReader> actionOnEachRow
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false); 
    - List<T> ExecuteReaderAndMap<T>(string query, Dictionary<string, string> propertiesMapping=null
            , Dictionary<string, object> queryParameters = null, bool isStoreProcedure = false);
    - void RunTransaction(Action<ISqlExecuter, DbConnection, DbTransaction> transactionAction);
   
- BulkInsert on Sql server and Oracle   
  - void BulkInsert<T>(string targetTable, List<T> data, Dictionary<string, string> allowedColumnsMapping = null, int batchSize = 100);
 
 
 

 
