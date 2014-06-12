using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB.ADO
{
    /// <summary>
    /// Static clss to handle Ado connection properties not present on the native .Net connection class
    /// </summary>
    public static class ADOConnectionSettingsHelper
    {
        private const string CURSOR_LOCATION_KEY = "CURSORLOCATION";
        private static Dictionary<DbConnection, Dictionary<string, object>> settings = new Dictionary<DbConnection, Dictionary<string, object>>();
        /// <summary>
        /// Gets the provider information stored on the connection string
        /// </summary>
        /// <param name="connection">The connection object to get the connection string from</param>
        /// <returns>An string providing the provider information</returns>
        public static String GetConnectionProvider(DbConnection connection)
        {
            object result = String.Empty;
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connection.ConnectionString;
            builder.TryGetValue("Provider", out result);
            return result == null ? string.Empty : (string)result;
        }

        /// <summary>
        /// Gets the cursor location assigned to an specific connection
        /// </summary>
        /// <param name="connection">The connection to get the inforrmation from</param>
        /// <returns>The cursor location asigned to the connection</returns>
        public static CursorLocationEnum GetCursorLocation(DbConnection connection)
        {
            CursorLocationEnum result = CursorLocationEnum.adUseClient;
            Dictionary<string, object> thevalue = null;
            object actuallocation = null;
            settings.TryGetValue(connection, out thevalue);
            if (thevalue == null)
            {
                SetCursorLocation(connection, result);
            }
            else
            {
                thevalue.TryGetValue(CURSOR_LOCATION_KEY, out actuallocation);
                if (actuallocation == null)
                    thevalue.Add(CURSOR_LOCATION_KEY, result);
                else
                    result = (CursorLocationEnum)actuallocation;
            }
            return result;
        }

        /// <summary>
        /// Sets the cursor location to an specific connection
        /// </summary>
        /// <param name="connection">The connection to assign the inforrmation from</param>
        /// <param name="location">The cursor location to be assingned</param>
        public static void SetCursorLocation(DbConnection connection, CursorLocationEnum location)
        {
            Dictionary<string,object> thevalue = null;
            object actuallocation = null;
            settings.TryGetValue(connection, out thevalue);
            if (thevalue == null)
            {
                thevalue = new Dictionary<string, object>();
                thevalue.Add(CURSOR_LOCATION_KEY, location);
                settings.Add(connection, thevalue);
            }
            else
            {
                thevalue.TryGetValue(CURSOR_LOCATION_KEY, out actuallocation);
                if (actuallocation == null)
                    thevalue.Add(CURSOR_LOCATION_KEY, location);
                else
                    thevalue[CURSOR_LOCATION_KEY] = location;
            }
        }
    }
}
