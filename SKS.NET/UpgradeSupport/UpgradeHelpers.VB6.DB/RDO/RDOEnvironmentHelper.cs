using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB.RDO
{
    /// <summary>
    /// This class represents the rdoEnvironment semantic meaning holding a collection of connections.
    /// </summary>
    public class RDOEnvironmentHelper : ConnectionContainers
    {
        /// <summary>
        /// Timeout value.
        /// </summary>
        private int loginTimeOut = 15;

        /// <summary>
        /// Name value for the environment.
        /// </summary>
        private string name;

        /// <summary>
        /// Creates a new environment.
        /// </summary>
        public RDOEnvironmentHelper() { }

        /// <summary>
        /// Creates a new environment.
        /// </summary>
        /// <param name="factory">Represents the provider factory to be used to create the ADO .Net.</param>
        public RDOEnvironmentHelper(DbProviderFactory factory)
            : base(factory)
        {
        }

        /// <summary>
        /// Gets and sets the name of the environment.
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets and sets the login time out.
        /// </summary>
        public int LoginTimeOut
        {
            get { return loginTimeOut; }
            set { loginTimeOut = value; }
        }

        /// <summary>
        /// Opens a new database connection.
        /// </summary>
        /// <param name="connectionString">This is the connection to be used to connect to the database.</param>
        /// <returns>A new open connection.</returns>
        public DbConnection OpenConnection(String connectionString)
        {
            DbConnection result = base.Open(connectionString);
            return result;
        }

     }
}
