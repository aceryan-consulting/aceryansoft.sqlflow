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
        ISybaseExecuter WithSybaseExecuter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IPostGreSqlExecuter WithPostGreSqlExecuter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMySqlExecuter WithMySqlExecuter(); 
    }
}
