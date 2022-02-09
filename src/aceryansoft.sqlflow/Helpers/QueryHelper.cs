using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aceryansoft.sqlflow.Helpers
{
    public static class QueryHelper
    {
        public static (string query, Dictionary<string, object> queryParameters) BuildInsertIntoQuery<T>(List<T> data, string tableName
            , Dictionary<string, string> columnsMapping, Dictionary<string, Func<T, object>> objectValueProvider)
        {
            var query = new StringBuilder();
            query.Append(BuildInsertIntoHeader(tableName, columnsMapping));
            var index = 0;
            var queryValues = new List<string>();
            var queryParameters = new Dictionary<string, object>();
            foreach (var row in data)
            {
                queryValues.Add(BuildValueRow(index,columnsMapping));
                foreach (var mapping in columnsMapping)
                { 
                    queryParameters.Add(GetParameterName(index, mapping.Value), objectValueProvider[mapping.Key](row));
                } 
                index++;
            }
            query.Append(string.Join(" , ", queryValues));
            query.Append(" ; ");

            return (query.ToString() , queryParameters);
        }
         
        public static Dictionary<string,Func<T,object>> BuildObjectValueProvider<T>(Dictionary<string, string> columnsMapping)
        {
            var result = new Dictionary<string, Func<T, object>>();
            foreach (var mapping in columnsMapping)
            {
                var prop = typeof(T).GetProperty(mapping.Key);
                if (prop != null)
                {
                    result.Add(mapping.Key, (obj)=> prop.GetValue(obj));
                }
            }
            return result;
        }

        private static string BuildInsertIntoHeader(string tableName, Dictionary<string, string> columnsMapping)
        {
            var fieldNames = string.Join(" , ", columnsMapping.Values.ToList());
            return $"insert into {tableName} ( {fieldNames} ) values ";
        }

        private static string BuildValueRow(int index, Dictionary<string, string> columnsMapping)
        {
            var parameterNames = string.Join(" , ", columnsMapping.Values.Select(x => GetParameterName(index, x)));
            return $" ( {parameterNames} ) ";
        }
        private static string GetParameterName(int index,string propName)
        {
            return $"@{propName}{index}";
        }
    }
}
