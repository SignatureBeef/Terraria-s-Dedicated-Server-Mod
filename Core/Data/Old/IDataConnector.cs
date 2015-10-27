using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSM.Core.Data.Permissions;

namespace TDSM.Core.Data.Old
{
    /// <summary>
    /// Bare implementation for the required needs of a Data Connector
    /// </summary>
    public interface IDataConnector : IPermissionHandler
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <returns>The builder.</returns>
        /// <param name="pluginName">Plugin name.</param>
        QueryBuilder GetBuilder(string pluginName);

        //        QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type);

        /// <summary>
        /// Opens the connection to the data store
        /// </summary>
        void Open();

        /// <summary>
        /// Execute the specified builder.
        /// </summary>
        /// <param name="builder">Builder.</param>
        bool Execute(QueryBuilder builder);

        /// <summary>
        /// Executes the builder and returns the insert id.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="builder">Builder.</param>
        long ExecuteInsert(QueryBuilder builder);

        /// <summary>
        /// Executes the non query as specified in the builder
        /// </summary>
        /// <returns>The non query.</returns>
        /// <param name="builder">Builder.</param>
        int ExecuteNonQuery(QueryBuilder builder);

        /// <summary>
        /// Executes the a scalar query via the builder
        /// </summary>
        /// <returns>The scalar.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T ExecuteScalar<T>(QueryBuilder builder);

        /// <summary>
        /// Executes the builder and returns the data set.
        /// </summary>
        /// <returns>The data set.</returns>
        /// <param name="builder">Builder.</param>
        DataSet ExecuteDataSet(QueryBuilder builder);

        /// <summary>
        /// Executes the buidler and returns an array of reflected rows
        /// </summary>
        /// <returns>The array.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T[] ExecuteArray<T>(QueryBuilder builder) where T : new();
    }
}
