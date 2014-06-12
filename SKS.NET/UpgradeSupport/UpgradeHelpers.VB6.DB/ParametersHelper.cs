using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace UpgradeHelpers.VB6.DB
{
    /// <summary>
    /// Class to handle Database command parameters complex functionality
    /// </summary>
    public static class ParametersHelper
    {
        /// <summary>
        /// Extracts the command information from the command object and add specific information based on the factory being use.
        /// </summary>
        /// <param name="theCommand">Command to be processed.</param>
        /// <param name="factory">The factory to be use.</param>
        public static void DeriveParameters(DbCommand theCommand, DbProviderFactory factory)
        {
            theCommand.Parameters.Clear();
            if (theCommand.CommandType == CommandType.StoredProcedure)
            {
                using (DbConnection conn = factory.CreateConnection())
                {
                    conn.ConnectionString = theCommand.Connection.ConnectionString;
                    conn.Open();
                    using (DbCommand pivotCommand = factory.CreateCommand())
                    {
                        pivotCommand.CommandText = theCommand.CommandText;
                        pivotCommand.CommandType = theCommand.CommandType;
                        pivotCommand.Connection = conn;
                        if (theCommand is System.Data.OleDb.OleDbCommand)
                        {
                            if (conn.ConnectionString.Contains("Provider=Microsoft.Jet"))
                            {
                                DeriveParametersFromProcedureCode(conn, pivotCommand);
                            }
                            else
                                System.Data.OleDb.OleDbCommandBuilder.DeriveParameters((System.Data.OleDb.OleDbCommand)pivotCommand);
                        }
                        else if (theCommand is System.Data.SqlClient.SqlCommand)
                        {
                            System.Data.SqlClient.SqlCommandBuilder.DeriveParameters((System.Data.SqlClient.SqlCommand)pivotCommand);
                        }
                        else if (theCommand is System.Data.OracleClient.OracleCommand)
                        {
                            System.Data.OracleClient.OracleCommandBuilder.DeriveParameters((System.Data.OracleClient.OracleCommand)pivotCommand);
                        }
                        foreach (DbParameter parameter in pivotCommand.Parameters)
                        {
                            System.Data.IDataParameter cloneParameter = (System.Data.IDataParameter)((ICloneable)parameter).Clone();
                            theCommand.Parameters.Add(cloneParameter);
                        }
                    }
                }
            }
        }
        

        /// <summary>
        /// Extracts the command information from the command object and add specific information based on the factory being use.
        /// </summary>
        /// <param name="connection">The connection to extract the information from.</param>
        /// <param name="pivotCommand">Command to be processed.</param>
        private static void DeriveParametersFromProcedureCode(DbConnection connection, DbCommand pivotCommand)
        {
            DataTable dbObjects = connection.GetSchema("Procedures", new String[] { null, null, pivotCommand.CommandText });
            if (dbObjects.Rows.Count > 0)
            {
                String procText = dbObjects.Rows[0]["PROCEDURE_DEFINITION"].ToString();
                String[] procLines = procText.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if ((procLines.Length > 0) && procLines[0].StartsWith("PARAMETERS", StringComparison.InvariantCultureIgnoreCase))
                {
                    procLines = procLines[0].ToUpper().Replace("PARAMETERS", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (String paraminfo in procLines)
                    {
                        string[] param = paraminfo.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        DbParameter parameter = pivotCommand.CreateParameter();
                        parameter.ParameterName = param[0];
                        parameter.DbType = getDbType(param[1]);
                        pivotCommand.Parameters.Add(parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the corresponding DBType for the string received has parameter.
        /// </summary>
        /// <param name="p">The string with the name of the type to convert to DBType.</param>
        /// <returns>The DBType that correspond to the name revieved has parameter, otherwise DBType.String.</returns>
        private static DbType getDbType(string p)
        {
            DbType result = DbType.String;
            switch (p)
            {
                case "Short":
                    result = DbType.Int16;
                    break;
                case "Decimal":
                    result = DbType.Decimal;
                    break;
                case "DateTime":
                    result = DbType.DateTime;
                    break;
            }
            return result;
        }
    }
}
