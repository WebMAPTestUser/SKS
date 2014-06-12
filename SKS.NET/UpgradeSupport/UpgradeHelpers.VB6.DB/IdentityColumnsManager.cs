using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace UpgradeHelpers.VB6.DB
{
    /// <summary>
    /// This class stores information on the columns whose 
    /// values are set on the database automatically, 
    /// either by triggers or automatically because they are an identity column.
    ///<AdoIdentityColumns>
    /// <IdentityColumns>
    ///     <add tablename="CR_Customreport">
    ///         <columns>
    ///             <add columnname="CR_ID" sequencename="CR_ID"/>
    ///         </columns>
    ///     </add>
    /// </IdentityColumns>
    ///</AdoIdentityColumns>
    /// </summary>
    public class IdentityColumnsManager
    {
        /// <summary>
        /// Holds the section instance.
        /// </summary>
        private static AdoIdentityColumnsConfigurationSection configSection = null;
        /// <summary>
        /// Holds the indentities information.
        /// </summary>
        private static Dictionary<String, Dictionary<String, String>> identities = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// Gets the configurtion section instance.
        /// </summary>
        private static AdoIdentityColumnsConfigurationSection ConfigSection
        {
            get
            {
                if (configSection == null)
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    configSection = (AdoIdentityColumnsConfigurationSection)config.GetSection(AdoIdentityColumnsConfigurationSection.SECTION_NAME);
                }
                return configSection;
            }
        }

        /// <summary>
        /// Gets the identity information for an specific table.
        /// </summary>
        /// <param name="_TableName">The name of the table to get the identity information.</param>
        /// <returns>A dictionary containing the indentity information for the specified table.</returns>
        public static Dictionary<String, String> GetIndentityInformation(String _TableName)
        {
            String TableName = _TableName.ToUpper();
            Dictionary<String, String> result = null;
            if (!identities.TryGetValue(TableName, out result) && ConfigSection != null)
            {
                AdoIdentityTableColumnConfigurationElement table = ConfigSection.Tables[TableName];
                if (table != null)
                {
                    identities.Add(table.TableName, new Dictionary<string, string>());
                    foreach (AdoIdentityColumnConfigurationElement element in table.Columns)
                    {
                        identities[table.TableName].Add(element.ColumnName, element.SequenceName);
                    }
                    result = identities[table.TableName];
                }
            }
            return result;
        }
    }
    /// <summary>
    /// Represents the configuration section to handle the identity column information.
    /// </summary>
    internal class AdoIdentityColumnsConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The section name.
        /// </summary>
        public const string SECTION_NAME = "AdoIdentityColumns";
        /// <summary>
        /// Gets the tables collections.
        /// </summary>
        [ConfigurationProperty("IdentityColumns", IsRequired = true)]
        public AdoIdentityColumnTableConfigurationElementCollection Tables
        {
            get { return (AdoIdentityColumnTableConfigurationElementCollection)base["IdentityColumns"]; }
            set { base["IdentityColumns"] = value; }
        }
    }

    /// <summary>
    /// The collection of identity elements.
    /// </summary>
    internal class AdoIdentityColumnTableConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new element.
        /// </summary>
        /// <param name="elementName">The name of the new element to be created.</param>
        /// <returns>The new instance of the configuration element.</returns>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new AdoIdentityTableColumnConfigurationElement(elementName);
        }

        /// <summary>
        /// Creates a new element.
        /// </summary>
        /// <returns>The new instance of the configuration element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new AdoIdentityTableColumnConfigurationElement();
        }

        /// <summary>
        /// Gets the key of the element in the collection.
        /// </summary>
        /// <param name="element">The element to get the key from.</param>
        /// <returns>The element key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AdoIdentityTableColumnConfigurationElement)element).TableName;
        }

        /// <summary>
        /// Indexer to retrieve an specific element.
        /// </summary>
        /// <param name="name">The element key.</param>
        /// <returns>The element corresponding to the key.</returns>
        public new AdoIdentityTableColumnConfigurationElement this[String name]
        {
            get
            {
                return (AdoIdentityTableColumnConfigurationElement)BaseGet(name);
            }
        }
        /// <summary>
        /// Indexer to retrieve an specific element.
        /// </summary>
        /// <param name="index">The index key.</param>
        /// <returns>The element corresponding to the index.</returns>
        public AdoIdentityTableColumnConfigurationElement this[int index]
        {
            get
            {
                return (AdoIdentityTableColumnConfigurationElement)BaseGet(index);
            }
        }
        /// <summary>
        /// Adds a new element to the collection.
        /// </summary>
        /// <param name="tableConfig">The factory to be added.</param>
        public void Add(AdoIdentityTableColumnConfigurationElement tableConfig)
        {
            BaseAdd(tableConfig);
        }
        /// <summary>
        /// Adds a new element to the collection.
        /// </summary>
        /// <param name="element">The element to be added.</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
    /// <summary>
    /// The configuration element to define the identity columns on a table.
    /// </summary>
    internal class AdoIdentityTableColumnConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Creates a new element.
        /// </summary>
        public AdoIdentityTableColumnConfigurationElement() { }
        /// <summary>
        /// Creates a new element with the specific name.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        public AdoIdentityTableColumnConfigurationElement(String tableName)
        {
            TableName = tableName;
        }
        /// <summary>
        /// Creates a new element with the specific values.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="colums">The columns collection.</param>
        public AdoIdentityTableColumnConfigurationElement(String tableName, AdoIdentityColumnConfigurationElementCollection colums)
        {
            TableName = tableName;
            Columns = colums;
        }
        /// <summary>
        /// Gets and sets the Table name.
        /// </summary>
        [ConfigurationProperty("tablename", IsRequired = true, IsKey = true)]
        public String TableName
        {
            get { return ((string)this["tablename"]).ToUpper(); }
            set { this["tablename"] = value; }
        }
        /// <summary>
        /// Gets and sets the colimns collection.
        /// </summary>
        [ConfigurationProperty("columns", IsRequired = true)]
        public AdoIdentityColumnConfigurationElementCollection Columns
        {
            get { return (AdoIdentityColumnConfigurationElementCollection)this["columns"]; }
            set { this["columns"] = value; }
        }
    }

    /// <summary>
    /// Class to ADO Identity column configuration
    /// </summary>
    internal class AdoIdentityColumnConfigurationElementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new AdoIdentityColumnConfigurationElement(elementName);
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new AdoIdentityColumnConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AdoIdentityColumnConfigurationElement)element).ColumnName;
        }

        public new AdoIdentityColumnConfigurationElement this[String name]
        {
            get
            {
                return (AdoIdentityColumnConfigurationElement)BaseGet(name);
            }
        }

        public AdoIdentityColumnConfigurationElement this[int index]
        {
            get
            {
                return (AdoIdentityColumnConfigurationElement)BaseGet(index);
            }
        }

        public void Add(AdoIdentityColumnConfigurationElement columnConfig)
        {
            BaseAdd(columnConfig);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

    }

    internal class AdoIdentityColumnConfigurationElement : ConfigurationElement
    {
        public AdoIdentityColumnConfigurationElement() { }
        public AdoIdentityColumnConfigurationElement(String columnName)
        {
            ColumnName = columnName;
        }
        public AdoIdentityColumnConfigurationElement(String columnName, String sequenceName)
        {
            ColumnName = columnName;
            SequenceName = sequenceName;
        }

        [ConfigurationProperty("columnname", IsRequired = true, IsKey = true)]
        public String ColumnName
        {
            get { return (string)this["columnname"]; }
            set { this["columnname"] = value; }
        }

        [ConfigurationProperty("sequencename", IsRequired = true)]
        public String SequenceName
        {
            get { return (string)this["sequencename"]; }
            set { this["sequencename"] = value; }
        }
    }

}
