using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB
{
    /// <summary>
    /// This is the base class to administrate multiple connections under the same structure with the possibility to use a transactional model for all the connections.
    /// </summary>
    public class ConnectionContainers
    {
        private List<DbConnection> connections;
        private DbProviderFactory factory;

        /// <summary>
        /// Creates a new ConnectionContainers object.
        /// </summary>
        protected ConnectionContainers()
        {
            connections = new List<DbConnection>();
        }

        /// <summary>
        /// Creates a new ConnectionContainers object and set the DBProviderFactory to “factory”.
        /// </summary>
        /// <param name="factory">The factory to be used by the connections created with this ConnectionContainers object.</param>
        protected ConnectionContainers(DbProviderFactory factory):this()
        {
            this.factory = factory;
        }

        /// <summary>
        /// Sets the DBProviderFactory to be use in the connections created with this ConnectionContainers object.
        /// </summary>
        internal DbProviderFactory Factory
        {
            set { factory = value; }
        }

        /// <summary>
        /// Gets the list of all connections contained in this object.
        /// </summary>
        public List<DbConnection> Connections
        {
            get { return connections; }
        }

        /// <summary>
        /// Begins a transaction for a specific connection.
        /// </summary>
        /// <param name="connection">The connection where the transaction will be initiated.</param>
        private void BeginTransaction(DbConnection connection)
        {
            TransactionManager.Enlist(connection);
        }

        /// <summary>
        /// Begins a transaction for all connections contained in this object.
        /// </summary>
        public void BeginTransaction()
        {
            connections.ForEach(BeginTransaction);
        }

        /// <summary>
        /// Closes a transaction for a specific connection.
        /// </summary>
        /// <param name="connection">The connection where the transaction will be close.</param>
        private void Close(DbConnection connection)
        {
            TransactionManager.DeEnlist(connection);
            connection.Close();
        }

        /// <summary>
        /// Closes a transaction for all connections contained in this object.
        /// </summary>
        public void Close()
        {
            connections.ForEach(Close);
        }

        /// <summary>
        /// Commits a transaction for a specific connection.
        /// </summary>
        /// <param name="connection">The connection where the transaction will be committed.</param>
        private void CommitTransaction(DbConnection connection)
        {
            TransactionManager.Commit(connection);
        }

        /// <summary>
        /// Commits a transaction for all connections contained in this object.
        /// </summary>
        public void CommitTransaction()
        {
            connections.ForEach(CommitTransaction);
        }
        
        /// <summary>
        /// Rollbacks a transaction for a specific connection.
        /// </summary>
        /// <param name="connection">The connection to work on.</param>
        private void Rollback(DbConnection connection)
        {
            TransactionManager.Rollback(connection);
        }

        /// <summary>
        /// Rollbacks a transaction for all connections contained in this object.
        /// </summary>
        public void Rollback()
        {
            connections.ForEach(Rollback);
        }


        /// <summary>
        /// Creates a new connection and opens it using the provided connection string.
        /// </summary>
        /// <param name="connectionString">The connection string with the information to connect to a database.</param>
        /// <returns>The newly created DBConnection object.</returns>
        protected DbConnection Open(String connectionString)
        {
            DbConnection result = factory.CreateConnection();
            result.ConnectionString = connectionString;
            result.Open();
            connections.Add(result);
            result.StateChange += new System.Data.StateChangeEventHandler(result_StateChange);
            return result;
        }

        /// <summary>
        /// Event that notifies the current state of a change.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">Additional event information.</param>
        void result_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState ==  System.Data.ConnectionState.Closed)
            {
                connections.Remove((DbConnection)sender);
            }
        }
    }
}
