using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Drawing;

namespace UpgradeHelpers.VB6.Utils
{
    /// <summary>
    /// The ReflectionHelper class provides functionality to handle the instantiation, 
    /// setting/reading properties and method invocation using reflection on the .NET Framework. 
    /// Using of this class is optional, and will only appear if it is selected in the Upgrade Profile. 
    /// It is used when it is necessary to continue using late-bound calls to 
    /// objects in the migrated application.
    /// </summary>
    public class ReflectionHelper
    {
        #region GetMember Methods
        /// <summary>
        /// Gets a member from an object by reflection.
        /// </summary>
        /// <typeparam name="T">The type, which the member value must be casted.</typeparam>
        /// <param name="obj">The source object that should be checked for the property.</param>
        /// <param name="propName">The name of the property that is required.</param>
        /// <param name="indexes">In the case that property represents an array 
        /// the index(es) must be specified here.</param>
        /// <returns>Returns the value of the member.</returns>
        public static T GetMember<T>(object obj, string propName, params object[] indexes)
        {
            object Result = null;
            propName = GetDotNetMemberName(obj.GetType(), propName, false);
            if ((indexes == null) || (indexes.Length == 0))
                Result = Interaction.CallByName(obj, propName, CallType.Get);
            else
                Result = Interaction.CallByName(obj, propName, CallType.Get, indexes);
            return (T)GetValueForcedToType(Result, typeof(T));
        }

        /// <summary>
        /// Gets a member from an object by reflection.
        /// </summary>
        /// <param name="obj">The source object that should be checked for the property.</param>
        /// <param name="propName">The name of the property that is required.</param>
        /// <param name="indexes">In the case that property represents an array 
        /// the index(es) must be specified here.</param>
        /// <returns>Returns the value of the member.</returns>
        public static object GetMember(object obj, string propName, params object[] indexes)
        {
            return GetMember<object>(obj, propName, indexes);
        }
        #endregion

        #region SetMember Methods
        /// <summary>
        /// Sets the value of a member from an object by reflection.
        /// </summary>
        /// <param name="obj">The source object that should be checked for the property.</param>
        /// <param name="propName">The name of the property that is required.</param>
        /// <param name="indexes">In the case that property represents an array 
        /// the index(es) must be specified here.</param>
        /// <param name="value">The new value to be set.</param>
        public static void SetMember(object obj, string propName, object value, params object[] indexes)
        {
            propName = GetDotNetMemberName(obj.GetType(), propName, true);
            value = GetValueForced(obj, propName, value);
            if ((indexes == null) || (indexes.Length == 0))
                Interaction.CallByName(obj, propName, CallType.Set, value);
            else
                Interaction.CallByName(obj, propName, CallType.Set, indexes, value);
        }
        #endregion

        #region LetMember Methods
        /// <summary>
        /// Lets the value of a member from an object by reflection.
        /// </summary>
        /// <param name="obj">The source object that should be checked for the property.</param>
        /// <param name="propName">The name of the property that is required.</param>
        /// <param name="indexes">In the case that property represents an array 
        /// the index(es) must be specified here.</param>
        /// <param name="value">The new value to be set.</param>
        public static void LetMember(object obj, string propName, object[] indexes, object value)
        {
            propName = GetDotNetMemberName(obj.GetType(), propName, true);
            value = GetValueForced(obj, propName, value);
            if ((indexes == null) || (indexes.Length == 0))
                Interaction.CallByName(obj, propName, CallType.Let, value);
            else
                Interaction.CallByName(obj, propName, CallType.Let, indexes, value);
        }

        /// <summary>
        /// Lets the value of a member from an object by reflection.
        /// </summary>
        /// <param name="obj">The source object that should be checked for the property.</param>
        /// <param name="propName">The name of the property that is required.</param>
        /// <param name="value">The new value to be set.</param>
        public static void LetMember(object obj, string propName, object value)
        {
            LetMember(obj, propName, null, value);
        }
        #endregion

        #region Invoke Methods
        /// <summary>
        /// Invokes a member from an object by reflection.
        /// </summary>
        /// <typeparam name="T">The type, which the invokation value must be casted.</typeparam>
        /// <param name="obj">The source object that should be checked for the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameters">An array containing the values of the parameters 
        /// to be used in the invocation.</param>
        /// <returns>The value returned by the invocation if one is returned.</returns>
        public static T Invoke<T>(object obj, string memberName, object[] parameters)
        {
            object Result = null;
            memberName = GetDotNetMemberName(obj.GetType(), memberName, false);
            PropertyInfo pInfo = obj.GetType().GetProperty(memberName);
            if (pInfo != null)
                Result = Interaction.CallByName(obj, memberName, CallType.Get, parameters);
            else
                Result = Interaction.CallByName(obj, memberName, CallType.Method, parameters);
            return (T)GetValueForcedToType(Result, typeof(T));
        }

        /// <summary>
        /// Invokes a member from an object by reflection.
        /// </summary>
        /// <typeparam name="T">The type, which the invokation value must be casted.</typeparam>
        /// <param name="obj">The source object that should be checked for the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameters">An array containing the values of the parameters 
        /// to be used in the invocation.</param>
        /// <param name="paramNames">The names of params corresponding to the parameters sent
        /// int the other array 'parameters'.</param>
        /// <returns>The value returned by the invocation if one is returned.</returns>
        public static T Invoke<T>(object obj, string memberName, object[] parameters, string[] paramNames)
        {
            parameters = OrderNamedParameters(obj, memberName, parameters, paramNames);
            return Invoke<T>(obj, memberName, parameters);
        }

