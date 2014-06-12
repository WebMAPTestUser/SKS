using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace UpgradeHelpers.VB6.Activex
{


    
    /// <summary>
    /// This interface is used for exposing the ComponsentServeHelper Class thru remoting
    /// </summary>
    public interface IComponentServerHelper
    {
        /// <summary>
        /// Creates an instance of T in the corresponding space of memory or Process
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="oldInstance">The instance being freed if it had a referenced instance</param>
        /// <param name="isExternal">Indicates if instance will be referenced externally</param>
        /// <returns>An instance of type T</returns>
        T CreateInstance<T>(object oldInstance, bool isExternal) where T : ComponentClassHelper;
        /// <summary>
        /// Creates an instance of T in the corresponding space of memory or Process
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="oldInstance">The instance being freed if it had a referenced instance</param>
        /// <returns>An instance of type T</returns>
        T CreateInstance<T>(object oldInstance) where T : ComponentClassHelper;
        /// <summary>
        /// Creates an instance of T in the corresponding space of memory or Process assuming that there was no previous instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CreateInstance<T>() where T : ComponentClassHelper;
        /// <summary>
        /// Frees a component instance and checks if Domain/GlobalVars should be initialized
        /// </summary>
        /// <param name="instance">The instance to be freed</param>
        int DisposeInstance<T>(T instance) where T : ComponentClassHelper;

        /// <summary>
        /// Gets the default instance for this type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetDefaultInstance<T>() where T : ComponentClassHelper;

        /// <summary>
        /// Returns the number of instances managed by this CS
        /// </summary>
        int InstancesCount
        {
            get;
        }


        /// <summary>
        /// @todo @developer review if we can merge it with the ReleaseCOM feature
        /// </summary>
        /// <param name="p"></param>
        void DisposeInstanceByUri(string p);
    }

    /// <summary>
    /// Classes migrated from an ActiveX-Dll or ActiveX-Exe have internal contructors. 
    /// This is done to make sure that this classes are not instantiated directly.
    /// Due to this restriction a method is added to the Factory to instantiate 
    /// classes and that method is "passed" to the ComponentServerImplementations with a delegate of this type.
        /// </summary>
    /// <param name="instanceType">The type of the instance you want</param>
    /// <returns></returns>
    public delegate object MakeNewInstanceDelegate(Type instanceType);


    /// <summary>
    /// This enumeration is used to specify the kind of implementation that will be used
    /// </summary>
    public enum ComponentServerImplementationType
    {
        /// <summary>
        /// Indicates that the implementation used will run in the current domain
        /// </summary>
        NoDomains,
        /// <summary>
        /// Indicates that the implementation will create new AppDomains to host new SingleUse ActiveX-Exe instances
        /// </summary>
        AppDomains,
        /// <summary>
        /// Indicates that the implementationwill create new a Process to host new SingleUse ActiveX-Exe instances
        /// </summary>
        Process
    }

    /// <summary>
    /// Delegate that points to a method, which function is to initialize the global vars to the corresponding module.
    /// This delegate should be registered (RegisterInitGlobalVarsDelegate) if UseDomain flag is Off
    /// </summary>
    public delegate void InitGlobalVarsDelegate();

    /// <summary>
    /// Marks a class like ComponentClassHelper type, it means this class could be instantiated via the ComponentServerHelper
    /// </summary>
    public class ComponentClassHelper : MarshalByRefObject
    {
        /// <summary>
        /// Register init global variables delegates
        /// </summary>
        public virtual void RegisterInitGlobalVarsDelegates()
        {
        }
    }

    /// <summary>
    /// Marks a class like ComponentSingleUseClassHelper type, it means this class could be instantiated via the ComponentServerHelper
    /// and it behaves like a VB6 SingleUse class. It could be created in a new domain
    /// </summary>
    public class ComponentSingleUseClassHelper : ComponentClassHelper
    {
    }

    /// <summary>
    /// Marks a class like GlbComponentSingleUseClassHelper type, it means this class could be instantiated via the ComponentServerHelper
    /// and it behaves like a VB6 GlbSingleUse class. It could be created in a new domain
    /// </summary>
    public class GlbComponentSingleUseClassHelper : ComponentClassHelper
    {
    }
}
