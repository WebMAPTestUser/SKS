using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB.DAO
{
    /// <summary>
    /// Support class for DAO.DBEngine. 
    /// </summary>
    public class DBEngineHelper
    {
        /// <summary>
        /// connectionContainers 
        /// </summary>
        private List<WorkspaceHelper> connectionContainers;
        
        /// <summary>
        /// DbProvider factory
        /// </summary>
        protected DbProviderFactory factory;
        /// <summary>
        /// Singleton Instance
        /// </summary>
        protected static DBEngineHelper instance;
    
        /// <summary>
        /// Creates a new DBEngineHelper object using the provided factory.
        /// </summary>
        /// <param name="factory">The factory to be use this object.</param>
        /// <returns>A new DBEngineHelper.</returns>
        public static DBEngineHelper Instance(DbProviderFactory factory)
        {
            if (instance == null)
            {
                instance = new DBEngineHelper(factory);
            }
            return (DBEngineHelper)instance;
        }

        /// <summary>
        /// Creates a new DBEngineHelper object using the provided factory.
        /// </summary>
        /// <param name="factory">The factory to be use this object.</param>
        protected DBEngineHelper(DbProviderFactory factory)
        {
            this.factory = factory;
            connectionContainers = new List<WorkspaceHelper>();
            WorkspaceHelper container = new WorkspaceHelper();
            container.Factory = factory;
            connectionContainers.Add(container);
        }

        /// <summary>
        /// Gets the WorkspaceHelper object at “index”.
        /// </summary>
        /// <param name="index">The index of the WorkspaceHelper to be returned.</param>
        /// <returns>The WorkspaceHelper at index.</returns>
        public WorkspaceHelper this[int index]
        {
            get
            {
                if (index > connectionContainers.Count - 1)
                {
                    connectionContainers.Add(new WorkspaceHelper(factory));
                }
                return connectionContainers[index];
            }
        }

        /// <summary>
        /// Begins a new transaction. Read/write Database.
        /// </summary>
        public void BeginTransaction()
        {
            connectionContainers[0].BeginTransaction();
        }

        /// <summary>
        /// Ends the current transaction and saves the changes.
        /// </summary>
        public void CommitTransaction()
        {
            connectionContainers[0].CommitTransaction();
        }

        /// <summary>
        /// Ends the current transaction and restores the databases in the Workspace object to the state they were in when the current transaction began.
        /// </summary>
        public void Rollback()
        {
            connectionContainers[0].Rollback();
        }

        /// <summary>
        /// Opens a specified database and returns a reference to the DbConnection object that represents it.
        /// </summary>
        /// <param name="connectionString">The connection strings with the necessary information to connect with the desire Database.</param>
        /// <returns>A DbConnection object that represents the connection with the database.</returns>
        public DAODatabaseHelper OpenDatabase(string connectionString)
        {
            return connectionContainers[0].OpenDatabase(connectionString);
        }

        /// <summary>
        /// Creates a new Workspace object.
        /// </summary>
        /// <param name="name">The name of the new WorkspaceHelper.</param>
        /// <param name="factoryName">The name of the factory to by use by this WorkspaceHelper object (the name most exist on the configuration xml file).</param>
        /// <returns>The new WorkspaceHelper object.</returns>
        public static WorkspaceHelper CreateWorkspace(string name, string factoryName)
        {
            WorkspaceHelper ws = new WorkspaceHelper(name, AdoFactoryManager.GetFactory(factoryName));
            return ws;
        }
        /// <summary>
        /// Creates a new Workspace object.
        /// </summary>
        /// <param name="name">The name of the new WorkspaceHelper.</param>
        /// <param name="factoryName">The name of the factory to by use by this WorkspaceHelper object (the name most exist on the configuration xml file).</param>
        /// <param name="user">The name of the new WorkspaceHelper.</param>
        /// <param name="password">The name of the new WorkspaceHelper.</param>
        /// <returns>The new WorkspaceHelper object.</returns>
        public static WorkspaceHelper CreateWorkspace(string name, string factoryName, string user, string password)
        {
            WorkspaceHelper ws = new WorkspaceHelper(name, AdoFactoryManager.GetFactory(factoryName), user, password);
            return ws;
        }

        /// <summary>
        /// Creates a new Workspace object.
        /// </summary>
        /// <param name="factoryName">The name of the factory to by use by this WorkspaceHelper object (the name most exist on the configuration xml file).</param>
        /// <returns>The new WorkspaceHelper object.</returns>
        public static WorkspaceHelper CreateWorkspace(string factoryName)
        {
            return DBEngineHelper.CreateWorkspace(string.Empty, factoryName);
        }

        /// <summary>
        /// Extracts the command information from the command object and add specific information based on the factory being use.
        /// </summary>
        /// <param name="theCommand">Command to be processed.</param>
        /// <param name="factory">The factory to be use.</param>
        public static void DeriveParameters(DbCommand theCommand, DbProviderFactory factory)
        {
            ParametersHelper.DeriveParameters(theCommand, factory);           
        }

        /// <summary>
        /// 
        /// </summary>
        public string SystemDB { get{ throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
