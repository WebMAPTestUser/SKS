using System.Threading;
using System.Diagnostics;
using System;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Management;
using System.Collections;


namespace UpgradeHelpers.VB6.Activex
{

    /// <summary>
    /// Codes used for ComponentServer manager implementations
    /// </summary>
    internal enum ComponentServerCodes
    {
        CS_ERROR = -1,
        CS_OK = 1,
        CS_REMOVE_FACTORY = 100
    }
    /// <summary>
    /// This factory allows you to choose different 
    /// </summary>
    public abstract class ComponentServerFactory : MarshalByRefObject, IComponentServerHelper
    {

        /// <summary>
        /// Sponsor object used to ping the client from the server to identify remote intances that are no 
        /// longer needed
        /// </summary>
        public ClientSponsor Sponsor = new ClientSponsor();

        private IComponentServerHelper helper;
        Dictionary<WeakReference, String> managedObjects = new Dictionary<WeakReference, String>();

        /// <summary>
        /// This flag indicates if this factory was created in a Process started from the
        /// same assembly where the Class was defined
        /// </summary>
        bool createdInNewProcess = false;

        /// <summary>
        /// This timer thread is used to run the LifeTimeCheck clean-up method
        /// </summary>
        System.Threading.Timer timer = null;

        /// <summary>
        /// The CS has a worker thread that detects if the CS has no more instances and then kills
        /// this process. But this thread should not act, until at least one instance has been created.
        /// This is specially to make sure that it does not gets kill before time.
        /// </summary>
        internal bool instanceHasBeenCreated = false;

        /// <summary>
        /// Holds a list of the default instances (Global) for each type
        /// </summary>
        Dictionary<Type, object> deflist_handles = null;





        /// <summary>
        /// Factories must implement a method that creates new instances.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns>A newly created instance</returns>
        public abstract object MakeNewInstance(Type instanceType);

        /// <summary>
        /// Property to give access to a Factory singlenton
        /// </summary>
        protected abstract ComponentServerFactory Instance
        {
            get;
            set;
        }


        /// <summary>
        /// Makes the lifetime for this object unlimited, the component server implementation
        /// is in charge on managing instances lifetime
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }



        /// <summary>
        /// We need to determine if this factory was instanciate from the same assembly that contains
        /// this factory or if it was instanciate from another assembly.
        /// This is important becuase it let us know it some Worker Thread must be instanciate to monitor
        /// ComponentServer 
        /// This constructor is used to allow to factory to know if it was instanciated 
        /// inside a new process
        /// </summary>
        /// <param name="wasCreatedInThisAssembly"></param>
        internal void InitializationCode(bool wasCreatedInThisAssembly)
        {
       
            Debug.Assert(Instance == null);
            createdInNewProcess = wasCreatedInThisAssembly;
            Instance = this;
            if (createdInNewProcess)
            {
                //Worker Thread to track instances
                TimerCallback cb = new TimerCallback(LifeTimeCheck);
                timer = new System.Threading.Timer(cb, this, 4000, 4000);
            }
            else if (!createdInNewProcess)
            {
                //This is a worker thread that tracks references.
                //It helps to determine if there are memory leaks
               // System.Threading.TimerCallback cb = new System.Threading.TimerCallback(CheckForUnReferencedObjects);
            }

        }

        /// <summary>
        /// Default Factory Constructor.
        /// </summary>
        public ComponentServerFactory() 
        {
            Trace.AutoFlush = true;//This avoids exceptions during finalization.
            Trace.TraceInformation("InitInstance: {0}=={1} => {2}", System.Reflection.Assembly.GetEntryAssembly(), this.GetType().Assembly, System.Reflection.Assembly.GetEntryAssembly() == this.GetType().Assembly);
            bool wasCreateFromAnotherProcess = System.Reflection.Assembly.GetEntryAssembly() == this.GetType().Assembly;
            Trace.TraceInformation("ComponentServerFactory::ctor({0}) -- Type {1}", wasCreateFromAnotherProcess, this.GetType().Name);
            InitializationCode(wasCreateFromAnotherProcess && ImplementationType!=ComponentServerImplementationType.AppDomains);
        }

        internal bool HasCreatedAnyInstances
        {
            get
            {
                return instanceHasBeenCreated;
            }
        }

        /// <summary>
        /// This token is used as a flag to indicate that this CS can be removed
        /// </summary>
        internal String component_server_removal_token = String.Empty;
        internal bool HasToken
        {
            get
            {
                return component_server_removal_token != String.Empty;
            }
        }


        /// <summary>
        /// Clears any remaining removal token
        /// </summary>
        /// <returns></returns>
        internal bool ClearRemovalToken()
        {
            Trace.TraceInformation("ComponentServerFactory::ClearRemovalToken");
            if (component_server_removal_token != String.Empty)
            {
                //First let's clear this flag
                try
                {
                    EventWaitHandle ewh = EventWaitHandle.OpenExisting(component_server_removal_token);
                    ewh.Set();
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("Error while releasing token: {0}", ex.Message);
                }
                finally
                {
                    component_server_removal_token = String.Empty;
                }
                return true;
            }
            Trace.TraceInformation("ComponentServerFactory::ClearRemovalToken END");
            return false;
        }

        /// <summary>
        /// This method is called from a Timer Worker thread and it makes sure to remove 
        /// a CS if it was no more instances.
        /// </summary>
        /// <param name="obj"></param>
        internal static void LifeTimeCheck(object obj)
        {
            //Trace.TraceInformation("ComponentServerFactory::LifeTimeCheck");
            ComponentServerFactory factory = obj as ComponentServerFactory;
            if (!factory.HasCreatedAnyInstances)
            {
                //  Trace.TraceInformation("ComponentServerFactory::LifeTimeCheck no instances has been created yet");
                return;
            }
            if (factory.HasToken && !factory.ClearRemovalToken())
            {
                return; //We will try again later
            }
            int instance_count = factory.InstancesCount;
            //Trace.TraceInformation("Instances=" + instance_count);
            if (instance_count == 0)
            {
                if (Monitor.TryEnter(factory))
                {
                    try
                    {
                        Trace.TraceInformation("kill kill kill");
                        Trace.Flush();
                        ChannelServices.UnregisterChannel(channel);
                        Process.GetCurrentProcess().Kill();

                    }
                    finally
                    {
                        Monitor.Exit(factory);
                    }
                }
                else
                {
                    Trace.TraceInformation("Monitor denied, some activity is present, let's try next time");
                }
            }
            //Trace.TraceInformation("ComponentServerFactory::LifeTimeCheck END");   
        }

        private static void CheckForUnReferencedObjects(object obj)
        {
            ComponentServerFactory factory = obj as ComponentServerFactory;
            if (factory != null)
            {
                if (factory.managedObjects.Count == 0)
                {
                    Trace.TraceInformation("No references to remove. Everything is normal");
                    return;
                }
                List<WeakReference> refsToRemove = new List<WeakReference>();
                lock (factory.managedObjects)
                {
                    foreach (WeakReference weakRef in factory.managedObjects.Keys)
                    {
                        if (!weakRef.IsAlive)
                        {
                            System.Diagnostics.Trace.TraceInformation("A dead reference found trying to remove it by uri");
                            refsToRemove.Add(weakRef);
                        }
                    }
                }
                lock (factory.managedObjects)
                {
                    foreach (WeakReference uriRef in refsToRemove)
                    {

                        if (factory.helper != null)
                        {
                            System.Diagnostics.Trace.TraceInformation("Removing by uri {0}", uriRef.ToString());
                            String remotingUri = String.Empty;
                            bool found = factory.managedObjects.TryGetValue(uriRef, out remotingUri);
                            if (found)
                            {
                                try
                                {
                                    factory.helper.DisposeInstanceByUri(factory.managedObjects[uriRef]);
                                }
                                catch (RemotingException remotingException)
                                {
                                    Trace.TraceError(" {0} -- {1}", remotingException.Message, remotingException.InnerException);
                                    factory.helper = null;
                                }
                            }
                            else
                            {
                                Trace.TraceError("Weak reference not found during clean up!");
                            }
                            factory.managedObjects.Remove(uriRef);
                        }

                    }
                }
            }
        }




        /// <summary>
        /// Indicates which implementation to use for this factory
        /// </summary>
        protected abstract ComponentServerImplementationType ImplementationType
        {
            get;
        }

