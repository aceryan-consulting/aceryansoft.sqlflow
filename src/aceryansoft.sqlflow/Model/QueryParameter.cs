using System;
using System.Collections.Generic;
using System.Text;

namespace aceryansoft.sqlflow.Model
{

    public interface IQueryParameter
    {
        object Value { get; set; }
        bool IsOuputParameter { get; set; }

        object GetDefaultValue();
        Action<object> GetOutputParameterValue { get; set; }
    }

    public class QueryParameter<T> : IQueryParameter
    {
        public object Value { get; set; }
        public bool IsOuputParameter { get; set; }
        //public int Size { get; set; }
        public Action<object> GetOutputParameterValue { get; set; }

        public object GetDefaultValue()
        {
            return default(T);
        }
    }
}
