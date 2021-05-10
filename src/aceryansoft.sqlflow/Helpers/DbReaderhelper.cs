using System;
using System.Data.Common;

namespace aceryansoft.sqlflow.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbReaderhelper
    { 
        /// <summary>
        /// generic get data reader value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DbDataReader reader, string columnName, T defaultValue = default(T)) where T : IConvertible
        {
            var result = reader.GetValue(reader.GetOrdinal(columnName));
            if (result == null || result == DBNull.Value)
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception e)
            {
                return defaultValue;
            }

        }
    }
}
