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
        /// Run on a shared connection and transaction, the caller is in charge of transaction commit or rollback
        /// </summary>
        /// <param name="sharedTransactionAction"></param>
        /// <param name="isolationLevel"></param>
        void RunOnSharedTransaction(Action<ISqlExecuter, DbTransaction, DbConnection> sharedTransactionAction, System.Data.IsolationLevel isolationLevel);
    }
}
