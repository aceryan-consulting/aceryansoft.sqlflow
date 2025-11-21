using System;
using System.Data.Common;

namespace aceryansoft.sqlflow
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlTransactExecuter : ISqlExecuter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionAction"></param>
        void RunTransaction(Action<ISqlExecuter, DbConnection, DbTransaction> transactionAction);


        /// <summary>
        /// Run multiple db actions on the shared connection, this should be more usefull for complex transaction scenario
        /// </summary>
        /// <param name="sharedConnectionAction"> the callback to create and manage multiple action on shared connection</param>
        void RunOnSharedConnection(Action<ISqlExecuter, DbConnection> sharedConnectionAction);
    }
}
