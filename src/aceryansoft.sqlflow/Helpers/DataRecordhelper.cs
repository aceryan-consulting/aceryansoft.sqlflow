using System;
using System.Collections.Generic;
using System.Text;

namespace aceryansoft.sqlflow.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataRecordhelper
    {
        /// <summary>
        /// generic get data record value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this System.Data.IDataRecord record, string columnName, T defaultValue = default(T)) where T : IConvertible
        {
            var result = record.GetValue(record.GetOrdinal(columnName));
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
