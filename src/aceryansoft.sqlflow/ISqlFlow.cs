namespace aceryansoft.sqlflow
{  
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlFlow
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISqlServerExecuter WithSqlServerExecuter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IOracleExecuter WithOracleExecuter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISqlTransactExecuter WithSybaseExecuter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISqlTransactExecuter WithPostGreSqlExecuter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISqlTransactExecuter WithMySqlExecuter(); 
    }
}
