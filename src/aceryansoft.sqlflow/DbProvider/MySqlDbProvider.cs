using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System;
using System.IO;


namespace aceryansoft.sqlflow
{
    internal class MySqlDbProvider : IDbProvider
    {
        public void BulkInsert(DbConnection dbconnexion, string targetTable, DataTable data, int batchSize = 100)
        { 
            var tempCsvFile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempCsvFile))
            {
                writer.NewLine = "\r\n";
                Rfc4180Writer.WriteDataTable(data, writer, false); 
            }

            var bulkCopy = new MySqlBulkLoader((MySqlConnection)dbconnexion)
            {
                TableName = targetTable,
                FileName = tempCsvFile,
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                LineTerminator = "\r\n"
                //,NumberOfLinesToSkip = 1
            };
            bulkCopy.Load();
            File.Delete(tempCsvFile);
        }

        public DbConnection CreateDbConnexion(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public DbParameter CreateDbParameter(string name, object value, bool isOutputParameter = false)
        {
            return new MySqlParameter() { ParameterName = name, Value = value, Direction = isOutputParameter ? ParameterDirection.Output : ParameterDirection.Input };
        }
    }
}
