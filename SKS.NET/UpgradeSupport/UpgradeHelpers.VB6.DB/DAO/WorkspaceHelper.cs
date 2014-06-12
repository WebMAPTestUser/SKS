using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using UpgradeHelpers.VB6.DB.ADO;


namespace UpgradeHelpers.VB6.DB.DAO
{
    /// <summary>
	/// User Class
	/// </summary>
	public class User
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public User(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            throw new NotImplementedException();
        }

	    private Groups _groups;

	    ///
        /// <summary>
        /// 
        /// </summary>
        public Groups Groups
	    {
	        get { return _groups; }
	        set { _groups = value; }
	    }

	    private string _name;

	    /// <summary>
        /// 
        /// </summary>
        public string Name
	    {
	        get { return _name; }
	        set { _name = value; }
	    }
    }

	/// <summary>
	/// Group Class
	/// </summary>
    public class Group
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Group(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 
        /// </summary>
        public Users Users
        {
            get { return null; }
        }

	    private string _name;

	    /// <summary>
        /// 
        /// </summary>
        public string Name
	    {
	        get { return _name; }
	        set { _name = value; }
	    }
    }

    /// <summary>
	/// Groups Class
    /// </summary>
    public class Groups : Dictionary<string, Group>, IEnumerable<Group>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<Group> GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
			if (this != null)
			{
				this.Clear();
			}
        }
    }

    /// <summary>
	/// Users Class
    /// </summary>
    public class Users : Dictionary<string, User>, IEnumerable<User>
    {
        /// <summary>
        /// 
        /// </summary>
        public Users()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<User> GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
			if (this != null)
			{
				this.Clear();
			}
        }
        
    }

    /// <summary>
	/// WorkspaceHelper Class
    /// A Workspace object defines a named session for a user. It contains open databases and provides mechanisms for simultaneous transactions.
    /// </summary>
    public class WorkspaceHelper
    {
        private List<DAODatabaseHelper> connections = new List<DAODatabaseHelper>();
        private DbProviderFactory factory;
        private string user;
        private string password;

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
        public List<DAODatabaseHelper> Connections
        {
            get { return connections; }
        }

        /// <summary>
        /// Begins a transaction for a specific connection.
        /// </summary>
        /// <param name="connection">The connection where the transaction will be initiated.</param>
        private void BeginTransaction(DAODatabaseHelper connection)
        {
            TransactionManager.Enlist(connection.Connection);
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
        private void Close(DAODatabaseHelper connection)
        {
            TransactionManager.DeEnlist(connection.Connection);
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
        private void CommitTransaction(DAODatabaseHelper connection)
        {
            TransactionManager.Commit(connection.Connection);
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
        private void Rollback(DAODatabaseHelper connection)
        {
            TransactionManager.Rollback(connection.Connection);
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
        protected DAODatabaseHelper Open(String connectionString)
        {
            
            DbConnection cn = factory.CreateConnection();
            //cn.ConnectionString = connectionString + Credentials; //SQL
			cn.ConnectionString = connectionString; //Access
            cn.Open();
            cn.StateChange += new System.Data.StateChangeEventHandler(result_StateChange);
            DAODatabaseHelper daodb = new DAODatabaseHelper(cn);
            connections.Add(daodb);
            return daodb;
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
                foreach (DAODatabaseHelper db in connections)
                {
                    if (db.Connection == sender)
                    {
                        connections.Remove(db);
                        break;
                    }
                }
            }
        }

        private string name;

        #region Constructors
        
        /// <summary>
        /// Creates a new WorkspaceHelper object with the default configuration.
        /// </summary>
        public WorkspaceHelper()
        {
            this.name = "default";
        }

        /// <summary>
        /// Creates a new WorkspaceHelper object with the default configuration and the provided name.
        /// </summary>
        /// <param name="name">The name for the new WorkspaceHelper</param>
        public WorkspaceHelper(string name)
        {
            this.name = name; 
        }

        /// <summary>
        /// Creates a new WorkspaceHelper object using the provided factory.
        /// </summary>
        /// <param name="factory">The factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
        public WorkspaceHelper(DbProviderFactory factory)
        {
        	this.factory = factory;
            this.name = "default";
        }

        /// <summary>
        /// Creates a new WorkspaceHelper object using the provided name and factory.
        /// </summary>
        /// <param name="name">The name for the new WorkspaceHelpe.r</param>
        /// <param name="factory">The factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
        public WorkspaceHelper(string name, DbProviderFactory factory)
        {
            this.factory = factory;
            this.name = name;
        }
        /// <summary>
        /// Creates a new WorkspaceHelper object using the provided name and factory.
        /// </summary>
        /// <param name="name">The name for the new WorkspaceHelpe.r</param>
        /// <param name="factory">The factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
        /// <param name="user">The name for the new WorkspaceHelpe.r</param>
        /// <param name="password">The name for the new WorkspaceHelpe.r</param>
        public WorkspaceHelper(string name, DbProviderFactory factory, string user, string password)
        {
            this.factory = factory;
            this.name = name;
            this.user = user;
            this.password = password;
			using (DbConnection cn = factory.CreateConnection())
			{
				cn.ConnectionString = ConnectionString;
				cn.Open();
			}
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the name that uniquely identifies this WorkspaceHelper.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        #endregion

        /// <summary>
        /// Opens a Database using the provided connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>A DAODatabaseHelper object with the representation of the openned database.</returns>
        public DAODatabaseHelper OpenDatabase(String connectionString)
        {
            return Open(connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        private Users _Users = null;

        /// <summary>
        /// 
        /// </summary>
        public Users Users
        {
            get
            {
                if (_Users == null || _Users.Count == 0)
                {
                    _Users = new Users();
                    using (DbConnection cn = factory.CreateConnection())
                    {
                        cn.ConnectionString = ConnectionString;
                        cn.Open();

                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = cn;
                            cmd.CommandText = "select * from dbo.sysusers where islogin = 1";
                            using (DbDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string user = Convert.ToString(reader["name"]);
                                    _Users.Add(user, new User(user));
                                }
                            }
                        }

                        foreach (User user in _Users)
                        {
                            Groups groups = new Groups();
                            using (DbCommand cmd = factory.CreateCommand())
                            {
                                StringBuilder query = new StringBuilder("Select dp2.name ");
                                query.Append("from sys.database_principals dp2 ");
                                query.Append("where dp2.principal_id in ");
                                query.Append("( Select	drm.role_principal_id ");
                                query.Append("From	sys.database_principals dp, ");
                                query.Append("sys.database_role_members drm ");
                                query.Append("  Where ");
                                query.Append("dp.principal_id = drm.member_principal_id and ");
                                query.Append("dp.name = '{0}' ");
                                query.Append(") ");
                                query.Append("and dp2.type = 'R' and dp2.is_fixed_role = 0");
                                cmd.Connection = cn;
                                cmd.CommandText = String.Format(query.ToString(), user.Name);
                                using (DbDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string name = Convert.ToString(reader["name"]);
                                        groups.Add(name, new Group(name));
                                    }
                                }
                            }
                            user.Groups = groups;
                        }

                    }
                }
                return _Users;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        private Groups _Groups = null;

        ///
        /// <summary>
        /// 
        /// </summary>
        public Groups Groups
        {
            get 
            {
                if (_Groups == null || _Groups.Count == 0)
                {
                    _Groups = new Groups();
                    using (DbConnection cn = factory.CreateConnection())
                    {
						cn.ConnectionString = ConnectionString;
                        cn.Open();

                        using (DbCommand cmd = factory.CreateCommand())
                        {
                            cmd.Connection = cn;
                            cmd.CommandText = "select * from sys.database_principals where type = 'R' and is_fixed_role = 0";
                            using (DbDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string name = Convert.ToString(reader["name"]);
                                    _Groups.Add(name, new Group(name));
                                }
                            }
                        }
                    }
                }
                return _Groups;
            }
            set {  }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseHelper"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="UserRole"></param>
        public void CreateUser(DAODatabaseHelper databaseHelper, string Username, string Password, string UserRole)
        {
            this.BeginTransaction();
            System.Data.Common.DbCommand TempCommand_2 = databaseHelper.CreateCommand();
            TempCommand_2.CommandText = "sp_addnewuser";
            TempCommand_2.CommandType = System.Data.CommandType.StoredProcedure;
            TempCommand_2.Transaction = TransactionManager.GetTransaction(databaseHelper.Connection);
            DbCommand command = TempCommand_2;
            ADORecordSetHelper.commandParameterBinding(command, "Username").Value = Username;
            ADORecordSetHelper.commandParameterBinding(command, "Password").Value = Password;
            ADORecordSetHelper.commandParameterBinding(command, "UserRole").Value = UserRole;
            command.ExecuteNonQuery();
            this.CommitTransaction();
        }
/// <summary>
        /// 
        /// </summary>
        /// <param name="databaseHelper"></param>
        /// <param name="Username"></param>
        public void DeleteUser(DAODatabaseHelper databaseHelper, string Username)
        {
            System.Data.Common.DbCommand TempCommand_2 = databaseHelper.CreateCommand();
            TempCommand_2.CommandText = String.Format("DROP USER {0}", Username);
            TempCommand_2.CommandType = System.Data.CommandType.Text;
            TempCommand_2.Transaction = TransactionManager.GetTransaction(databaseHelper.Connection);
            DbCommand command = TempCommand_2;
            command.ExecuteNonQuery();
            TempCommand_2 = databaseHelper.CreateCommand();
            TempCommand_2.CommandText = String.Format("DROP Login {0}", Username);
            command = TempCommand_2;
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseHelper"></param>
        /// <param name="OldGroup"></param>
        /// <param name="NewGroup"></param>
        /// <param name="user"></param>
        public void UpdateRole(DAODatabaseHelper databaseHelper, string OldGroup, string NewGroup, string user)
        {
            System.Data.Common.DbCommand TempCommand_2 = databaseHelper.CreateCommand();
            TempCommand_2.CommandText = String.Format("exec sp_droprolemember '{0}', '{1}'", OldGroup, user);
            TempCommand_2.CommandType = System.Data.CommandType.Text;
            TempCommand_2.Transaction = TransactionManager.GetTransaction(databaseHelper.Connection);
            DbCommand command = TempCommand_2;
            command.ExecuteNonQuery();
            TempCommand_2 = databaseHelper.CreateCommand();
            TempCommand_2.CommandText = String.Format("exec sp_addrolemember '{0}', '{1}'", NewGroup, user);
            command = TempCommand_2;
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseHelper"></param>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void NewPassword(DAODatabaseHelper databaseHelper, string user, string oldPassword, string newPassword)
        {
            this.BeginTransaction();
            System.Data.Common.DbCommand TempCommand_2 = databaseHelper.CreateCommand();
            TempCommand_2.CommandText = String.Format("ALTER LOGIN [{0}] WITH PASSWORD=N'{1}'", user, newPassword);
            TempCommand_2.CommandType = System.Data.CommandType.Text;
            TempCommand_2.Transaction = TransactionManager.GetTransaction(databaseHelper.Connection);
            DbCommand command = TempCommand_2;
            command.ExecuteNonQuery();
            this.CommitTransaction();
        }

		private string Credentials
		{
			get
			{
                if (String.IsNullOrEmpty(user) && String.IsNullOrEmpty(password))
                {
                    return String.Empty;
                }
                if (factory is System.Data.Odbc.OdbcFactory)
                {
                    return String.Format(";UID='{0}';PWD={1};", user, password);
                }
                else if (factory is System.Data.SqlClient.SqlClientFactory)
                {
				return String.Format(";User ID='{0}';Password={1};", user, password);
			}
                else
                {
                    return string.Empty;
                }

			}
		}
		private string ConnectionString
		{
			get
			{
				return System.Configuration.ConfigurationManager.ConnectionStrings["QCCS.Properties.Settings.ConnectionString"].ConnectionString + Credentials;
			}
		}
        private string ConnectionStringSecurity
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["QCCS.Properties.Settings.SecurityConnectionString"].ConnectionString;
            }
        }
    }
}