        /// <summary>
        /// Invokes a member from an object by reflection.
        /// </summary>
        /// <param name="obj">The source object that should be checked for the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameters">An array containing the values of the parameters 
        /// to be used in the invocation.</param>
        /// <returns>The value returned by the invocation if one is returned.</returns>
        public static object Invoke(object obj, string memberName, object[] parameters)
        {
            return Invoke<object>(obj, memberName, parameters);
        }

        /// <summary>
        /// Invokes a member from an object by reflection.
        /// </summary>
        /// <param name="obj">The source object that should be checked for the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameters">An array containing the values of the parameters 
        /// to be used in the invocation.</param>
        /// <param name="paramNames">The names of params corresponding to the parameters sent
        /// int the other array 'parameters'.</param>
        /// <returns>The value returned by the invocation if one is returned.</returns>
        public static object Invoke(object obj, string memberName, object[] parameters, string[] paramNames)
        {
            return Invoke<object>(obj, memberName, parameters, paramNames);
        }
        #endregion

        #region SetPrimitiveValue
        /// <summary>
        /// Obtains the default property from 'ctl' and sets the 'propertyValue' to it.
        /// </summary>
        /// <param name="ctl">The control to be set the 'propertyValue'.</param>
        /// <param name="propertyValue">The value to be set to the default property of the 'obj'.</param>
        /// <param name="index">The arguments to invoke the default property if needed.</param>
        public static void SetPrimitiveValue(ref Control ctl, object propertyValue, params object[] index)
        {
            object obj = ctl;
            SetPrimitiveValue(ref obj, propertyValue, index);
            ctl = (Control)obj;
        }
        
        /// <summary>
        /// Obtains the default property from 'obj' and sets the 'propertyValue' to it.
        /// </summary>
        /// <param name="obj">The object to be set the 'propertyValue'.</param>
        /// <param name="propertyValue">The value to be set to the default property of the 'obj'.</param>
        /// <param name="index">The arguments to invoke the default property if needed.</param>
        public static void SetPrimitiveValue(ref object obj, object propertyValue, params object[] index)
        {
            try
            {
                PropertyInfo pInfo = null;
                Type type = (obj==null)?null:obj.GetType();
                if (IsPrimitive(type))
                {
                    obj = propertyValue;
                    return;
                }
                else if (obj is Array)
                {
                    obj = propertyValue; // should this be a copy array? (hfernan)
                    return;
                }
                else if (IsIntrinsic(type))
                    pInfo = (PropertyInfo)ObtainDefaultPropertyFromIntrinsic(type, true);
                else if (IsCOMObject(type))
                    pInfo = (PropertyInfo)ObtainDefaultPropertyFromCOM(obj);
                else
                    pInfo = (PropertyInfo)ObtainDefaultPropertyFromInternalClass(obj);

                if (IsPrimitive(pInfo.PropertyType))
                    pInfo.SetValue(obj, GetValueForcedToType(propertyValue, pInfo.PropertyType), index);
                else
                {
                    object newObj = pInfo.GetValue(obj, new object[] { });
                    SetPrimitiveValue(ref newObj, propertyValue, index);
                }
            }
            catch (Exception e)
            {
                throw new DefaultPropertyException("Error setting Default Property. Type: " + ((obj==null)?"null":obj.GetType().FullName)  , e);
            }
        }
        #endregion

        #region GetPrimitiveValue
        /// <summary>
        /// Obtains the default property value from the 'obj'.
        /// </summary>
        /// <param name="obj">The object to get the default property.</param>
        /// <typeparam name="T">The type, which the default property value must be casted.</typeparam>
        /// <param name="index">The arguments to invoke the default property if needed.</param>
        /// <returns>The default property value obtained from the 'obj'</returns>
        public static T GetPrimitiveValue<T>(object obj, params object[] index)
        {
            try
            {
                MemberInfo pInfo = null;
                Type type = (obj == null) ? null : obj.GetType();
                if (IsPrimitive(type))
                    return (T)GetValueForcedToType(obj, typeof(T));
                else if (obj is Array)
                    return (T)ArraysHelper.DeepCopy(obj);
                else if (IsIntrinsic(type))
                    pInfo = ObtainDefaultPropertyFromIntrinsic(type, false);
                else if (IsCOMObject(type))
                    pInfo = ObtainDefaultPropertyFromCOM(obj);
                else
                    pInfo = ObtainDefaultPropertyFromInternalClass(obj);

                object noCastedValue = null;
                object[] parameters = new object[] { };
                bool isAlreadyPrimitive = false;

                if (pInfo is PropertyInfo)
                    isAlreadyPrimitive = IsPrimitive(((PropertyInfo)pInfo).PropertyType);
                else
                    isAlreadyPrimitive = IsPrimitive(((MethodInfo)pInfo).ReturnType);
                if (isAlreadyPrimitive) parameters = index;

                if (pInfo is PropertyInfo)
                    noCastedValue = ((PropertyInfo)pInfo).GetValue(obj, parameters);
                else
                    noCastedValue = ((MethodInfo)pInfo).Invoke(obj, parameters);

                if (isAlreadyPrimitive)
                    return (T)GetValueForcedToType(noCastedValue, typeof(T));
                else
                    return GetPrimitiveValue<T>(noCastedValue, index);
            }
            catch (Exception e)
            {
                throw new DefaultPropertyException("Error getting Default Property, Type: " + ((obj == null) ? "null" : obj.GetType().FullName) + " Ex: " + e.ToString(), e);
            }
        }

