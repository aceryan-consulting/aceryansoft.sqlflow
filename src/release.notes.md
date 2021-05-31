
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
 
 
 

 
