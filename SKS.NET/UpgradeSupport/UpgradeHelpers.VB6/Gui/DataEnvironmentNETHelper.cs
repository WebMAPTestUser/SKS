using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Class provided to recreate a DataEnvironment as a DataSet in .NET (Migration from ADODB to ADO.NET).
    /// </summary>
    public class DataEnvironmentNETHelper : DataSet
    {
        private Dictionary<string, DEConnectionHelper> m_Connections = new Dictionary<string, DEConnectionHelper>();
        private Dictionary<string, DECommandHelper> m_Commands = new Dictionary<string, DECommandHelper>();
        private List<OleDbConnection> openConnections = new List<OleDbConnection>();

        /// <summary>
        /// Helper class created to represent a connection within a DataEnvironmentNETHelper.
        /// </summary>
        public class DEConnectionHelper
        {
            private OleDbConnection _Connection = null;
            internal DbConnection Connection
            {
                get
                {
                    return _Connection;
                }
            }

            internal DEConnectionHelper(string connectionString)
            {
                _Connection = new OleDbConnection(connectionString);
            }
        }

        /// <summary>
        /// Helper class created to represent a command within a DataEnvironmentNETHelper.
        /// </summary>
        public class DECommandHelper
        {
            private OleDbCommand _Command = null;
            internal DbCommand Command
            {
                get
                {
                    return _Command;
                }
            }

            internal DECommandHelper(string commandText, DEConnectionHelper ConnectionHelper)
            {
                _Command = new OleDbCommand(commandText, (OleDbConnection)ConnectionHelper.Connection);
            }
        }

        /// <summary>
        /// Refresh all the data and tables represented by the connections and commands added to this instance.
        /// </summary>
        public void RefreshDataEnvironment()
        {
            OleDbCommand cmd = null;
            OleDbDataAdapter adapter = new OleDbDataAdapter();

            try
            {
                this.Tables.Clear();

                foreach (string cmdName in m_Commands.Keys)
                {
                    cmd = (OleDbCommand)m_Commands[cmdName].Command;

                    CheckIfConnectionIsOpened(cmdName);
                    adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(this, cmdName);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseOpenedConnections();
            }
        }

        /// <summary>
        /// Add a connection to the DataEnvironmentNETHelper.
        /// </summary>
        /// <param name="connectionName">The name of the connection.</param>
        /// <param name="connectionString">The connection string to establish the connection.</param>
        /// <returns>A DEConnectionHelper representing the Connection just added.</returns>
        public DEConnectionHelper AddConnection(string connectionName, string connectionString)
        {
            if (m_Connections.ContainsKey(connectionName))
                throw new Exception("The connection name '" + connectionName + "' is already in use");

            m_Connections.Add(connectionName, new DEConnectionHelper(connectionString));
            return m_Connections[connectionName];
        }

        /// <summary>
        /// Adds a command to the DataEnvironmentNETHelper.
        /// </summary>
        /// <param name="connnectionName">The name of the connection to execute the command.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <returns>A DECommandHelper instance representing the Command just added.</returns>
        public DECommandHelper AddCommand(string connnectionName, string commandName, string commandText)
        {
            string newCommandName = connnectionName + "_" + commandName;

            if (!m_Connections.ContainsKey(connnectionName))
                throw new Exception("The connection name '" + connnectionName + "' has not been defined");

            if (m_Commands.ContainsKey(newCommandName))
                throw new Exception("The command name '" + newCommandName + "' is already in use");

            m_Commands.Add(newCommandName, new DECommandHelper(commandText, m_Connections[connnectionName]));

            return m_Commands[newCommandName];
        }

        /// <summary>
        /// Close all connections opened after the invocations of CheckIfConnectionIsOpened.
        /// </summary>
        private void CloseOpenedConnections()
        {
            foreach (OleDbConnection con in openConnections)
            {
                try
                {
                    con.Close();
                }
                catch { }
            }
            openConnections.Clear();
        }

        /// <summary>
        /// Verifies and if necessary open the connection associated with a command.
        /// </summary>
        /// <param name="cmdName">The command name.</param>
        private void CheckIfConnectionIsOpened(string cmdName)
        {
            OleDbCommand cmd = null;
            try
            {
                cmd = (OleDbCommand)m_Commands[cmdName].Command;

                if (cmd.Connection == null)
                    throw new Exception("The connection is not set");

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                if (!openConnections.Contains(cmd.Connection))
                    openConnections.Add(cmd.Connection);
            }
            catch (Exception e)
            {
                throw new Exception("Exception processing the connection for the command '" + cmdName + "'. "
                    + e.Message);
            }
        }

        /// <summary>
        /// Release unmanaged resources from memory
        /// </summary>
        /// <param name="Disposing">To Release unmanaged and managed resources</param>
        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                foreach (DEConnectionHelper conn in m_Connections.Values)
                {
                    try
                    {
                        if (conn.Connection.State != ConnectionState.Closed)
                            conn.Connection.Close();
                    }
                    catch { }
                }
                m_Commands.Clear();
                m_Connections.Clear();
                openConnections.Clear();

                m_Commands = null;
                m_Connections = null;
                openConnections = null;
            }
            base.Dispose(Disposing);
        }

    }
}