        /// <summary>
        /// Obtains the default property value from the 'obj'.
        /// </summary>
        /// <param name="obj">The object to get the default property.</param>
        /// <param name="index">The arguments to invoke the default property if needed.</param>
        /// <returns>The default property value obtained from the 'obj'</returns>
        public static object GetPrimitiveValue(object obj, params object[] index)
        {
            return GetPrimitiveValue<object>(obj, index);
        }

        #endregion

        #region Utility Methods
        /// <summary>
        /// The mappings loaded from Mappings.xml resource file to map the member name during execution
        /// </summary>
        private static Dictionary<string, string> mappings = null;
        /// <summary>
        /// The default properties loaded from DefaultProperties.xml resource file
        /// </summary>
        private static Dictionary<string, string> defaultProps = null;
        /// <summary>
        /// Constant to access Xml Tag in the DefaulProperties file
        /// </summary>
        private const string XML_TAG_DEFAULT_PROPERTY = "DefaultProperty";
        /// <summary>
        /// Constant to access Xml Tag in the DefaulProperties file
        /// </summary>
        private const string XML_TAG_MAPPING = "Mapping";

        /// <summary>
        /// Functions to check if a member exists.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <param name="propName">The name of the member.</param>
        /// <returns>True if the member exists.</returns>
        protected internal static bool ExistMember(object obj, string propName)
        {
            try
            {
                GetMember(obj, propName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the .NET name corresponding to this type and memberName.
        /// </summary>
        /// <param name="objType">The Type of the object being accessed via the ReflectionHelper.</param>
        /// <param name="memberName">The name of the member being searched.</param>
        /// <param name="theSetProperty">Indicates if look for the Set/Get default property.</param>
        /// <returns>The resultant name string to search into the object.</returns>
        private static string GetDotNetMemberName(Type objType, string memberName, bool theSetProperty)
        {
            if (mappings == null)
            {
                LoadXmlFile("Mappings.xml", XML_TAG_MAPPING, ref mappings);
                LoadExtendedXmlFile("ExtendedMappings.xml", XML_TAG_MAPPING, ref mappings);
            }

            string postfix = (theSetProperty ? ".Set." : ".Get.");
            if (mappings.ContainsKey(objType.FullName + "." + memberName))
                return mappings[objType.FullName + "." + memberName];
            else if (mappings.ContainsKey(objType.FullName + postfix + memberName))
                return mappings[objType.FullName + postfix + memberName];
            else if ((objType.BaseType != null) && (objType.BaseType.FullName != "System.Windows.Forms.AxHost"))
                return GetDotNetMemberName(objType.BaseType, memberName, theSetProperty);
            else return memberName;
        }

        /// <summary>
        /// Order the list of parameters for an invokation according to the corresponding
        /// parameter names in the list 'paramNames'.
        /// </summary>
        /// <param name="obj">The object where to find the method to be invoked.</param>
        /// <param name="methodName">The method name being invoked.</param>
        /// <param name="parameters">The list of parameters to order.</param>
        /// <param name="paramNames">The list of parameters names to use for ordering the parameters.</param>
        /// <returns>The parameters ordered according to the paramNames.</returns>
        private static object[] OrderNamedParameters(object obj, string methodName, object[] parameters, string[] paramNames)
        {
            // Look for the MethodInfo. Multiple declarations will throw an exception because
            // it doesnt know which declaration must choose
            MethodInfo theMethod = obj.GetType().GetMethod(methodName);

            ParameterInfo[] declaredParams = theMethod.GetParameters();
            object[] resultantParams = new object[declaredParams.Length];
            int cont = 0;
            foreach (ParameterInfo pInfo in declaredParams)
            {
                int paramPos = 0;
                foreach (string paramName in paramNames)
                {
                    if (paramName == pInfo.Name)
                        break;
                    paramPos++;
                }
                if (paramPos < paramNames.Length)
                    resultantParams[cont] = parameters[paramPos];
                else if (cont < paramNames.Length && String.IsNullOrEmpty(paramNames[cont]))
                    resultantParams[cont] = parameters[cont];
                else
                    resultantParams[cont] = null;
                cont++;
            }

            return resultantParams;
        }

        /// <summary>
        /// Indicates if 'type' is a Primitive Type.
        /// </summary>
        /// <param name="type">The 'System.Type' to be checked.</param>
        /// <returns>True if the 'type' is a primitive Type.</returns>
        private static bool IsPrimitive(Type type)
        {
            return (type == null || type.IsPrimitive || type.IsEnum || (type.FullName == "System.String") || (type.FullName == "System.Object") || (type.FullName == "System.DBNull") || (type.FullName == "System.Decimal") || (type.FullName == "System.DateTime"));
        }

        /// <summary>
        /// Indicates if 'type' is a Intrisic Type, it means it is not a COM object and it has a default property.
        /// </summary>
        /// <param name="type">The 'System.Type' to be checked.</param>
        /// <returns>True if the 'type' is a intrinsic Type.</returns>
        private static bool IsIntrinsic(Type type)
        {
            if (defaultProps == null)
                LoadXmlFile("DefaultProperties.xml", XML_TAG_DEFAULT_PROPERTY, ref defaultProps);
            return defaultProps.ContainsKey(type.FullName) ||
                   defaultProps.ContainsKey(type.FullName + ".Get") ||
                   defaultProps.ContainsKey(type.FullName + ".Set");
        }

        /// <summary>
        /// Indicates if 'type' is a COMObject or a wrapped ActiveX object (AxHost).
        /// </summary>
        /// <param name="type">The 'System.Type' to be checked.</param>
        /// <returns>True if the 'type' is a COMObject Type.</returns>
        private static bool IsCOMObject(Type type)
        {
            return (type.IsCOMObject || ((type.BaseType != null) && (type.BaseType.FullName == "System.Windows.Forms.AxHost")));
        }

        /// <summary>
        /// Gets the indicated 'propertyName' in the 'type'.
        /// </summary>
        /// <param name="type">The 'System.Type' where to look for.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <returns>A 'MemberInfo' containing the indicated default property.</returns>
        private static MemberInfo ObtainProperty(Type type, string propertyName)
        {
            return type.GetMember(propertyName)[0];
        }

        /// <summary>
        /// Gets the default property in an Intrinsic type.
        /// </summary>
        /// <param name="type">The 'System.Type' to look for.</param>
        /// <param name="theSetProperty">Indicates if look for the Set/Get default property.</param>
        /// <returns>A 'MemberInfo' containing the default property.</returns>
        private static MemberInfo ObtainDefaultPropertyFromIntrinsic(Type type, bool theSetProperty)
        {
            if (defaultProps.ContainsKey(type.FullName))
                return ObtainProperty(type, defaultProps[type.FullName]);
            else
                return ObtainProperty(type, defaultProps[type.FullName] + (theSetProperty ? ".Set" : ".Get"));
        }

        /// <summary>
        /// Gets the default property in a COMObject type.
        /// </summary>
        /// <param name="obj">The 'obj' where to look for.</param>
        /// <returns>A 'MemberInfo' containing the default property.</returns>
        private static MemberInfo ObtainDefaultPropertyFromCOM(object obj)
        {
            MemberInfo pInfo = null;

            Type type = obj.GetType();
            if ((type.BaseType != null) && (type.BaseType.FullName == "System.Windows.Forms.AxHost"))
            {
                object ocxObj = ((AxHost)obj).GetOcx();
                IDispatch ocxIDisp = (IDispatch)ocxObj;
                IntPtr ocxTypeInfo = IntPtr.Zero;
                ocxIDisp.GetTypeInfo(0, 0, ref ocxTypeInfo);
                type = Marshal.GetTypeForITypeInfo(ocxTypeInfo);
            }
            MemberInfo[] mInfo = type.GetDefaultMembers();
            if (mInfo.Length == 1)
            {
                pInfo = ObtainProperty(obj.GetType(), mInfo[0].Name);
            }
            else
            {
                throw new DefaultPropertyException("Default Property for object '" + obj.ToString() + "' was not found");
            }
            return pInfo;
        }


        /// <summary>
        /// Gets the default property in a user internal class.
        /// </summary>
        /// <param name="obj">The 'obj' where to look for.</param>
        /// <returns>A 'MemberInfo' containing the default property or null if it is not found.</returns>
        private static MemberInfo ObtainDefaultPropertyFromInternalClass(object obj)
        {
            MemberInfo pInfo = null;

            Type type = obj.GetType();
            MemberInfo[] mInfo = type.GetDefaultMembers();
            if (mInfo.Length == 1)
            {
                pInfo = ObtainProperty(type, mInfo[0].Name);
            }
            return pInfo;
        }

        /// <summary>
        /// Load extended mappings if needed
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tagName"></param>
        /// <param name="dict"></param>
        private static void LoadExtendedXmlFile(string fileName, string tagName, ref Dictionary<string, string> dict)
        {
            //TODO: to be implemented!
        }

        /// <summary>
        /// Loads the a xml resource file containing the list of mappings/default properties.
        /// </summary>
        /// <param name="fileName">The file name to load.</param>
        /// <param name="tagName">The string tag name to search each element into the xml.</param>
        /// <param name="dict">The 'Dictionary' where to add the element from the xml file.</param>
        private static void LoadXmlFile(string fileName, string tagName, ref Dictionary<string, string> dict)
        {
            try
            {
                dict = new Dictionary<string, string>();
                XmlDocument xmlDoc = new XmlDocument();
                Assembly theAssembly = Assembly.GetExecutingAssembly();
                string resourceNameStr = typeof(ReflectionHelper).Namespace + "." + fileName;
                xmlDoc.Load(theAssembly.GetManifestResourceStream(resourceNameStr));
                if (null != xmlDoc)
                {
                    foreach (XmlNode node in xmlDoc.GetElementsByTagName(tagName))
                    {
                        string key = "";
                        string val = node.InnerText;
                        if (tagName == XML_TAG_MAPPING)
                            key = node.Attributes["DotNetTypeName"].Value + "." + node.Attributes["VB6MemberName"].Value;
                        else if (tagName == XML_TAG_DEFAULT_PROPERTY)
                            key = node.Attributes["FullName"].Value;
                        if (!dict.ContainsKey(key))
                            dict.Add(key, val);
                        else if (val != dict[key])
                            System.Diagnostics.Debug.Fail("Error Loading Xml File: " + fileName, "It is being added the key '" + key + "' with a different value '" + val + "'");
                    }
                }
            }
            catch (Exception e)
            {
                throw new DefaultPropertyException("Error Loading " + fileName + " file", e);
            }
        }
        #endregion

        #region Execution Casting Methods
        /// <summary>
        /// Converts the 'propertyValue' instance to the corresponding Type.
        /// </summary>
        /// <param name="obj">The object where to find the propertyName.</param>
        /// <param name="propertyName">The property name to look into the obj.</param>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to corresponding property type into the object.</returns>
        private static object GetValueForced(object obj, string propertyName, object propertyValue)
        {
            PropertyInfo pInfo = obj.GetType().GetProperty(propertyName);
            if (pInfo != null)
                propertyValue = GetValueForcedToType(propertyValue, pInfo.PropertyType);
            else
            {
                FieldInfo fInfo = obj.GetType().GetField(propertyName);
                if (fInfo != null)
                    propertyValue = GetValueForcedToType(propertyValue, fInfo.FieldType);
            }
            return propertyValue;
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to 'targetType'.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <param name="targetType">The Type to be converted.</param>
        /// <returns>A value converted to 'targetType'.</returns>
        private static object GetValueForcedToType(object propertyValue, Type targetType)
        {
            if (targetType.FullName == "System.Object")
                return propertyValue;
            else if (targetType.FullName == "System.String")
                return GetValueForcedToString(propertyValue);
            else if (targetType.FullName == "System.Double")
                return GetValueForcedToDouble(propertyValue);
            else if (targetType.FullName == "System.Single")
                return GetValueForcedToFloat(propertyValue);
            else if (targetType.FullName == "System.Int64")
                return GetValueForcedToLong(propertyValue);
            else if (targetType.FullName == "System.Int32")
                return GetValueForcedToInt(propertyValue);
            else if (targetType.FullName == "System.Int16")
                return GetValueForcedToShort(propertyValue);
            else if (targetType.FullName == "System.Byte")
                return GetValueForcedToByte(propertyValue);
            else if (targetType.FullName == "System.Boolean")
                return GetValueForcedToBoolean(propertyValue);
            else if (targetType.FullName == "System.Decimal")
                return GetValueForcedToDecimal(propertyValue);
            else if (targetType.FullName == "System.Char")
                return GetValueForcedToChar(propertyValue);
            else if (targetType.FullName == "System.DateTime")
                return GetValueForcedToDate(propertyValue);
            else if (targetType.FullName == "System.Drawing.Color")
                return GetValueForcedToColor(propertyValue);
            else if (targetType.FullName == "System.Windows.Forms.Cursor")
                return GetValueForcedToCursor(propertyValue);
            else if (targetType.FullName == "System.Windows.Forms.PictureBoxSizeMode")
                return GetValueForcedToPictureBoxSizeMode(propertyValue);
            else if (targetType.IsArray)
                return GetValueForcedToArray(propertyValue);
            else if (targetType.IsEnum)
                return GetValueForcedToEnum(propertyValue, targetType);
            else if (targetType.IsClass)
                return GetValueForcedToClass(propertyValue);

            // No Identified Type
            return propertyValue;
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to string.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to string.</returns>
        private static string GetValueForcedToString(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if ((sourceType.FullName == "System.Double") || (sourceType.FullName == "System.Single") ||
                (sourceType.FullName == "System.Int64") || (sourceType.FullName == "System.Int32") ||
                (sourceType.FullName == "System.Int16") || (sourceType.FullName == "System.Byte") ||
                (sourceType.FullName == "System.Boolean"))
                return propertyValue.ToString();
            else if (sourceType.FullName == "System.DateTime")
                return DateTimeHelper.ToString((DateTime)propertyValue);
            else if ((sourceType.IsArray) && (sourceType.GetElementType().FullName == "System.Byte"))
                return StringsHelper.ByteArrayToString((byte[])propertyValue);
            else if (sourceType.IsEnum)
                return ((int)propertyValue).ToString();

            return Convert.ToString(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue', which is a boolean instance to numeric.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A boolean value converted to numeric.</returns>
        private static object GetBooleanValueForcedToNumeric(object propertyValue)
        {
            return (((bool)propertyValue) ? -1 : 0);
        }

        /// <summary>
        /// Converts the 'propertyValue', which is a date instance to numeric.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A date value converted to numeric.</returns>
        private static object GetDateValueForcedToNumeric(object propertyValue)
        {
            return ((DateTime)propertyValue).ToOADate();
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to double.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to double.</returns>
        private static double GetValueForcedToDouble(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return Double.Parse((string)propertyValue);
            else if (sourceType.FullName == "System.Boolean")
                return (double)GetBooleanValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.DateTime")
                return (double)GetDateValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.Drawing.Color")
                return (double)ColorTranslator.ToOle((Color)propertyValue);

            return Convert.ToDouble(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to float.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to float.</returns>
        private static float GetValueForcedToFloat(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return Convert.ToInt16(Double.Parse((string)propertyValue));
            else if (sourceType.FullName == "System.Boolean")
                return (float)GetBooleanValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.DateTime")
                return (float)GetDateValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.Drawing.Color")
                return (float)ColorTranslator.ToOle((Color)propertyValue);

            return Convert.ToSingle(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to long.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to long.</returns>
        private static long GetValueForcedToLong(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return System.Convert.ToInt32((string)propertyValue);
            else if (sourceType.FullName == "System.Boolean")
                return (long)GetBooleanValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.DateTime")
                return (long)GetDateValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.Drawing.Color")
                return (long)ColorTranslator.ToOle((Color)propertyValue);

            return Convert.ToInt64(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to int.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to int.</returns>
        private static int GetValueForcedToInt(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return Convert.ToInt32(Double.Parse((string)propertyValue));
            else if (sourceType.FullName == "System.Boolean")
                return (int)GetBooleanValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.DateTime")
                return (int)GetDateValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.Drawing.Color")
                return (int)ColorTranslator.ToOle((Color)propertyValue);

            return Convert.ToInt32(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to short.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to short.</returns>
        private static short GetValueForcedToShort(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return Convert.ToInt16(Double.Parse((string)propertyValue));
            else if (sourceType.FullName == "System.Boolean")
                return (short)GetBooleanValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.DateTime")
                return (short)GetDateValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.Drawing.Color")
                return (short)ColorTranslator.ToOle((Color)propertyValue);

            return Convert.ToInt16(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to byte.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to byte.</returns>
        private static byte GetValueForcedToByte(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return Convert.ToByte(Double.Parse((string)propertyValue));
            else if (sourceType.FullName == "System.Boolean")
                return (byte)GetBooleanValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.DateTime")
                return (byte)GetDateValueForcedToNumeric(propertyValue);
            else if (sourceType.FullName == "System.Drawing.Color")
                return (byte)ColorTranslator.ToOle((Color)propertyValue);

            return Convert.ToByte(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to bool.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to bool.</returns>
        private static bool GetValueForcedToBoolean(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
            {
                bool bValue;
                return Boolean.TryParse((string)propertyValue, out bValue) ? bValue : Convert.ToBoolean(Double.Parse((string)propertyValue));
            }
            else if (sourceType.IsEnum)
                return (GetValueForcedToInt(propertyValue) == 0);
            else if (sourceType.FullName == "System.Drawing.Color")
                return Convert.ToBoolean(ColorTranslator.ToOle((Color)propertyValue));

            return Convert.ToBoolean(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to bool.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to bool.</returns>
        private static decimal GetValueForcedToDecimal(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.String")
                return Decimal.Parse((string)propertyValue);
            else if (sourceType.FullName == "System.Boolean")
                return Convert.ToDecimal(GetBooleanValueForcedToNumeric(propertyValue));
            else if (sourceType.FullName == "System.DateTime")
                return Convert.ToDecimal(GetDateValueForcedToNumeric(propertyValue));
            else if (sourceType.FullName == "System.Drawing.Color")
                return Convert.ToDecimal(ColorTranslator.ToOle((Color)propertyValue));

            return Convert.ToDecimal(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to char.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to char.</returns>
        private static char GetValueForcedToChar(object propertyValue)
        {
            return Convert.ToChar(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to datetime.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to datetime.</returns>
        private static DateTime GetValueForcedToDate(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.Boolean")
                return new DateTime(Convert.ToInt64(propertyValue));
            else if ((sourceType.FullName == "System.Double") || (sourceType.FullName == "System.Single") ||
                (sourceType.FullName == "System.Int64") || (sourceType.FullName == "System.Int32") ||
                (sourceType.FullName == "System.Int16") || (sourceType.FullName == "System.Byte") ||
                (sourceType.FullName == "System.Boolean"))
                return DateTime.FromOADate((double)propertyValue);
            else if (sourceType.FullName == "System.String")
            {
                DateTime dValue;
                return DateTime.TryParse((string)propertyValue, out dValue) ? dValue : DateTime.FromOADate(Double.Parse((string)propertyValue));
            }
            else if (sourceType.FullName == "System.Drawing.Color")
                return Convert.ToDateTime(ColorTranslator.ToOle((Color)propertyValue));

            return Convert.ToDateTime(propertyValue);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to System.Drawing.Color.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to System.Drawing.Color.</returns>
        private static Color GetValueForcedToColor(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.UInt32")
            {   
                string hexValue = Convert.ToString(((UInt32)propertyValue), 16);
                if (hexValue.Length == 8 &&  hexValue.StartsWith("8"))
                    return GetValueForcedToSystemColor(hexValue);
            }

            return ColorTranslator.FromOle(GetValueForcedToInt(propertyValue));
        }

        private static Color GetValueForcedToSystemColor(string propertyValue)
        {
            switch (propertyValue)
            {
                case "80000000": return System.Drawing.SystemColors.ScrollBar;
                case "80000001": return System.Drawing.SystemColors.Desktop;
                case "80000002": return System.Drawing.SystemColors.ActiveCaption;
                case "80000003": return System.Drawing.SystemColors.InactiveCaption;
                case "80000004": return System.Drawing.SystemColors.Menu;
                case "80000005": return System.Drawing.SystemColors.Window;
                case "80000006": return System.Drawing.SystemColors.WindowFrame;
                case "80000007": return System.Drawing.SystemColors.MenuText;
                case "80000008": return System.Drawing.SystemColors.WindowText;
                case "80000009": return System.Drawing.SystemColors.ActiveCaptionText;
                case "80000010": return System.Drawing.SystemColors.ActiveBorder;
                case "80000011": return System.Drawing.SystemColors.InactiveBorder;
                case "80000012": return System.Drawing.SystemColors.AppWorkspace;
                case "80000013": return System.Drawing.SystemColors.Highlight;
                case "80000014": return System.Drawing.SystemColors.HighlightText;
                case "80000015": return System.Drawing.SystemColors.Control;
                case "80000016": return System.Drawing.SystemColors.ControlDark;
                case "80000017": return System.Drawing.SystemColors.GrayText;
                case "80000018": return System.Drawing.SystemColors.ControlText;
                case "80000019": return System.Drawing.SystemColors.InactiveCaptionText;
                case "80000020": return System.Drawing.SystemColors.ControlLight;
                case "80000021": return System.Drawing.SystemColors.ControlDarkDark;
                case "80000022": return System.Drawing.SystemColors.ControlLightLight;
                case "80000023": return System.Drawing.SystemColors.InfoText;
                case "80000024": return System.Drawing.SystemColors.Info;
            }

            System.Diagnostics.Debug.Assert(true, "Getting an invalid System Color!!");
            return System.Drawing.SystemColors.Control;
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to System.Windows.Forms.PictureBoxSizeMode.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to System.Windows.Forms.PictureBoxSizeMode.</returns>
        private static PictureBoxSizeMode GetValueForcedToPictureBoxSizeMode(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName != "System.Windows.Forms.PictureBoxSizeMode")
                return (PictureBoxSizeMode)propertyValue;
            else if (sourceType.FullName != "System.Boolean")
                propertyValue = GetValueForcedToBoolean(propertyValue);
            
            return ((bool)propertyValue ? PictureBoxSizeMode.StretchImage : PictureBoxSizeMode.Normal);
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to System.Windows.Forms.Cursor
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to System.Windows.Forms.Cursor.</returns>
        private static Cursor GetValueForcedToCursor(object propertyValue)
        {
            Type sourceType = GetPropertyValueType(propertyValue);
            if (sourceType.FullName == "System.Windows.Forms.Cursor")
                return (Cursor)propertyValue;

            int cursorIntValue = GetValueForcedToInt(propertyValue);
            switch (cursorIntValue)
            {
                case 0: return Cursors.Default;
                case 1: return Cursors.Arrow;
                case 2: return Cursors.Cross;
                case 3: return Cursors.IBeam;
                case 6: return Cursors.SizeNESW;
                case 7: return Cursors.SizeNS;
                case 8: return Cursors.SizeNWSE;
                case 9: return Cursors.SizeWE;
                case 10: return Cursors.UpArrow;
                case 11: return Cursors.WaitCursor;
                case 12: return Cursors.No;
                case 13: return Cursors.WaitCursor;
                case 14: return Cursors.Help;
                case 15: return Cursors.SizeAll;
                default: return (Cursor)propertyValue;
            }
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to array.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to array.</returns>
        private static object GetValueForcedToArray(object propertyValue)
        {
            return propertyValue;
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to a class.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <returns>A value converted to a class.</returns>
        private static object GetValueForcedToClass(object propertyValue)
        {
            return propertyValue;
        }

        /// <summary>
        /// Converts the 'propertyValue' instance to a enum.
        /// </summary>
        /// <param name="propertyValue">The value to be converted.</param>
        /// <param name="enumType">The enum type to be casted.</param>
        /// <returns>A value converted to a enum.</returns>
        private static object GetValueForcedToEnum(object propertyValue, Type enumType)
        {
            return Enum.Parse(enumType, GetValueForcedToString(propertyValue));
        }

        /// <summary>
        /// Retrieves the property 'Type' according to the value of the propertyValue.
        /// </summary>
        /// <param name="propertyValue">The value to be checked.</param>
        /// <returns>The respective Type according to the propertyValue.</returns>
        private static Type GetPropertyValueType(object propertyValue)
        {
            return (propertyValue == null) ? typeof(Object) : propertyValue.GetType();
        }
        #endregion
    }

    #region Custom Exceptions
    /// <summary>
    /// Represents errors that occur during Default Property handling.
    /// </summary>
    public class DefaultPropertyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a DefaulPropertyException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DefaultPropertyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of a DefaulPropertyException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception tha tis the cause of the current exception
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public DefaultPropertyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
    #endregion

    #region IDispatch Util Declarations
    // Types declarations used when the object is a ComObject. 
    // Necessary if we are going to use reflection to reach members using 
    // the IDispatch interface of com objects.

    /// <summary>
    /// ITypeComp IDispatch interface.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020403-0000-0000-C000-000000000046")]
    internal interface ITypeComp
    {
        void Bind([MarshalAs(UnmanagedType.LPWStr)] string szName, int lHashVal, short wFlags, out ITypeInfo ppTInfo, out System.Runtime.InteropServices.ComTypes.DESCKIND pDescKind, out System.Runtime.InteropServices.ComTypes.BINDPTR pBindPtr);
        void BindType([MarshalAs(UnmanagedType.LPWStr)] string szName, int lHashVal, out ITypeInfo ppTInfo, out ITypeComp ppTComp);
    }

    /// <summary>
    /// ITypeLib IDispatch interface.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020402-0000-0000-C000-000000000046")]
    internal interface ITypeLib
    {
        void RemoteGetTypeInfoCount([Out, MarshalAs(UnmanagedType.LPArray)] int[] pcTInfo);
        void GetTypeInfo([In, MarshalAs(UnmanagedType.U4)] int index, [Out, MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo);
        void GetTypeInfoType([In, MarshalAs(UnmanagedType.U4)] int index, [Out, MarshalAs(UnmanagedType.LPArray)] tagTYPEKIND[] pTKind);
        void GetTypeInfoOfGuid([In] ref Guid guid, [Out, MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo);
        void RemoteGetLibAttr(IntPtr ppTLibAttr, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pDummy);
        void GetTypeComp([Out, MarshalAs(UnmanagedType.LPArray)] ITypeComp[] ppTComp);
        void RemoteGetDocumentation(int index, [In, MarshalAs(UnmanagedType.U4)] int refPtrFlags, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrName, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrDocString, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pdwHelpContext, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrHelpFile);
        void RemoteIsName([In, MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, [In, MarshalAs(UnmanagedType.U4)] int lHashVal, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] pfName, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrLibName);
        void RemoteFindName([In, MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, [In, MarshalAs(UnmanagedType.U4)] int lHashVal, [Out, MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo, [Out, MarshalAs(UnmanagedType.LPArray)] int[] rgMemId, [In, Out, MarshalAs(UnmanagedType.LPArray)] short[] pcFound, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrLibName);
        void LocalReleaseTLibAttr();
    }

    /// <summary>
    /// tagTYPEKIND IDispatch enumeration.
    /// </summary>
    internal enum tagTYPEKIND
    {
        TKIND_ENUM,
        TKIND_RECORD,
        TKIND_MODULE,
        TKIND_INTERFACE,
        TKIND_DISPATCH,
        TKIND_COCLASS,
        TKIND_ALIAS,
        TKIND_UNION,
        TKIND_MAX
    }

    /// <summary>
    /// tagINVOKEKIND IDispatch enumeration.
    /// </summary>
    internal enum tagINVOKEKIND
    {
        INVOKE_FUNC = 1,
        INVOKE_PROPERTYGET = 2,
        INVOKE_PROPERTYPUT = 4,
        INVOKE_PROPERTYPUTREF = 8
    }

    /// <summary>
    /// ITypeInfo IDispatch interface.
    /// </summary>
    [ComImport, Guid("00020401-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITypeInfo
    {
        [PreserveSig]
        int GetTypeAttr(ref IntPtr pTypeAttr);
        [PreserveSig]
        int GetTypeComp([Out, MarshalAs(UnmanagedType.LPArray)] ITypeComp[] ppTComp);
        [PreserveSig]
        int GetFuncDesc([In, MarshalAs(UnmanagedType.U4)] int index, ref IntPtr pFuncDesc);
        [PreserveSig]
        int GetVarDesc([In, MarshalAs(UnmanagedType.U4)] int index, ref IntPtr pVarDesc);
        [PreserveSig]
        int GetNames(int memid, [Out, MarshalAs(UnmanagedType.LPArray)] string[] rgBstrNames, [In, MarshalAs(UnmanagedType.U4)] int cMaxNames, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcNames);
        [PreserveSig]
        int GetRefTypeOfImplType([In, MarshalAs(UnmanagedType.U4)] int index, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pRefType);
        [PreserveSig]
        int GetImplTypeFlags([In, MarshalAs(UnmanagedType.U4)] int index, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pImplTypeFlags);
        [PreserveSig]
        int GetIDsOfNames(IntPtr rgszNames, int cNames, IntPtr pMemId);
        [PreserveSig]
        int Invoke();
        [PreserveSig]
        int GetDocumentation(int memid, ref string pBstrName, ref string pBstrDocString, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pdwHelpContext, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrHelpFile);
        [PreserveSig]
        int GetDllEntry(int memid, tagINVOKEKIND invkind, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrDllName, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrName, [Out, MarshalAs(UnmanagedType.LPArray)] short[] pwOrdinal);
        [PreserveSig]
        int GetRefTypeInfo(IntPtr hreftype, ref ITypeInfo pTypeInfo);
        [PreserveSig]
        int AddressOfMember();
        [PreserveSig]
        int CreateInstance([In] ref Guid riid, [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppvObj);
        [PreserveSig]
        int GetMops(int memid, [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrMops);
        [PreserveSig]
        int GetContainingTypeLib([Out, MarshalAs(UnmanagedType.LPArray)] ITypeLib[] ppTLib, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pIndex);
        [PreserveSig]
        void ReleaseTypeAttr(IntPtr typeAttr);
        [PreserveSig]
        void ReleaseFuncDesc(IntPtr funcDesc);
        [PreserveSig]
        void ReleaseVarDesc(IntPtr varDesc);
    }

    /// <summary>
    /// IDispatch interface.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020400-0000-0000-C000-000000000046")]
    internal interface IDispatch
    {
        int GetTypeInfoCount();
        int GetTypeInfo(uint iTInfo, uint lcid, ref IntPtr TypeLib);
        [PreserveSig]
        int GetIDsOfNames([In] ref Guid riid, [In, MarshalAs(UnmanagedType.LPArray)] string[] rgszNames, [In, MarshalAs(UnmanagedType.U4)] int cNames, [In, MarshalAs(UnmanagedType.U4)] int lcid, [Out, MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);
        [PreserveSig]
        int Invoke(int dispIdMember, [In] ref Guid riid, [In, MarshalAs(UnmanagedType.U4)] int lcid, [In, MarshalAs(UnmanagedType.U4)] int dwFlags, [In, Out] tagDISPPARAMS pDispParams, [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult, [In, Out] tagEXCEPINFO pExcepInfo, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] pArgErr);
    }

    /// <summary>
    /// tagDISPPARAMS IDispatch class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class tagDISPPARAMS
    {
        public IntPtr rgvarg;
        public IntPtr rgdispidNamedArgs;
        [MarshalAs(UnmanagedType.U4)]
        public int cArgs;
        [MarshalAs(UnmanagedType.U4)]
        public int cNamedArgs;
    }

    /// <summary>
    /// tagEXCEPINFO IDispatch class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class tagEXCEPINFO
    {
        [MarshalAs(UnmanagedType.U2)]
        public short wCode;
        [MarshalAs(UnmanagedType.U2)]
        public short wReserved;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrSource;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrDescription;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrHelpFile;
        [MarshalAs(UnmanagedType.U4)]
        public int dwHelpContext;
        public IntPtr pvReserved = IntPtr.Zero;
        public IntPtr pfnDeferredFillIn = IntPtr.Zero;
        [MarshalAs(UnmanagedType.U4)]
        public int scode;
    }
    #endregion
}
