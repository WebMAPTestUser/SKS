using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB
{
    /// <summary>
    /// Base class to provide the DBEngine functionality.
    /// </summary>
    public class EngineHelper<T> where T : ConnectionContainers, new()
    {
        /// <summary>
        /// connectionContainers Template List
        /// </summary>
        protected List<T> connectionContainers;
        /// <summary>
        /// DbProvider factory
        /// </summary>
        protected DbProviderFactory factory;
        /// <summary>
        /// Singleton Instance
        /// </summary>
        protected static EngineHelper<T> instance;
    
        /// <summary>
        /// EngineHelper Constructor
        /// </summary>
        /// <param name="factory">set internal factory instance</param>
        protected EngineHelper(DbProviderFactory factory)
        {
            this.factory = factory;
            connectionContainers = new List<T>();
            T container = new T();
            container.Factory = factory;
            connectionContainers.Add(container);
        }
        /// <summary>
        /// Array access
        /// </summary>
        /// <param name="index">index to access</param>
        /// <returns></returns>
        public virtual T this[int index]
        {
            get
            {
               return connectionContainers[index];
            }
        }
    }
}
