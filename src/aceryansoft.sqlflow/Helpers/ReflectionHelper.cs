using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace aceryansoft.sqlflow.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// convert list to data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="allowedColumnsMapping"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDataTable<T>(List<T> data, Dictionary<string, string> allowedColumnsMapping, bool preserveMappingOrder = false)
        {
            if (preserveMappingOrder && allowedColumnsMapping == null)
            {
                throw new ArgumentException("can't preserve mapping order when allowedColumnsMapping is null. Bulk insert require "); 
            }

            if (allowedColumnsMapping == null)
            {
                allowedColumnsMapping = GetDefaultColumnsMapping<T>(typeof(T).GetProperties().Select(elt => elt.Name).ToList()); // allow all properties with default order
            }

            var tableResult = new DataTable(); 

            var mappedProperties = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().Where(p=> allowedColumnsMapping.Keys.Contains(p.Name)).ToList();
            var orderedProperties = new List<PropertyDescriptor>();
            foreach (var allowedColumn in allowedColumnsMapping)
            {
                var mappedProp = mappedProperties.FirstOrDefault(x => x.Name == allowedColumn.Key);
                if (mappedProp == null)
                {
                    continue;
                }
                tableResult.Columns.Add(allowedColumn.Value, Nullable.GetUnderlyingType(mappedProp.PropertyType) ?? mappedProp.PropertyType);
                orderedProperties.Add(mappedProp);
            }
            foreach (T item in data)
            {
                DataRow row = tableResult.NewRow();
                foreach (var prop in orderedProperties)
                { 
                   row[allowedColumnsMapping[prop.Name]] = prop.GetValue(item) ?? DBNull.Value;
                }
                tableResult.Rows.Add(row);
            } 
            return tableResult;
        }

        public static Dictionary<string, string> GetDefaultColumnsMapping<T>(List<string> columnsFilter)
        {
            var lowerFilter = columnsFilter.Select(elt => elt.ToLower());

            var allowedColumnsMapping = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().ToDictionary(elt => elt.Name, elt => elt.Name.ToLower());
            return allowedColumnsMapping.Where(elt => lowerFilter.Contains(elt.Value)).ToDictionary(elt => elt.Key, elt => elt.Value);
        }

        /// <summary>
        /// set inner object property values
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propValue"></param>
        /// <param name="propertyPath"></param>
        /// <param name="propertyPathSeparator"></param>
        public static void SetInnerPropertyValue(object target, object propValue, string propertyPath, char propertyPathSeparator = '.')
        {
            if (string.IsNullOrEmpty(propertyPath) || !propertyPath.Contains(propertyPathSeparator))
            {
                throw new ArgumentException($"Can't set inner properties on object {nameof(target)} , path = {propertyPath} without separator {propertyPathSeparator}");
            }

            var paths = propertyPath.ToLower().Split(propertyPathSeparator);
            PropertyInfo innerProperty = null;
            object innerObject = target;
            var innerPropertyList = target.GetType().GetProperties().ToList();
            for (var propIndex = 0; propIndex < paths.Length; propIndex++)
            {
                innerProperty = innerPropertyList.FirstOrDefault(x => x.Name.ToLower() == paths[propIndex]);
                if (innerProperty == null)
                {
                    break;
                }

                if (propIndex == paths.Length - 1)
                {
                    innerProperty.SetValue(innerObject, Convert.ChangeType(propValue, innerProperty.PropertyType));
                    break;
                }            

                innerPropertyList = innerProperty.PropertyType.GetProperties().ToList();
                object innerVal = innerProperty.GetValue(innerObject);
                if (innerVal == null)
                {
                    innerVal = Activator.CreateInstance(innerProperty.PropertyType);
                    innerProperty.SetValue(innerObject, innerVal);
                }
                innerObject = innerVal;
            }
        }
        
    }
}
