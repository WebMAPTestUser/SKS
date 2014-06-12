using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UpgradeHelpers.VB6.DB.DAO
{
    class DbTypesConverter
    {
        static Dictionary<string, KeyValuePair<string, string>> _providerTypes = null;

        static public Dictionary<string, KeyValuePair<string, string>> ProviderTypeMap
        {
            set
            {
                _providerTypes = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static Type ProviderTypeToType(string providerType)
        {
            if (_providerTypes.ContainsKey(providerType))
            {
                return Type.GetType(_providerTypes[providerType].Value);
            }
            return typeof(string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static DbType ProviderTypeToDbType(string providerType)
        {
            Type type = ProviderTypeToType(providerType);
            return TypeToDbType(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public static Type DbTypeToType(DbType dbtype)
        {
            Type result = typeof(System.String);
            switch (dbtype)
            {
                case DbType.Byte:
                    result = typeof(System.Byte);
                    break;
                case DbType.Binary:
                    result = typeof(System.Object);
                    break;
                case DbType.Boolean:
                    result = typeof(System.Boolean);
                    break;
                case DbType.DateTime:
                    result = typeof(System.DateTime);
                    break;
                case DbType.Decimal:
                    result = typeof(System.Decimal);
                    break;
                case DbType.Double:
                    result = typeof(System.Double);
                    break;
                case DbType.Guid:
                    result = typeof(System.Guid);
                    break;
                case DbType.Int16:
                    result = typeof(System.Int16);
                    break;
                case DbType.Int32:
                    result = typeof(System.Int32);
                    break;
                case DbType.Int64:
                    result = typeof(System.Int64);
                    break;
                case DbType.Object:
                    result = typeof(System.Object);
                    break;
                case DbType.SByte:
                    result = typeof(System.SByte);
                    break;
                case DbType.Single:
                    result = typeof(System.Single);
                    break;
                case DbType.String:
                    result = typeof(System.String);
                    break;
                case DbType.UInt16:
                    result = typeof(System.UInt16);
                    break;
                case DbType.UInt32:
                    result = typeof(System.UInt32);
                    break;
                case DbType.UInt64:
                    result = typeof(System.UInt64);
                    break;
            }

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public static string DbTypeToProviderType(DbType dbtype)
        {
            Type type = DbTypeToType(dbtype);
            foreach (KeyValuePair<string,string> t in _providerTypes.Values)
            {
                if (t.Value == type.FullName)
                {
                    return t.Key;
                }
            }
            return "VarChar";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType TypeToDbType(Type type)
        {
            DbType result = DbType.String;
            switch (type.Name)
            {
                case "Byte":
                    result = DbType.Byte;
                    break;
                case "Byte[]":
                    result = DbType.Binary;
                    break;
                case "Boolean":
                    result = DbType.Boolean;
                    break;
                case "DateTime":
                    result = DbType.DateTime;
                    break;
                case "Decimal":
                    result = DbType.Decimal;
                    break;
                case "Double":
                    result = DbType.Double;
                    break;
                case "Guid":
                    result = DbType.Guid;
                    break;
                case "Int16":
                    result = DbType.Int16;
                    break;
                case "Int32":
                    result = DbType.Int32;
                    break;
                case "Int64":
                    result = DbType.Int64;
                    break;
                case "Object":
                    result = DbType.Object;
                    break;
                case "SByte":
                    result = DbType.SByte;
                    break;
                case "Single":
                    result = DbType.Single;
                    break;
                case "String":
                    result = DbType.String;
                    break;
                case "UInt16":
                    result = DbType.UInt16;
                    break;
                case "UInt32":
                    result = DbType.UInt32;
                    break;
                case "UInt64":
                    result = DbType.UInt64;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TypeToProviderType(Type type)
        {
            foreach (KeyValuePair<string, string> t in _providerTypes.Values)
            {
                if (t.Value == type.FullName)
                {
                    return t.Key;
                }
            }
            return "VarChar";
        }
    }
}