        internal void CleanupDanglingInstances()
        {
            if (helper is ComponentServerHelperProcessImplementation)
            {
                ComponentServerHelperProcessImplementation helperP = helper as ComponentServerHelperProcessImplementation;

                List<ComponentClassHelper> instancesToDispose = new List<ComponentClassHelper>();
                lock (helperP.sponsors)
                {
                    foreach (KeyValuePair<ComponentClassHelper, ClientSponsor> pair in helperP.sponsors)
                    {
                        try
                        {
                            if (pair.Value != null)
                                pair.Value.Ping();
                        }
                        catch (RemotingException)
                        {
                            instancesToDispose.Add(pair.Key);
                        }
                    }

                    foreach (ComponentClassHelper instance in instancesToDispose)
                    {
                        DisposeInstance(instance);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the number of instances managed by this Factory
        /// </summary>
        public int InstancesCount
        {
            get
            {
                CleanupDanglingInstances();
                if (helper == null)
                    return 0;
                else
                    return helper.InstancesCount;
            }
        }

        internal bool useProcess
        {
            get
            {
               //@todo Review this implementation
                return true;
             }

        }



        /// <summary>
        /// Makes sure that the "helper" variable is initialized.
        /// </summary>
        /// <typeparam name="T">The type parameter is used, in case a remote factory is built, because the T type will be used to locate the Assembly that contains it</typeparam>
        internal void InitHelper<T>()
        {
            //if helper is already set, just return
            if (helper != null) return;
            if (ImplementationType == ComponentServerImplementationType.NoDomains)
                helper = ComponentServerHelperAppDomainImplementation.GetInstance(this, false /*useDomains*/);
            else if (ImplementationType == ComponentServerImplementationType.AppDomains)
                helper = ComponentServerHelperAppDomainImplementation.GetInstance(this, true /*useDomains*/);
            else
            {
                //If this code is being executed from the same assembly where the 
                //Factory was defined then we are already "createdInNewProcess", so this
                //process will be the host for the CS.
                //Otherwise then we must seek if there is already a process running as a server
                //or run a new one
                if (createdInNewProcess)
                    helper = new ComponentServerHelperProcessImplementation(this);
                else
                    helper = ProcessManager.GetFactory<T>();
            }
        }



        #region IComponentServerHelper Members

        /// <summary>
        /// Creates a new instance. A reference to an oldinstance can be passed in order to 
        /// release resources from the previous instance prior to the creation of a new one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldInstance">The instance to be release or null if there isn't any</param>
        /// <param name="isExternal">Indicates if instance will be referenced externally</param>
        /// <returns></returns>
        public T CreateInstance<T>(object oldInstance, bool isExternal) where T : ComponentClassHelper
        {
            Monitor.Enter(this);
            Trace.TraceInformation("ComponentServerFactory::CreateInstance<{0}>() -- Factory {1}", typeof(T),this.GetType().Name);
            InitHelper<T>();
            try
            {
                if (helper as ComponentServerFactory != null)
                {
                    (helper as ComponentServerFactory).Sponsor = this.Sponsor;
                }
                T res = helper.CreateInstance<T>(oldInstance, isExternal);
                if (ImplementationType == ComponentServerImplementationType.Process)
                {

                    MarshalByRefObject mbroRef = res as MarshalByRefObject;
                    if (mbroRef != null)
                    {
                        lock (managedObjects)
                        {
                            managedObjects.Add(new WeakReference(res), RemotingServices.GetObjectUri(mbroRef));
                        }
                    }
                    if (startupFinished) //This invocation could have been done from the ActiveX main
                    {
                        instanceHasBeenCreated = true;
                    }
                }
                return res;
            }
            finally
            {
                Trace.TraceInformation("ComponentServerFactory::CreateInstance<{0}>() END", typeof(T));
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Creates a new instance. A reference to an oldinstance can be passed in order to 
        /// release resources from the previous instance prior to the creation of a new one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldInstance">The instance to be release or null if there isn't any</param>
        /// <returns></returns>
        public T CreateInstance<T>(object oldInstance) where T : ComponentClassHelper
        {
            return CreateInstance<T>(oldInstance, true);
        }


        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <typeparam name="T">Type for new intance</typeparam>
        /// <returns></returns>
        public T CreateInstance<T>() where T : ComponentClassHelper
        {
            Monitor.Enter(this);
            Trace.TraceInformation("ComponentServerFactory::CreateInstance<{0}>", typeof(T));
            InitHelper<T>();
            T res = null;
            try
            {
                if (helper as ComponentServerFactory != null)
                {
                    (helper as ComponentServerFactory).Sponsor = this.Sponsor;
                }
                res = helper.CreateInstance<T>();
                if (ImplementationType == ComponentServerImplementationType.Process)
                {
                    MarshalByRefObject mbroRef = res as MarshalByRefObject;
                    if (mbroRef != null)
                    {
                        lock (managedObjects)
                        {
                            managedObjects.Add(new WeakReference(res), RemotingServices.GetObjectUri(mbroRef));
                        }
                    }
                }
                if (startupFinished) //This invocation could have been done from the ActiveX main
                {
                    instanceHasBeenCreated = true;
                }
            }
            finally
            {
                Trace.TraceInformation("ComponentServerFactory::CreateInstance<{0}>() END", typeof(T));
                Monitor.Exit(this);
            }
            return res;
        }

        /// <summary>
        /// This method is used to reset the underlying helper when there
        /// are no more instances
        /// </summary>
        internal virtual void ResetFactory()
        {
            helper = null;
        }


        /// <summary>
        /// Releases the given instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns>A value from the ComponentServerCodes enumeration</returns>
        public int DisposeInstance<T>(T instance) where T : ComponentClassHelper
        {
            Monitor.Enter(this);
            Trace.TraceInformation("ComponentServerFactory::Dispose<{0}>({1})", typeof(T), instance);
            Trace.Flush();
            InitHelper<T>();
            try
            {
                String uri = RemotingServices.GetObjectUri(instance as MarshalByRefObject);
                int res = helper.DisposeInstance<T>(instance);
                if (res == (int)ComponentServerCodes.CS_REMOVE_FACTORY && useProcess && !createdInNewProcess)
                {
                    if (!String.IsNullOrEmpty(component_server_removal_token))
                        ClearRemovalToken(); //We clear any previous token
                    ResetFactory();
                }
                if (res == (int)ComponentServerCodes.CS_REMOVE_FACTORY && useProcess)
                    component_server_removal_token = uri;
                return res;
            }
            catch
            {
                ResetFactory();
                return (int)ComponentServerCodes.CS_ERROR;
            }
            finally
            {
                Trace.TraceInformation("ComponentServerFactory::Dispose<{0}>() END", typeof(T));
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Returns the DefaultInstance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDefaultInstance<T>() where T : ComponentClassHelper
        {
            Type typeofT = typeof(T);
            if (deflist_handles == null) deflist_handles = new Dictionary<Type, object>();
            Monitor.Enter(this);
            Trace.TraceInformation("ComponentServerFactory::GetDefaultInstance<{0}>", typeof(T));
            InitHelper<T>();
            T res = null;
            try
            {
                if (deflist_handles.ContainsKey(typeofT))
                {
                    return (T)deflist_handles[typeofT];
                }
                else
                {
                    res = helper.GetDefaultInstance<T>();
                    deflist_handles.Add(typeofT, res);
                }

            }
            finally
            {
                Trace.TraceInformation("ComponentServerFactory::GetDefaultInstance<{0}>() END", typeof(T));
                Monitor.Exit(this);
            }
            return res;
        }

        /// <summary>
        /// Destructor for the class Factory
        /// </summary>
        ~ComponentServerFactory()
        {
            Trace.TraceInformation("~ComponentServerFactory");
            if (deflist_handles != null)
            {
                Trace.TraceInformation("Default instances {0}", deflist_handles.Count);
                try
                {
                    foreach (MarshalByRefObject mbro in deflist_handles.Values)
                    {
                        Trace.TraceInformation("Default instance {0}", mbro);
                        DisposeInstanceByUri(System.Runtime.Remoting.RemotingServices.GetObjectUri(mbro));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("Error {0} {1}", ex.Message, ex.StackTrace);
                }
            }
            Trace.TraceInformation("Other instances {0}", managedObjects.Count);
            try
            {
                foreach (WeakReference weakref in managedObjects.Keys)
                {
                    Trace.TraceInformation("Other instances {0}", weakref.Target);
                    this.DisposeInstanceByUri(managedObjects[weakref]);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Error {0} {1}", ex.Message, ex.StackTrace);
            }
            Trace.TraceInformation("~ComponentServerFactory END");
        }


        /// <summary>
        /// This method is used to release remoting references. When an instance is created thru AppDomains\Process
        /// a background thread tracks all references and when references are garbage collected, they are released
        /// identified by the MarshalByRef uri
        /// </summary>
        /// <param name="uri"></param>
        public void DisposeInstanceByUri(string uri)
        {
            if (helper != null)
            {
                helper.DisposeInstanceByUri(uri);
            }
        }
        #endregion

        private static void SetStandAloneFlag(Type factoryType, bool value)
        {
            try
            {
                FieldInfo field = GetStandAloneField(factoryType);
                field.SetValue(null, value);
            }
            catch
            {
                throw new Exception("The factory must have a public boolean field called StandAloneExecution.");
            }
        }

        private static FieldInfo GetStandAloneField(Type factoryType)
        {
            FieldInfo field = factoryType.GetField("StandAloneExecution", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
                throw new Exception("Field not found. The factory must have a public boolean field called StandAloneExecution.");
            return field;
        }

        internal static bool GetStandAloneFlag(Type factoryType)
        {
            try
            {
                return (bool)GetStandAloneField(factoryType).GetValue(null);
            }
            catch
            {
                throw new Exception("The factory must have a public boolean field called StandAloneExecution.");
            }

        }


        /// <summary>
        /// Register delegates used for CompenentServer implementations that do not use domains or processes
        /// </summary>
        /// <param name="del"></param>
        public void RegisterInitGlobalVarsDelegate(InitGlobalVarsDelegate del)
        {
            if (ImplementationType == ComponentServerImplementationType.NoDomains)
            {
                if (helper == null)
                    helper = ComponentServerHelperAppDomainImplementation.GetInstance(this, false);
                ComponentServerHelperAppDomainImplementation appDomainsHelper = helper as ComponentServerHelperAppDomainImplementation;
                appDomainsHelper.RegisterInitGlobalVarsDelegate(del);
            }
        }

        /// <summary>
        /// This function must be used in the main of projects that used ActiveX DLL or EXE projects.
        /// This function will open an unique client channel that is required for events, or callbacks.
        /// </summary>
        public static void RegisterClientIPCChannel()
        {
            string channelName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + "_" + System.Guid.NewGuid().ToString();
            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
            System.Collections.IDictionary properties = new System.Collections.Hashtable();
            properties["name"] = "ipc";
            properties["priority"] = "20";
            properties["portName"] = channelName;
            IpcChannel chnl = new IpcChannel(properties, clientProv, serverProv);
            ChannelServices.RegisterChannel(chnl, false);
        }

        /// <summary>
        /// Helper method used to get the username and domain for a process
        /// </summary>
        /// <param name="PID">process id</param>
        /// <param name="User">this parameter will hold the current user name</param>
        /// <param name="Domain">this parameter will hold the domain for the current user name</param>
        /// <returns></returns>
        internal static string GetProcessInfoByPID(int PID, out string User, out string Domain)
        {
            User = String.Empty;
            Domain = String.Empty;
            string OwnerSID = String.Empty;
            string processname = String.Empty;
            try
            {
                ObjectQuery sq = new ObjectQuery
                    ("Select * from Win32_Process Where ProcessID = '" + PID + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
                if (searcher.Get().Count == 0)
                    return OwnerSID;
                foreach (ManagementObject oReturn in searcher.Get())
                {
                    string[] o = new String[2];
                    //Invoke the method and populate the o var with the user name and domain
                    oReturn.InvokeMethod("GetOwner", (object[])o);

                    //int pid = (int)oReturn["ProcessID"];
                    processname = (string)oReturn["Name"];
                    //dr[2] = oReturn["Description"];
                    User = o[0];
                    if (User == null)
                        User = String.Empty;
                    Domain = o[1];
                    if (Domain == null)
                        Domain = String.Empty;
                    string[] sid = new String[1];
                    oReturn.InvokeMethod("GetOwnerSid", (object[])sid);
                    OwnerSID = sid[0];
                    return OwnerSID;
                }
            }
            catch
            {
                return OwnerSID;
            }
            return OwnerSID;
        }

        internal static string GetProcessUserName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("\\", "@");
        }

        internal static IChannel channel = null;
        internal static String syncToken = String.Empty;
        internal static bool startupFinished = false;
        internal static Guid portGuid = Guid.Empty;
        /// <summary>
        /// This is generic main that is used for all ActiveX Exe projects. It contains the logic to 
        /// setup the comunication channels, and client yncronization
        /// </summary>
        /// <param name="factoryType"></param>
        /// <param name="args"></param>
        public static void ServerMain(Type factoryType, string[] args)
        {
#if TRACE
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(string.Format(@"C:\{0}.log", factoryType.Name)));
#endif


            String ProcessName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            Process currentProcess = Process.GetCurrentProcess();
            Trace.TraceInformation("Process {0} PID:{1} started", ProcessName, currentProcess.Id);

            //Default value for channel name is ProcessName
            String channelName = String.Empty;

            //Processing arguments
            int current_arg_index = 0;
            while (current_arg_index < args.Length)
            {
                switch (args[current_arg_index].ToUpper())
                {
                    case "/CHANNEL":
                        if (current_arg_index + 1 < args.Length)
                        {
                            current_arg_index++;
                            channelName = args[current_arg_index];
                        }
                        break;
                    case "/STANDALONE":
                        SetStandAloneFlag(factoryType, true);
                        break;

                }
                current_arg_index++;
            }
            if (args.Length == 0)
            {
                SetStandAloneFlag(factoryType, true);
            }
            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
            bool isDefaultServer = false;
            if (channelName == String.Empty)
            {
                //We must add the username and domain
                channelName = GetProcessUserName();
                isDefaultServer = true;
            }
            else
                portGuid = new Guid(channelName);
            string serverName = ProcessName;
            string portName = ProcessName + channelName;

            int semValue = 0;
            if (isDefaultServer)
            {

                semValue = ProcessManager.GetSemaphoreValue(portName, true);
                semValue = semValue - 1; //To get the value used to start default process
                Trace.TraceInformation("ServerMain. Semaphore value: {0}", semValue);
                portName += semValue;
            }
            System.Collections.IDictionary properties = new System.Collections.Hashtable();
            properties["name"] = "ipc";
            properties["priority"] = "20";
            properties["portName"] = portName;
            properties["exclusiveAddressUse"] = false;
            channel = new IpcChannel(properties, clientProv, serverProv);
            ChannelServices.RegisterChannel(channel, false);


            // factoryType = typeof(ComponentServerHelperProcessImplementation);
            RemotingConfiguration.RegisterWellKnownServiceType(factoryType, "FactoryURI", WellKnownObjectMode.Singleton);
            Trace.TraceInformation(@"Attending at uri:\\{0}\FactoryURI", portName);
            String handleName = ProcessName + channelName;
            if (isDefaultServer)
                handleName += semValue;
            syncToken = handleName;
        }

        /// <summary>
        /// This method is added in the Main method for an activeX Exe
        /// This method sets a sync event that is used to make sure that a client will not a make
        /// a call to the server when it hasn't finish initializing
        /// </summary>
        /// <param name="factoryType"></param>
        public static void FinishStartup(Type factoryType)
        {

            if (!GetStandAloneFlag(factoryType))
            {
                EventWaitHandle ewh = null;
                try
                {

                    Trace.TraceInformation("Syncronizing handle: " + syncToken);
                    try
                    {
                        ewh = EventWaitHandle.OpenExisting(syncToken);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Failed " + ex.Message + ex.StackTrace);
                        throw new Exception("Syncronization error starting server process");
                    }
                }
                finally
                {
                    if (ewh != null)
                    {
                        ewh.Set();
                        ewh.Close();
                    }
                }
            }
            Trace.TraceInformation("Syncronization successful!");
            startupFinished = true;
        }











        #region IComponentServerHelper Members


        int IComponentServerHelper.InstancesCount
        {
            get 
            {
                if (helper == null)
                    return 0;
                else
                    return helper.InstancesCount;
            
            }
        }

        #endregion
    }

    /// <summary>
    /// When the user request to create a class that is inside an ActiveX project, it is necesary to perform several steps:
    /// 1. Determine in which EXE is the class located
    /// 2. Determine if the .EXE is already running
    /// 3. Start a new process if it is necessary.
    /// This class encapsultes all this logic
    /// </summary>
    internal class ProcessManager
    {
        static Dictionary<String, String> typeToAssembly = new Dictionary<string, string>();
        static bool initialized = false;

        /// <summary>
        /// Loads all .EXE in found in the search path and setups some metadata needed to
        /// determine the .EXE file that must be started as a new server
        /// The search path can be specified in the App.Config file.
        /// just add an entry like:
        /// <code>
        /// <configuration>
        ///   <appSettings>
        ///     <add key="PathForComponents" value="C:\TestInfo"/>
        ///   </appSettings>
        /// </configuration>
        /// </code>
        /// </summary>
        private static void LoadAllComponentInfo()
        {
            if (!initialized)
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(CurrentDomain_ReflectionOnlyAssemblyResolve);
                string pathForComponents = ConfigurationManager.AppSettings["PathForComponents"];
                if (pathForComponents == null)
                {
                    pathForComponents = Directory.GetCurrentDirectory();
                }
                foreach (string pathToComponentEXE in Directory.GetFiles(pathForComponents, "*.exe"))
                {
                    try
                    {
                        Assembly assemblyFile = Assembly.ReflectionOnlyLoadFrom(pathToComponentEXE);

                        foreach (Type type in assemblyFile.GetExportedTypes())
                        {
                            if (!typeToAssembly.ContainsKey(type.FullName))
                                typeToAssembly.Add(type.FullName, pathToComponentEXE);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write("Exception" + ex.Message + " " + ex.StackTrace);
                    }
                }
                initialized = true;
            }
        }

        /// <summary>
        /// We must make sure that the Manager is able to load all required dependencies. 
        /// As an optimization, only reflection information is loaded. While the reflection information is loaded it
        /// might happen that some assemblies are not loaded. This method provides some simple aid to find reflection 
        /// information that was not available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            String directory = Directory.GetCurrentDirectory();
            AssemblyName name = new AssemblyName(args.Name);
            Assembly result = null;
            try
            {
                result = Assembly.ReflectionOnlyLoad(name.FullName);
            }
            catch { }
            if (result == null)
            {
                try
                {
                    result = Assembly.ReflectionOnlyLoadFrom(directory + "\\" + name.Name + ".dll");
                }
                catch { }
            }
            if (result == null)
            {
                try
                {

                    result = Assembly.ReflectionOnlyLoadFrom(directory + "\\" + name.Name + ".exe");
                }
                catch { }
            }
            return result;
        }



        /// <summary>
        /// Obtains the factory to instanciate classes migrated from an ActiveX-Dll or an ActiveX-Exe.
        /// This method will locate the .exe file that contains those classes.
        /// </summary>
        /// <typeparam name="T">A class is needed to determine which assembly contains it and start the new process</typeparam>
        /// <returns>A reference to the factory that can instanciate classes of T type</returns>
        internal static IComponentServerHelper GetFactory<T>()
        {
            Trace.TraceInformation("ProcessManager::GetFactory<{0}>()", typeof(T));
            Trace.Indent();
            string pathForComponentEXE;
            System.IO.FileInfo finfo;
            LocateEXEForType<T>(out pathForComponentEXE, out finfo);
            //@TODO The extention must be checked. For a Process implementation files must be .EXE files for
            //an AppDomain implementation classes must be loaded and instantiated in another Domain
            //Let's check if the process is up
            bool loaded = false;
            string processName = finfo.Name.Replace(finfo.Extension, "");
            String domainAndUser = ComponentServerFactory.GetProcessUserName();
            foreach (Process p in Process.GetProcessesByName(processName))
            {
                string user = String.Empty;
                string domain = String.Empty;
                ComponentServerFactory.GetProcessInfoByPID(p.Id,out user,out domain);

                string processDomainAndUser = domain + "@" + user;
                if (string.Compare(domainAndUser, processDomainAndUser, true) == 0)
                {
                    loaded = true;
                    break;
                }
            }
#if DEBUG
            if (!loaded)
            {
                //check debug version
                loaded = Process.GetProcessesByName(processName + ".vshost").Length > 0;
            }
#endif
            Process theProcess = null;
            Guid newProcessGuid = Guid.NewGuid();
            String postFix = newProcessGuid.ToString();
            String default_namedpipe = processName + domainAndUser;
            String default_namedpipeCurrent = default_namedpipe;
            int current = -1;
            if (!loaded) //To generate a new UNIQUE named pipe
            {
                current = GetSemaphoreValue(default_namedpipe, false);
                default_namedpipeCurrent = default_namedpipe + current;
            }
            else
            {
                current = GetSemaphoreValue(default_namedpipe, true);
                current--;
                default_namedpipeCurrent = default_namedpipe + current;
            }
            if (!loaded)
            {
                theProcess = StartProcess(pathForComponentEXE, processName, current, "/ComponentServer"); //,"/Channel", "##" + default_namedpipeCurrent
            }
            IComponentServerHelper res = null;
            //Now we must validate the default NamedPipe
            String uri = String.Format("ipc://{0}/FactoryURI", default_namedpipeCurrent);
            Trace.TraceInformation("ProcessManager::GetFactory<{0}> URI:{1}", typeof(T), uri);
            res = (IComponentServerHelper)Activator.GetObject(typeof(IComponentServerHelper), uri);
            if (res == null)
                throw new Exception("Could not connect to default project factory");
            Trace.Unindent();
            Trace.TraceInformation("ProcessManager::GetFactory<{0}>() END", typeof(T));
            return res;
        }
        static Semaphore sem = null;
        const int MAX_SEMAPHORE_VALUE = 100000;
        const int MIN_SEMAPHORE_VALUE = 100;
        /// <summary>
        /// Semaphores are used to syncronize processes and have a deterministed way of
        /// getting a named pipe name.
        /// This is mostly due to limitatios on the Remoting.
        /// </summary>
        /// <param name="default_namedpipe">Name for the semaphore</param>
        /// <param name="doNotIncrement">Indicates that you want to get the semaphone name but you dont want to modify it</param>
        /// <returns></returns>
        internal static int GetSemaphoreValue(String default_namedpipe, bool doNotIncrement)
        {
            try
            {
                bool created = false;
                sem = new Semaphore(MIN_SEMAPHORE_VALUE, MAX_SEMAPHORE_VALUE, default_namedpipe, out created);

            }
            catch (WaitHandleCannotBeOpenedException)
            {
                throw new Exception("Syncronization token for server process cannot be obtained!");
            }
            int current = sem.Release();
            if (doNotIncrement)
            {
                sem.WaitOne();
                return current;
            }
            if (current > MAX_SEMAPHORE_VALUE)
            {
                //we just need to reset semaphore values
                int distance = (MAX_SEMAPHORE_VALUE - MIN_SEMAPHORE_VALUE) + 2;
                for (int i = 0; i < distance; i++)
                {
                    sem.WaitOne();
                }
                current = sem.Release();
            }

            return current;
        }


        internal static void LocateEXEForType<T>(out string pathForComponentEXE, out System.IO.FileInfo finfo)
        {
            Type desiredType = typeof(T);
            LoadAllComponentInfo();
            if (!typeToAssembly.ContainsKey(desiredType.FullName))
            {
                throw new Exception(String.Format("Type {0} not found in loaded assemblies from {1}", desiredType.FullName, Directory.GetCurrentDirectory()));
            }
            //First locate the assembly containing the class
            pathForComponentEXE = typeToAssembly[desiredType.FullName];

            finfo = new FileInfo(pathForComponentEXE);
        }


        /// <summary>
        /// Starts the process and makes sure that it is syncronizeds
        /// </summary>
        /// <param name="pathForComponentEXE">Path for the .EXE file that will be started</param>
        /// <param name="processName">The process name is used for syncronization</param>
        /// <param name="semValue"> When a process is created a semaphore is used for syncronization issues between server and client. The semValue is then used to create a syncEvent</param>
        /// <param name="arguments"> Arguments to be passed to start the process</param>
        /// <returns>The Process that has just been started</returns>
        internal static Process StartProcess(string pathForComponentEXE, string processName, int semValue, params string[] arguments)
        {
            Trace.TraceInformation("ProcessManager::StartProcess {0}", processName);
            Trace.Indent();
            EventWaitHandle ewh = null;
            try
            {
                //Create Process
                Process p = new Process();
                ProcessStartInfo info = new ProcessStartInfo(pathForComponentEXE);

                p.StartInfo = info;
                String channelName = String.Empty;

                //Processing arguments we just need the channel name
                StringBuilder argumentsStr = new StringBuilder();
                int current_arg_index = 0;
                foreach (String arg_str in arguments)
                {
                    argumentsStr.Append(arg_str);
                    argumentsStr.Append(" ");
                }
                info.Arguments = argumentsStr.ToString();
                while (current_arg_index < arguments.Length)
                {
                    if (arguments[current_arg_index].ToUpper() == "/CHANNEL")
                    {
                        if (current_arg_index + 1 < arguments.Length)
                        {
                            current_arg_index++;
                            channelName = arguments[current_arg_index];
                            break;
                        }
                    }

                    current_arg_index++;
                }
                Trace.TraceInformation("ProcessManager::StartProcess channelName {0}", channelName);
                bool isDefaultServer = false;
                if (channelName == String.Empty)
                {
                    channelName = ComponentServerFactory.GetProcessUserName();
                    isDefaultServer = true;

                }


                String sync_handleName = processName + channelName;
                if (isDefaultServer)
                    sync_handleName += semValue;
                Trace.TraceInformation("ProcessManager::StartProcess sync with {0}", sync_handleName);

                ewh = new EventWaitHandle(false, EventResetMode.AutoReset, sync_handleName);
                if (!p.Start())
                    throw new Exception("Process could not be started!");
                //SYNCRONIZATION 
                //We must be sure that the process is up
                ewh.WaitOne();  //wait for the process t notify
                return p;
            }
            catch (Exception ex)
            {
                throw new Exception("The process could not be started", ex);
            }
            finally
            {
                if (ewh != null)
                {
                    ewh.Close();
                }
                Trace.Unindent();
                Trace.TraceInformation("StartProcess END");
            }

        }

    }



    /// <summary>
    /// This is a base implementation for supporting the VB6 behaviour of classes contained inside ActiveX-Dll and ActiveX-exe projects.
    /// </summary>
    /// <typeparam name="X">This is the class that will be used to provide a .NET equivalent of process</typeparam>
    public abstract class ComponentServerHelperBase<X> : MarshalByRefObject, IComponentServerHelper
    {
        internal struct InstancesInfo
        {
            int[] sessions;
            object value;
            bool external;
            internal void AddSession(int sessionID)
            {
                if (this.sessions == null)
                    this.sessions = new int[] { sessionID };
                else
                {
                    int current_length = this.sessions.Length;
                    Array.Resize<int>(ref sessions, current_length + 1);
                    sessions[current_length] = sessionID;
                }
            }
            internal void RemoveSession(int sessionID)
            {
                if (sessions == null)
                    return;
                int index = Array.IndexOf<int>(sessions, sessionID);
                if (index != -1)
                {
                    int current_length = this.sessions.Length;
                    for (int i = index; i < current_length-1; i++)
                    {
                        sessions[i] = sessions[i + 1];
                    }
                    Array.Resize<int>(ref sessions, current_length -1);
                }
            }
            internal Type Type
            {
                get
                {
                    if (value != null)
                        return value.GetType();
                    else
                        return null;
                }
            }
            internal object Value
            {
                set
                {
                    this.value = value;
                }
                get
                {
                    return value;
                }

            }
            internal bool External
            {
                set
                {
                    this.external = value;
                }
                get
                {
                    return external;
                }

            }
            internal int[] Sessions
            {
                get
                {
                    if (sessions == null)
                        return new int[] { };
                    else
                        return sessions;
                }
            }
        }

        internal class InstancesList : List<InstancesInfo>
        {
            public bool Contains(object instance)
            {
                foreach (InstancesInfo inf in this)
                {
                    if (inf.Value == instance)
                        return true;
                }
                return false;
            }
        }
        internal abstract class InstancesDictionary
        {
            int instance_count = 0;


            public int InstanceCount
            {
                get { return instance_count; }
                set { instance_count = value; }
            }
            /// <summary>
            /// Works like a references count, storing the all instances created for the component
            /// </summary>
            protected Dictionary<X, InstancesList> instances = null;
            
            public void Add(X process,int sessionID,object newInstance,bool isExternal)
            {
                if (!instances.ContainsKey(process))
                {
                    instances.Add(process, new InstancesList());
                }
                InstancesInfo info = new InstancesInfo();
                info.Value = newInstance;
                info.External = isExternal;
                info.AddSession(sessionID);
                instances[process].Add(info);
                instance_count++;
            }

            public void Remove(X process)
            {
                if (instances.ContainsKey(process))
                {
                    instances.Remove(process);
                }
            }
            public bool Remove(X process, object instance)
            {
                if (instances.ContainsKey(process))
                {
                    List<InstancesInfo> list = instances[process];
                    int count = list.Count;
                    for(int i=0;i<list.Count; i++)
                    {
                        InstancesInfo inf = list[i];
                        if (inf.Value == instance)
                        {
                            list.RemoveAt(i);
                            instance_count--;
                            break;
                        }
                        if (list.Count==0) //We do not need this process anymore
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public Dictionary<X, InstancesList>.KeyCollection Processes
            {
                get
                {
                    return instances.Keys;
                }
            }

            public IEnumerable<InstancesInfo> Instances
            {
                get
                {
                    foreach (InstancesList list in instances.Values)
                    {
                        foreach (InstancesInfo info in list)
                        {
                            yield return info;
                        }
                    }
                    yield break;
                }
            }

            public abstract bool AreTheSame(X left, X right);

            public InstancesList this[X aProcess]
            {
                get
                {
                    return instances[aProcess];
                }
            }

            public bool ContainsInstance(object instance)
            {
                X dummy;
                return ContainsInstance(instance, out dummy);
            }

            public bool ContainsInstance(object instance,out X proc)
            {
                proc = default(X);
                foreach (X process in Processes)
                {
                    bool res= instances[process].Contains(instance);
                    if (res)
                    {
                        proc = process;
                        return true;
                    }
                }
                return false;
            }


        }
        
        /// <summary>
        /// Makes the lifetime for this object unlimited
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        internal ComponentServerHelperBase(ComponentServerFactory factory)
        {
            factoryReference = factory;
        }

        internal void CheckFactory(ComponentServerFactory newFactory)
        {
            if (!factoryReference.Equals(newFactory))
                factoryReference = newFactory;
        }

        ComponentServerFactory factoryReference = null;
        internal ComponentServerFactory FactoryReference
        {
            get
            {
                return factoryReference;
            }
            set
            {
                factoryReference = value;
            }
        }

        /// <summary>
        /// Works like a references count, storing the all instances created for the component
        /// </summary>
        internal InstancesDictionary instances = null;

        /// <summary>
        /// Indicates if ComponentServerHelper works using Processs or just a simple ClassFactory
        /// </summary>
        internal bool useProcess = false;

        /// <summary>
        /// The first and Main Process or Domain loaded
        /// </summary>
        internal X mainProcess = default(X);
        /// <summary>
        /// The last Process or Domain loaded
        /// </summary>
        protected X curProcess = default(X);


        /// <summary>
        /// Creates an instance of T in Main Domain
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <returns>An instance of type T</returns>
        private T CreateInstanceInMainProcess<T>() where T : ComponentClassHelper
        {
            Type type = typeof(T);
            if (mainProcess == null)
                mainProcess = CreateNewProcess<T>();
            object res = CreateInstanceInProcess<T>(mainProcess);
            curProcess = mainProcess;
            return (T)res;
        }

        /// <summary>
        /// Searches a Domain without a specific class type (SingleUse/GlbSingleUse) where to create the type, else
        /// it is created in a new Domain
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <returns>An instance of type T</returns>
        private T CreateInstanceInAvailableProcess<T>() where T : ComponentClassHelper
        {
            X aProcess = GetProcessToCreate<T>();
            if (aProcess != null)
            {
                Type type = typeof(T);
                curProcess = aProcess;
                object res = CreateInstanceInProcess<T>(curProcess);
                return (T)res;
            }
            else
                return CreateInstanceInNewProcess<T>();
        }



        /// <summary>
        /// Creates an instance of T in a New Domain
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <returns>An instance of type T</returns>
        private T CreateInstanceInNewProcess<T>() where T : ComponentClassHelper
        {
            Type type = typeof(T);
            curProcess = CreateNewProcess<T>();
            return (T)CreateInstanceInProcess<T>(curProcess);
        }

        /// <summary>
        /// Creates instance "inside" the given "Process" space.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="curProcess"></param>
        /// <returns></returns>
        protected abstract object CreateInstanceInProcess<T>(X curProcess) where T : ComponentClassHelper;

        /// <summary>
        /// Creates a new process in which a new instance of type T could be create and returns the process object for manipulation
        /// </summary>
        /// <typeparam name="T">Type of instance that needs a new process</typeparam>
        /// <returns>A "Process" object</returns>
        protected abstract X CreateNewProcess<T>();

        /// <summary>
        /// Creates an instance of T in the addresses space of the main application
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <returns>An instance of type T</returns>
        internal T CreateInstanceNoProcess<T>() where T : ComponentClassHelper
        {
            Trace.TraceInformation("ComponentServerHelperBase::CreateInstanceNoProcess<{0}>", typeof(T));
            UpdateCurrentProcess();
            T res = FactoryReference.MakeNewInstance(typeof(T)) as T;
            Trace.TraceInformation("ComponentServerHelperBase::CreateInstanceNoProcess<{0}> END", typeof(T));
            return res;
        }

        /// <summary>
        /// Updates the curProcess variable holding a reference to the current AppDomain/Process
        /// </summary>
        protected abstract void UpdateCurrentProcess();

        /// <summary>
        /// Registers a InitGlobalVarsDelegate to be invoked when all component references are free.
        /// This method is invoked to each created instance just if UseDomain is false.
        /// </summary>
        /// <param name="del">The delegate to be registered</param>
        internal virtual void RegisterInitGlobalVarsDelegate(InitGlobalVarsDelegate del)
        {
            //Base case does nothing
        }


        /// <summary>
        /// Looks for a Process to create a specific (SingleUse/GlbSingleUse) type
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <returns>The Process where to create the type or null if all Process have the specific type</returns>
        /// 
        
        public X GetProcessToCreate<T>() where T : ComponentClassHelper
        {

            Type type = typeof(T);
            Trace.TraceInformation("Looking for process to create instance of type {0}", type);
            string baseName = type.BaseType.Name;
            foreach (X aProcess in instances.Processes)
            {
                bool available = true;
                foreach (InstancesInfo inst in instances[aProcess])
                {
                    Type instType = inst.Value.GetType();
                    if (instType.BaseType.Name == baseName)
                    {
                        available = false;
                        break;
                    }
                }
                if (available)
                {
                    Trace.TraceInformation("Process found {0}", GetProcessID(aProcess));
                    return aProcess;
                }
            }
            Trace.TraceInformation("No available process found");
            return default(X);
        }

        /// <summary>
        /// Finds an instance in ComponentServer and returns it
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="instance">The instance to be found</param>
        /// <returns>The instance if it was found or a new instance of type T</returns>
        public T FindInstance<T>(T instance) where T : ComponentClassHelper
        {
            if (InstanceIsCreated(instance))
                return instance;
            else
                return CreateInstance<T>();
        }


        /// <summary>
        /// Looks for an instance in instances Dictionary
        /// </summary>
        /// <param name="instance">The instance to be found</param>
        /// <returns>True if instance is found</returns>
        private bool InstanceIsCreated(object instance)
        {
            X theProcess;
            return InstanceIsCreated(instance, out theProcess);
        }

        /// <summary>
        /// Looks for an instance in instances Dictionary
        /// </summary>
        /// <param name="instance">The instance to be found</param>
        /// <param name="theProcess">The process where the instance was found</param>
        /// <returns>True if instance is found</returns>
        private bool InstanceIsCreated(object instance, out X theProcess)
        {
            theProcess = default(X);
            foreach (X aProcess in instances.Processes)
            {
                if (instances[aProcess].Contains(instance))
                {
                    theProcess = aProcess;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds the instance in the instances Dictionary related to its corresponding Process
        /// </summary>
        /// <param name="aProcess">The Process where the instance was created</param>
        /// <param name="instance">The instance to be added</param>
        /// <param name="isExternal">Indicates if instances is being referenced externally</param>
        internal virtual void AddInstance(X aProcess, object instance, bool isExternal)
        {
            Trace.TraceInformation("ComponentServerHelperBase::AddInstance({0},{1})", GetProcessID(aProcess), instance);
            instances.Add(aProcess, GetProcessID(aProcess), instance, isExternal);
            Trace.TraceInformation("Total instances for {0}:{1}", GetProcessID(aProcess), instances[aProcess].Count);
        }

        internal abstract int GetProcessID(X aProcess);

        internal virtual bool AreTheSameProcess(X aProcess1, X aProcess2)
        {
            return aProcess1.Equals(aProcess2);
        }

        /// <summary>
        /// Removes the instance from the instances Dictionary
        /// </summary>
        /// <param name="instance">The instance to be removed</param>
        private void RemoveInstance(object instance)
        {
            Trace.TraceInformation("ComponentServerHelperBase::RemoveInstance({0})", instance);
            X aProcess;

            if (instances.ContainsInstance(instance, out aProcess))
                {
                    //Removes the instance from the instances list for this process
                    X currentProcess = GetCurrentProcess();    
                    bool no_more_instances = instances.Remove(aProcess,instance);
                    Trace.TraceInformation("Instance removed from  Process {0}. Remaining instances {1}.", GetProcessID(aProcess), instances[aProcess].Count);
                    
                    if (!AreTheSameProcess(currentProcess, aProcess))
                    {
                        RemoveInstanceFromProcess(instance, aProcess);
                        //It the remote process has no more instances just get rid of it
                        if (no_more_instances)
                        {
                            instances.Remove(aProcess);
                            CleanChannelsInfo(aProcess);
                        }
                    }
                    else
                        Trace.TraceInformation("RemoveInstance. Instance was local");
                        

                }
            
        }

        /// <summary>
        /// @todo
        /// </summary>
        /// <param name="aProcess"></param>
        internal virtual void CleanChannelsInfo(X aProcess)
        {

        }

        /// <summary>
        /// @todo
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="aProcess"></param>
        protected abstract void RemoveInstanceFromProcess(object instance, X aProcess);

        #region IComponentServerHelper Members

        Dictionary<Type, object> default_instances = null;

        /// <summary>
        /// Global classes in VB6 are very similar to a default instance concept.
        /// So everytime the original code had a default instance it is change for a call to this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDefaultInstance<T>() where T : ComponentClassHelper
        {
            if (default_instances == null)
                default_instances = new Dictionary<Type, object>();
            Type typeofT = typeof(T);
            if (!default_instances.ContainsKey(typeofT))
            {
                default_instances.Add(typeofT, CreateInstance<T>());
            }
            return (T)default_instances[typeofT];
        }

        /// <summary>
        /// Creates an instance of T in the corresponding space of memory or Process
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <returns>An instance of type T</returns>
        public T CreateInstance<T>() where T : ComponentClassHelper
        {
            return CreateInstance<T>(null);
        }

        /// <summary>
        /// Creates an instance of T in the corresponding space of memory or Process
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="oldInstance">The instance being freed if it had a referenced instance</param>
        /// <param name="isExternal">Indicates if this instance will be referenced externally</param>
        /// <returns>An instance of type T</returns>
        public T CreateInstance<T>(object oldInstance, bool isExternal) where T : ComponentClassHelper
        {
            Type type = typeof(T);
            Trace.TraceInformation("ComponentServerHelperBase::CreateInstance<{0}>({1})", type, oldInstance);
            T newInstance = null;
            if (useProcess)
            {
                // VB6 SingleUse classes are created in a different Domain (component server)
                if ((type.BaseType.Name == "ComponentSingleUseClassHelper") ||
                    (type.BaseType.Name == "GlbComponentSingleUseClassHelper"))
                {
                    Trace.TraceInformation("In available process");
                    newInstance = CreateInstanceInAvailableProcess<T>();
                }
                else
                {
                    Trace.TraceInformation("In Main process");
                    newInstance = CreateInstanceInMainProcess<T>();
                }
            }
            else
            {
                Trace.TraceInformation("No process");
                newInstance = CreateInstanceNoProcess<T>();
            }
            newInstance.RegisterInitGlobalVarsDelegates();
            //curProcess holds a reference that is used to perform the AddInstance
            //and make the association to the respective(AppDomain/Process) 
            AddInstance(curProcess, newInstance, isExternal);
            if (oldInstance != null && InstanceIsCreated(oldInstance)) RemoveInstance(oldInstance);
            Trace.TraceInformation("ComponentServerHelperBase::CreateInstance returns {0}", newInstance);
            return newInstance;
        }

        /// <summary>
        /// Creates an instance of T in the corresponding space of memory or Process
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="oldInstance">The instance being freed if it had a referenced instance</param>
        /// <returns>An instance of type T</returns>
        public T CreateInstance<T>(object oldInstance) where T : ComponentClassHelper
        {
            return CreateInstance<T>(oldInstance, true);
        }

        /// <summary>
        /// Frees a component instance and checks if Domain/GlobalVars should be initialized
        /// </summary>
        /// <param name="instance">The instance to be freed</param>
        public virtual int DisposeInstance<T>(T instance) where T : ComponentClassHelper
        {
            Trace.TraceInformation("ComponentServerHelperBase::DisposeInstance<{0}>({1}) {2}", typeof(T), instance, useProcess);
            X theProcess;
            if (InstanceIsCreated(instance, out theProcess))
            {
                IDisposable disposableObj = instance as IDisposable;
                if (disposableObj != null)
                    disposableObj.Dispose();
                RemoveInstance(instance);
            }
            //CheckServerStatus();
            if ((GetInstancesCount(theProcess) == 0) || (!AreThereExternalInstances(theProcess)))
            {
                DestroyProcess(theProcess);
                return (int)ComponentServerCodes.CS_REMOVE_FACTORY;
            }
            return (int)ComponentServerCodes.CS_OK;
        }

        internal int GetInstancesCount(X theProcess)
        {
            return instances[theProcess].Count;
        }

        internal bool AreThereExternalInstances(X theProcess)
        {
            foreach (InstancesInfo inst in instances[theProcess])
            {
                if (inst.External) return true;
            }
            return false;
        }


        /// <summary>
        /// Checks if this server is still needed, if not, 
        /// this process is marked for elimination
        /// </summary>
        private int CheckServerStatus()
        {
            if (useProcess)
            {
                Trace.TraceInformation("Server status instances[{0}]", instances.InstanceCount);
                foreach (X proc in instances.Processes)
                {
                    Trace.TraceInformation(" {0} - instances {1}", GetProcessID(proc), instances[proc].Count);
                }
                if (instances.Processes.Count == 1)
                {
                    Trace.TraceInformation("References in process {0}", instances.InstanceCount);
                    if (instances.InstanceCount == 0)
                    {
                        Trace.TraceInformation("This server is no longer needed. Shutting down");
                        //mainProcess = GetCurrentProcess();
                        return (int)ComponentServerCodes.CS_REMOVE_FACTORY;
                    }
                }
            }
            else
                InitializeGlobalVarsIfNeeded();
            return (int)ComponentServerCodes.CS_OK;
        }

        /// <summary>
        /// @todo
        /// </summary>
        /// <param name="uri"></param>
        public void DisposeInstanceByUri(String uri)
        {
            if (instances.InstanceCount == 0)
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("This component server has no instances while trying to remove instance by uri {0}", uri));
                return;
            }
            System.Diagnostics.Trace.TraceInformation("Trying to remove instance by uri {0}", uri);
            object instanceToBeRemoved = FindInstanceByRemotingUri(uri);
            if (instanceToBeRemoved != null)
            {
                RemoveInstance(instanceToBeRemoved);
                System.Diagnostics.Trace.TraceInformation("Instance {0} found and removed", uri);
                CheckServerStatus();
            }
        }

        private object FindInstanceByRemotingUri(String uri)
        {
                foreach (object objRef in instances.Instances)
                {
                    MarshalByRefObject mbroRef = objRef as MarshalByRefObject;
                    if (mbroRef != null)
                    {
                        String mbroRefUri = RemotingServices.GetObjectUri(mbroRef);
                        if (string.Compare(mbroRefUri, uri, true) == 0)
                        {
                            return objRef;
                        }
                    }
                }
            return null;
        }

        /// <summary>
        /// Kills a "Process" object
        /// </summary>
        /// <param name="aProcess">The "process" that will be destroyed</param>
        protected abstract void DestroyProcess(X aProcess);

        /// <summary>
        /// Returns the "Process" object where the code is currently running
        /// </summary>
        /// <returns></returns>
        protected abstract X GetCurrentProcess();

        /// <summary>
        /// One of the possible implementations for ActiveX-Dll and ActiveX-Exe can be to migrate the classes
        /// as close as posible to standard .NET classes. However the instancing behaviour of VB6 requires that when all instances
        /// of classes of an activex-exe are removed that global vars have to be reset to their original values.
        /// To reproduce that behaviour, a list of delegates can be kept to reset those variables.
        /// The default implementation does nothing of this.
        /// </summary>
        protected void InitializeGlobalVarsIfNeeded()
        {
            //Default does nothing
        }

        MakeNewInstanceDelegate makeNewInstance = null;
        /// <summary>
        /// @TODO @remove
        /// </summary>
        /// <param name="theDelegate"></param>
        public void RegisterFactoryDelegate(MakeNewInstanceDelegate theDelegate)
        {
            makeNewInstance = theDelegate;
        }


        #endregion


        #region IComponentServerHelper Members


        int IComponentServerHelper.InstancesCount
        {
            get { return instances.InstanceCount; }
        }

        #endregion
    }

    /// <summary>
    /// This implementation extends the ComponentServerHelper using System.Diagnostics.Process to represent the "Process" concept.
    /// </summary>
    public class ComponentServerHelperProcessImplementation : ComponentServerHelperBase<System.Diagnostics.Process>
    {
        ProcessManager manager;

        /// <summary>
        /// Keeps a map of the process Name + process.Id to the Guid that 
        /// identifies the IPC port to comunicate with the remoting process
        /// </summary>
        Dictionary<String, Guid> portForProcess = new Dictionary<String, Guid>();

        

        class ProcessInstanceDictionary : ComponentServerHelperBase<Process>.InstancesDictionary
        {

            public ProcessInstanceDictionary()
            {
                base.instances = new Dictionary<Process, InstancesList>(new ProcessComparer());
                instances.Add(Process.GetCurrentProcess(), new InstancesList());

            }
            public override bool AreTheSame(Process left, Process right)
            {
                return left.Id == right.Id;
            }

            class ProcessComparer : IEqualityComparer<Process>
            {
                #region IEqualityComparer<Process> Members

                public bool Equals(Process x, Process y)
                {
                    return x.Id == y.Id;
                }

                public int GetHashCode(Process obj)
                {
                    return obj.Id;
                }

                #endregion
            }
        }

        /// <summary>
        /// Default contructor. Initializes several data tables, like instances, remoting ports, main and current process.
        /// </summary>
        public ComponentServerHelperProcessImplementation(ComponentServerFactory factory)
            : base(factory)
        {
            useProcess = true;
            //to include current process
            Process currentProcess = GetCurrentProcess();
            instances = new ProcessInstanceDictionary();
            portForProcess.Add(currentProcess.ProcessName + currentProcess.Id, ComponentServerFactory.portGuid);
            mainProcess = GetCurrentProcess();
        }

        internal Dictionary<ComponentClassHelper, ClientSponsor> sponsors = new Dictionary<ComponentClassHelper, ClientSponsor>();

        internal override void AddInstance(Process aProcess, object instance, bool isExternal)
        {
            base.AddInstance(aProcess, instance, isExternal);
            lock (sponsors)
            {
                sponsors.Add(instance as ComponentClassHelper, this.FactoryReference.Sponsor);
            }
        }

        /// <summary>
        /// Dispose an instance
        /// </summary>
        /// <typeparam name="T">Type of the instance to dispose</typeparam>
        /// <param name="instance">Instance to dispose</param>
        /// <returns></returns>
        public override int DisposeInstance<T>(T instance)
        {
            int result = base.DisposeInstance<T>(instance);
            lock (sponsors)
            {
                sponsors.Remove(instance as ComponentClassHelper);
            }
            return result;
        }
        
        internal override bool AreTheSameProcess(Process aProcess1, Process aProcess2)
        {
            return aProcess1.Id == aProcess2.Id;
        }

        internal override int GetProcessID(Process aProcess)
        {
            return aProcess.Id;
        }

        /// <summary>
        /// Creates instance "inside" a Process
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="curProcess"></param>
        /// <returns></returns>
        protected override object CreateInstanceInProcess<T>(Process curProcess)
        {

            IComponentServerHelper factory = GetFactoryForProcess(curProcess);
            if (factory == null) //There is no factory means that we create the object in the current process
            {
                //This is the current process
                object local_instance = CreateInstanceNoProcess<T>();
                return local_instance;
            }
            else
            {
                return factory.CreateInstance<T>();
            }
        }
        /// <summary>
        /// Removes an instance from an specific process
        /// </summary>
        /// <param name="instance">Instance to remove</param>
        /// <param name="aProcess">Process that we must contact to remove given instance</param>
        protected override void RemoveInstanceFromProcess(object instance, Process aProcess)
        {
            Trace.TraceInformation("ComponentServerHelperProcessImplementation::RemoveInstanceFromProcess({1},{0}", GetProcessID(aProcess), instance);
            IComponentServerHelper factory = GetFactoryForProcess(aProcess);
            //@todo check factory == null
            MarshalByRefObject mbroRef = instance as MarshalByRefObject;
            if (mbroRef != null)
            {
                if (factory != null)
                {
                    try
                    {
                        factory.DisposeInstanceByUri(RemotingServices.GetObjectUri(mbroRef));
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceInformation(ex.Message);
                    }
                }
            }
        }

        internal override void CleanChannelsInfo(Process aProcess)
        {
            Guid guid = Guid.Empty;
            string key = aProcess.ProcessName + aProcess.Id;
            bool found = portForProcess.TryGetValue(key, out guid);
            if (found)
            {
                portForProcess.Remove(key);
            }

        }





        IComponentServerHelper GetFactoryForProcess(Process curProcess)
        {
            Guid newGuid = Guid.Empty;
            String processName = curProcess.ProcessName;
            bool found = portForProcess.TryGetValue(processName + curProcess.Id, out newGuid);
            if (found)
            {
                String uri = String.Empty;
                if (newGuid == ComponentServerFactory.portGuid) //Is in same process
                {
                    return null;
                }
                else
                    uri = String.Format("ipc://{0}{1}/FactoryURI", processName, newGuid.ToString());
                Trace.TraceInformation("ComponentServerHelperProcessImplementation::GetFactoryProcess({0}) URI:{1}", curProcess.Id, uri);
                IComponentServerHelper res = (IComponentServerHelper)Activator.GetObject(typeof(IComponentServerHelper), uri);
                return res;
            }
            else
                throw new Exception("Comunication port for process could not be established");
        }


        /// <summary>
        /// Updates the curProcess variable holding a reference to the current Process
        /// </summary>
        protected override void UpdateCurrentProcess()
        {
            curProcess = Process.GetCurrentProcess();
        }



        private ProcessManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = new ProcessManager();
                }
                return manager;
            }
            set { manager = value; }
        }

        /// <summary>
        /// Creates a new process that will hold an instance of T type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected override Process CreateNewProcess<T>()
        {
            string pathForComponentEXE;
            System.IO.FileInfo finfo;
            ProcessManager.LocateEXEForType<T>(out pathForComponentEXE, out finfo);

            string processName = finfo.Name.Replace(finfo.Extension, "");
            Guid newGuid = Guid.NewGuid();
            String uri = String.Format("ipc://{0}/FactoryURI", newGuid.ToString());
            System.Diagnostics.Debug.WriteLine("Manager at " + uri);
            Process p = ProcessManager.StartProcess(pathForComponentEXE, processName, -1, "/Channel", newGuid.ToString(), "/ComponentServer");
            if (p != null)
            {
                portForProcess.Add(processName + p.Id, newGuid);
            }
            return p;


        }


        /// <summary>
        /// @TODO
        /// </summary>
        /// <param name="mainProcess"></param>
        protected override void DestroyProcess(Process mainProcess)
        {
            Trace.TraceInformation("Process {0} will be destroyed", mainProcess != null ? mainProcess.ProcessName : "unknown");
            Trace.Flush();
            Process curProc = GetCurrentProcess();
            if ((mainProcess != null) && ((! AreTheSameProcess(curProc, mainProcess)) || (instances.Processes.Count == 1)))
            {
                instances.Remove(mainProcess);
                mainProcess.Kill();
                Thread.Sleep(1000);
            }
            // Check if only the CurrentProcess is alive and it has none references
            if ((instances.Processes.Count == 1) && (GetInstancesCount(GetCurrentProcess()) == 0))
            {
                DestroyProcess(GetCurrentProcess());
            }
        }

        /// <summary>
        /// @TODO
        /// </summary>
        /// <returns></returns>
        protected override Process GetCurrentProcess()
        {
            return System.Diagnostics.Process.GetCurrentProcess();
        }

    }

    /// <summary>
    /// This implementation extends the ComponentServerHelper using AppDomain to represent the "Process" concept.
    /// </summary>
    public class ComponentServerHelperAppDomainImplementation : ComponentServerHelperBase<AppDomain>
    {
        class AppDomainInstanceDictionary : ComponentServerHelperBase<AppDomain>.InstancesDictionary
        {

            public AppDomainInstanceDictionary()
            {
                base.instances = new Dictionary<AppDomain, InstancesList>(new AppDomainComparer());
                instances.Add(AppDomain.CurrentDomain, new InstancesList());

            }
            public override bool AreTheSame(AppDomain left, AppDomain right)
            {
                return left.Id == right.Id;
            }

            class AppDomainComparer : IEqualityComparer<AppDomain>
            {
                #region IEqualityComparer<AppDomain> Members

                public bool Equals(AppDomain x, AppDomain y)
                {
                    return x.Id == y.Id;
                }

                public int GetHashCode(AppDomain obj)
                {
                    return obj.Id;
                }

                #endregion
            }
        }

        
        
        internal ComponentServerHelperAppDomainImplementation(ComponentServerFactory factory)
            : base(factory)
        {
            useProcess = true;
            //to include current process
            instances = new AppDomainInstanceDictionary();
            mainProcess = GetCurrentProcess();
            //portForProcess.Add(mainProcess + " " + mainProcess.Id, ComponentServerFactory.portGuid);
        }

        internal override int GetProcessID(AppDomain aDomain)
        {
            return aDomain.Id;
        }


        /// <summary>
        /// Updates local variable setting the current Domain
        /// </summary>
        protected override void UpdateCurrentProcess()
        {
            curProcess = AppDomain.CurrentDomain;
        }


        /// <summary>
        /// Contains the InitGlobalVarsDelegate's invoked when all references of this component were freed
        /// </summary>
        private List<InitGlobalVarsDelegate> initGlobalVarsDels = new List<InitGlobalVarsDelegate>();

        /// <summary>
        /// Overrides base method providing an implementation that will reset global vars when needed.
        /// </summary>
        /// <typeparam name="AppDomain"></typeparam>
        protected void InitializeGlobalVarsIfNeeded<AppDomain>()
        {
            foreach (InitGlobalVarsDelegate del in initGlobalVarsDels)
            {
                del.Invoke();
            }
        }

        /// <summary>
        /// Registers a InitGlobalVarsDelegate to be invoked when all component references are freed.
        /// This method is invoked to each created instance just if UseDomain is false.
        /// </summary>
        /// <param name="del">The delegate to be registered</param>
        internal override void RegisterInitGlobalVarsDelegate(InitGlobalVarsDelegate del)
        {
            if (!initGlobalVarsDels.Contains(del))
                initGlobalVarsDels.Add(del);
        }


        /// <summary>
        /// Creates instance in the given domain and returns the unWrapped reference
        /// </summary>
        /// <param name="curDomain"></param>
        /// <returns></returns>
        protected override object CreateInstanceInProcess<T>(AppDomain curDomain)
        {

            Type type = typeof(T);
            Type FactoryType = this.FactoryReference.GetType();
            if (curDomain == AppDomain.CurrentDomain)
            {
                return FactoryReference.MakeNewInstance(type);
            }
            else
            {
                ObjectHandle oh_helper = curDomain.CreateInstance(FactoryType.Assembly.FullName, FactoryType.FullName);
                IComponentServerHelper factory = (IComponentServerHelper)oh_helper.Unwrap();
                return factory.CreateInstance<T>();
            }
        }


        /// <summary>
        /// Creates a new AppDomain that can be used to hold new references of T Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A new AppDomain where new instances can be created</returns>
        protected override AppDomain CreateNewProcess<T>()
        {
            curProcess = AppDomain.CreateDomain("NewDomain", null, null);
            return curProcess;

        }


        /// <summary>
        /// @todo
        /// </summary>
        /// <param name="mainProcess"></param>
        protected override void DestroyProcess(AppDomain mainProcess)
        {
            System.AppDomain.Unload(mainProcess); //TODO Review
        }

        /// <summary>
        /// @todo
        /// </summary>
        /// <returns></returns>
        protected override AppDomain GetCurrentProcess()
        {
            return AppDomain.CurrentDomain;
        }


        /// <summary>
        /// Singleton storing the ComponentServerHelper instance
        /// </summary>
        private static ComponentServerHelperAppDomainImplementation _instance = null;



        /// <summary>
        /// Obtains the singleton instance of the ComponentServerHelper
        /// </summary>
        /// <returns>The singleton instance of the ComponentServerHelper</returns>
        public static ComponentServerHelperAppDomainImplementation GetInstance(ComponentServerFactory factory)
        {
            if (_instance == null)
                _instance = new ComponentServerHelperAppDomainImplementation(factory);
            _instance.CheckFactory(factory);

            return _instance;
        }

        /// <summary>
        /// Obtains the singleton instance of the ComponentServerHelper
        /// </summary>
        /// <param name="useProcessValue">Indicates if the ComponentServerHelper should handle Processs or just works like a ClassFactory</param>
        /// <param name="factory">When this helper is created it requires a factory reference because factories are needed to create new instances of any time </param>
        /// <returns>The singleton instance of the ComponentServerHelper</returns>
        public static ComponentServerHelperAppDomainImplementation GetInstance(ComponentServerFactory factory, bool useProcessValue)
        {
            ComponentServerHelperAppDomainImplementation newInstance = GetInstance(factory);
            newInstance.useProcess = useProcessValue;
            return newInstance;
        }

        /// <summary>
        /// @todo
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="aDomain"></param>
        protected override void RemoveInstanceFromProcess(object instance, AppDomain aDomain)
        {
            //@todo look for factory and remove instance
            //instances are not removed from Domains, it doesn't apply
        }

    }

    /// <summary>
    /// Class used to ping the client that created instances of a Component Server.
    /// 
    /// </summary>
    public class ClientSponsor : MarshalByRefObject
    {
        /// <summary>
        /// Pings the client to find out if it is still running. 
        /// </summary>
        /// <returns>Always returns true. 
        /// The client can be considered dead if a call to Ping throws a RemotingException.
        /// </returns>
        public bool Ping()
        {
            return true;
        }

        /// <summary>
        /// Return null so the object's life will never expire
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

}