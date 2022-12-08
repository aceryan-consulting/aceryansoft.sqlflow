using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace aceryansoft.sqlflow.Helpers
{
    // Author Thomas Corey , licence CPOL , source https://www.codeproject.com/Tips/665519/Writing-a-DataTable-to-a-CSV-file 
    //todo find a nuget package instead of source code  
    public static class Rfc4180Writer
    {
        public static void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders)
        {
            if (includeHeaders)
            {
                IEnumerable<String> headerValues = sourceTable.Columns
                    .OfType<DataColumn>()
                    .Select(column => QuoteValue(column.ColumnName));

                writer.WriteLine(String.Join(",", headerValues));
            }

            IEnumerable<String> items = null;

            foreach (DataRow row in sourceTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o?.ToString() ?? String.Empty));
                writer.WriteLine(String.Join(",", items));
            }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
                value.Replace("\"", "\"\""), "\"");
        }
    }
}