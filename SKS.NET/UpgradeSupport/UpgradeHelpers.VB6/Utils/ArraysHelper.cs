using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace UpgradeHelpers.VB6.Utils
{
    /// <summary>
    /// The ArraysHelper contains functionality for some array operations, such as: 
    /// initialization, casting, and redimension.
    /// </summary>
    public class ArraysHelper
    {
        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="E">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static E[] InitializeArray<E>(int length)
        {
            return InitializeArray<E[]>(new int[] { length });
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="E">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="lowerBound">The lower bound for the new array.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static E[] InitializeArray<E>(int length, int lowerBound)
        {
            return InitializeArray<E[]>(new int[] { length }, new int[] { lowerBound });
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="E">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static E[] InitializeArray<E>(int length, object[] constructorParams)
        {
            return InitializeArray<E[]>(new int[] { length }, constructorParams);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="E">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="lowerBound">The lower bound for the new array.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <returns>A new one-dimensional array with its values initialized to a default value.</returns>
        public static E[] InitializeArray<E>(int length, int lowerBound, object[] constructorParams)
        {
            return InitializeArray<E[]>(new int[] { length }, new int[] { lowerBound }, constructorParams);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="E">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="initValue">An initial value to set to each element.</param>
        /// <returns>A new one-dimensional array with its values initialized to initValue.</returns>
        public static E[] InitializeArray<E>(int length, object initValue)
        {
            return InitializeArray<E[]>(new int[] { length }, initValue);
        }

        /// <summary>
        /// Initializes a one-dimensional array.
        /// </summary>
        /// <typeparam name="E">The type of the elements of the array like 'string' for instance.</typeparam>
        /// <param name="length">The length of the new array.</param>
        /// <param name="lowerBound">The lower bound for the new array.</param>
        /// <param name="initValue">An initial value to set to each element.</param>
        /// <returns>A new one-dimensional array with its values initialized to initValue.</returns>
        public static E[] InitializeArray<E>(int length, int lowerBound, object initValue)
        {
            return InitializeArray<E[]>(new int[] { length }, new int[] { lowerBound }, initValue);
        }


        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static A InitializeArray<A>(int[] lengths) where A : class
        {
            return InitializeArray<A>(lengths, new object[] { });
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static A InitializeArray<A>(int[] lengths, int[] lowerBounds) where A : class
        {
            return InitializeArray<A>(lengths, lowerBounds, new object[] { });
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static A InitializeArray<A>(int[] lengths, object[] constructorParams) where A : class
        {
            if ((lengths == null) || (constructorParams == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths' or 'constructorParams' parameter is null");

            return InitializeArray<A>(lengths, new int[lengths.Length], constructorParams);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="constructorParams">The list of values to be sent to 
        /// the constructor of the item type of the array.</param>
        /// <returns>A new multi-dimensional array with its values initialized to a default value.</returns>
        public static A InitializeArray<A>(int[] lengths, int[] lowerBounds, object[] constructorParams) where A : class
        {
            if ((lengths == null) || (lowerBounds == null) || (constructorParams == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths', 'lowerBounds' or 'constructorParams' parameter is null");

            Type arrayType = typeof(A);
            if (!arrayType.IsArray)
                throw new Exception("AIS-Exception. Array type is expected as parameter");

            Type itemType = arrayType.GetElementType();
            if (itemType == null)
                throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

            InitialValueProvider valueProvider = new InitialValueProvider(itemType, constructorParams);

            //Only Primitive types and strings can be initialized with the same value, for other types a new instance 
            //will be used for each element
            if (itemType.IsPrimitive || itemType.Equals(typeof(string)))
                return InternalInitializeArray(lengths, lowerBounds, itemType, valueProvider.GetInitialValue()) as A;
            return InternalInitializeArray(lengths, lowerBounds, itemType, valueProvider) as A;
        }


        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="initValue">The init value to use for each element in the array.</param>
        /// <returns>A new multi-dimensional array with its values initialized with initValue.</returns>
        public static A InitializeArray<A>(int[] lengths, object initValue) where A : class
        {
            if (lengths == null)
                throw new NullReferenceException("AIS-Exception. 'lengths' parameter is null");

            return InitializeArray<A>(lengths, new int[lengths.Length], initValue);
        }

        /// <summary>
        /// Initializes a multi-dimensional array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="initValue">The init value to use for each element in the array.</param>
        /// <returns>A new multi-dimensional array with its values initialized with initValue.</returns>
        public static A InitializeArray<A>(int[] lengths, int[] lowerBounds, object initValue) where A : class
        {
            if ((lengths == null) || (lowerBounds == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths', 'lowerBounds' parameter is null");

            if (lengths.Length != lowerBounds.Length)
                throw new Exception("AIS-Exception. The length of 'lengths' and 'lowerBounds' parameters is different");

            Type arrayType = typeof(A);
            if (!arrayType.IsArray)
                throw new Exception("AIS-Exception. Array type is expected as parameter");

            Type itemType = arrayType.GetElementType();
            if (itemType == null)
                throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

            return InternalInitializeArray(lengths, lowerBounds, itemType, initValue) as A;
        }


        /// <summary>
        /// Internal method to initialize a multi-dimensional array.
        /// </summary>
        /// <param name="lengths">The length of each dimension.</param>
        /// <param name="lowerBounds">The lower bounds to use for each dimension.</param>
        /// <param name="itemType">The type to create the array.</param>
        /// <param name="value">The init value to use for each element in the array.</param>
        /// <returns>A new multi-dimensional array with its values initialized with initValue.</returns>
        private static Array InternalInitializeArray(int[] lengths, int[] lowerBounds, Type itemType, Object value)
        {
            object sampleValue;
            //Creates the array
            Array res = Array.CreateInstance(itemType, lengths, lowerBounds);

            //Initialize each element
            int[] upperBounds = new int[lowerBounds.Length];
            for (int i = 0; i < res.Rank; i++)
                upperBounds[i] = res.GetUpperBound(i);

            int[] indexes = new int[lengths.Length];
            Array.Copy(lowerBounds, indexes, lowerBounds.Length);

            int pos = res.Rank - 1;
            indexes[pos]--;
            pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);

            //Won't initialize anything if the default values are the expected
            if (pos >= 0)
            {
                sampleValue = res.GetValue(indexes);
                Object initValue = (value is InitialValueProvider) ? ((InitialValueProvider)value).GetInitialValue() : value;
                if (!(sampleValue == null && initValue == null) &&
                    (sampleValue == null || !sampleValue.Equals(initValue)))
                {
                    while (pos >= 0)
                    {
                        res.SetValue((value is InitialValueProvider) ? ((InitialValueProvider)value).GetInitialValue() : value, indexes);
                        pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);
                    }
                }
            }
            return res;
        }


        /// <summary>
        /// Executes a RedimPreserve over an array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions, for instance 'string[,,,]'.</typeparam>
        /// <param name="arraySource">The source array.</param>
        /// <param name="lengths">The length of the new dimensions.</param>
        /// <returns>The new array with the elements of the old one.</returns>
        public static A RedimPreserve<A>(A arraySource, int[] lengths) where A : class
        {
            if (lengths == null)
                throw new NullReferenceException("AIS-Exception. 'lengths' parameter is null");

            return RedimPreserve(arraySource, lengths, new int[lengths.Length]);
        }

        /// <summary>
        /// Executes a RedimPreserve over an array.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions, for instance 'string[,,,]'.</typeparam>
        /// <param name="arraySource">The source array.</param>
        /// <param name="lengths">The length of the new dimensions.</param>
        /// <param name="lowerBounds">The lower bound of the new dimensions.</param>
        /// <returns>The new array with the elements of the old one.</returns>
        public static A RedimPreserve<A>(A arraySource, int[] lengths, int[] lowerBounds) where A : class
        {
            Array res;

            Type arrayType = arraySource.GetType();
            Type arrayElementType;
            if (arrayType.ToString() == "System.Array")
            {
                arrayType = null;
                arrayElementType = (arraySource as Array).GetValue(0).GetType();
            }
            else
            {
                arrayElementType = arrayType.GetElementType();
            }

            if (arraySource == null)
                return InitializeArray<A>(lengths, lowerBounds);

            if (arrayType.ToString() != "System.Array")
                RunRedimPreserveVerifications(arraySource, arrayType, lengths, lowerBounds);
            Array array = arraySource as Array;
            //There is something to copy
            if (array != null)
            {
                res = Array.CreateInstance(arrayElementType, lengths, lowerBounds);
                InitialValueProvider valueProvider = new InitialValueProvider(arrayElementType, null);
                //Multiple dimensions
                if (array.Rank > 1)
                    FillsMultiDimensionalArray(array, res, valueProvider);
                else
                    FillsOneDimensionArray(array, res, valueProvider);
            }
            else
            {
                res = InitializeArray<A>(lengths, lowerBounds) as Array;
            }
            return res as A;
        }

        /// <summary>
        /// Fills the one-dimension targetArray with either matching cell values from 
        /// sourceArray or with a initial value.
        /// </summary>
        /// <param name="sourceArray">The array object containing the values to copy.</param>
        /// <param name="targetArray">The new array where to copy the values.</param>
        /// <param name="valueProvider">a <c>InitialValueProvider</c> object used to get 
        /// the default values for the new cells.</param>
        private static void FillsOneDimensionArray(Array sourceArray, Array targetArray, InitialValueProvider valueProvider)
        {
            Array.Copy(sourceArray, sourceArray.GetLowerBound(0), targetArray, sourceArray.GetLowerBound(0),
                Math.Min(targetArray.GetLength(0), sourceArray.GetLength(0)));
            if (targetArray.Length > sourceArray.Length)
                for (int i = sourceArray.Length; i < targetArray.Length; i++)
                    targetArray.SetValue(valueProvider.GetInitialValue(), i + targetArray.GetLowerBound(0));
        }

        /// <summary>
        /// Fills the n-dimension targetArray with either matching cell values from 
        /// sourceArray or with a initial value.
        /// </summary>
        /// <param name="sourceArray">The array object containing the values to copy.</param>
        /// <param name="targetArray">The new array where to copy the values.</param>
        /// <param name="valueProvider">a <c>InitialValueProvider</c> object used to get
        /// the default values for the new cells.</param>
        private static void FillsMultiDimensionalArray(Array sourceArray, Array targetArray, InitialValueProvider valueProvider)
        {
            int rowsToCopy = GetFirstDimensionsSize(sourceArray);
            int originalLastDimensionSize = GetLastDimensionSize(sourceArray);
            int newLastDimensionSize = GetLastDimensionSize(targetArray);
            int newCells = newLastDimensionSize - originalLastDimensionSize;
            int lowerBound = sourceArray.GetLowerBound(0);
            // creates a new array with same dimensions than the target array (but a smaller one version) to hold the 
            // default values to copy
            Array arrayLen = Array.CreateInstance(typeof(Int32), targetArray.Rank);
            for (int i = 0; i < targetArray.Rank - 1; i++)
                arrayLen.SetValue(1, i);
            arrayLen.SetValue(targetArray.GetLength(targetArray.Rank - 1), targetArray.Rank - 1);
            Array defaultValues = Array.CreateInstance(targetArray.GetType().GetElementType(), (int[])arrayLen);

            for (int i = 0; i < rowsToCopy; i++)
            {
                //copies the values from source array to target aray
                Array.Copy(sourceArray, (i * originalLastDimensionSize) + lowerBound, targetArray, (i * newLastDimensionSize) + lowerBound,
                    Math.Min(originalLastDimensionSize, newLastDimensionSize));
                // fills the remaining cells with the default value
                if (newCells > 0)
                {
                    for (int k = 0; k < arrayLen.Length; k++)
                        arrayLen.SetValue(0, k); // initilizing the indixes to first array element (0 on all dimensions)
                    // sets up the default values array with new values (we delegate to the value provider the responsability to get either
                    // a new or an old instance.
                    for (int j = originalLastDimensionSize; j < newLastDimensionSize; j++)
                    {
                        arrayLen.SetValue(j - originalLastDimensionSize, arrayLen.Length - 1); // moves the index
                        defaultValues.SetValue(valueProvider.GetInitialValue(), (int[])arrayLen);
                    }
                    Array.Copy(defaultValues, 0, targetArray,
                               (i * newLastDimensionSize) + originalLastDimensionSize + lowerBound, newCells);
                }
            }
        }

        /// <summary>
        /// Casts an array from one type to another.
        /// </summary>
        /// <typeparam name="A">The type of the array including the dimensions in the form 'string[,,]'.</typeparam>
        /// <param name="srcArray">The source array to cast.</param>
        /// <returns>A new array with the correct new target type.</returns>
        public static A CastArray<A>(Array srcArray) where A : class
        {
            A finalResult;
            Array tempResult;
            try
            {
                if (srcArray == null) return null;

                Type arrayType = typeof(A);
                if (!arrayType.IsArray)
                    throw new Exception("AIS-Exception. Array type is expected as parameter");

                Type itemType = arrayType.GetElementType();
                if (itemType == null)
                    throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

                int[] lengths = new int[srcArray.Rank];
                int[] lowerBounds = new int[srcArray.Rank];
                int[] upperBounds = new int[srcArray.Rank];
                for (int i = 0; i < srcArray.Rank; i++)
                {
                    lengths[i] = srcArray.GetLength(i);
                    lowerBounds[i] = srcArray.GetLowerBound(i);
                    upperBounds[i] = srcArray.GetUpperBound(i);
                }

                //Creates the array
                tempResult = Array.CreateInstance(itemType, lengths, lowerBounds);

                int[] indexes = new int[lengths.Length];
                Array.Copy(lowerBounds, indexes, lowerBounds.Length);

                int pos = tempResult.Rank - 1;
                indexes[pos]--;
                pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);

                while (pos >= 0)
                {
                    tempResult.SetValue(Convert.ChangeType(srcArray.GetValue(indexes), itemType), indexes);
                    pos = CalculateIndexes(ref indexes, pos, lowerBounds, upperBounds);
                }
            }
            catch (Exception e)
            {
                throw new Exception("AIS-Exception. Array casting is generating an exception: " + e.Message);
            }
            finalResult = tempResult as A;
            if (finalResult == null)
            {
                throw new Exception("AIS-Exception. Cannot cast a " + srcArray.GetType().ToString() + " to a " + typeof(A).ToString());
            }
            return finalResult;
        }

        /// <summary>
        /// Calculate the indexes of the next element to copy.
        /// </summary>
        /// <param name="indexes">The list of the indexes in the different dimensions for 
        /// the element to copy.</param>
        /// <param name="pos">The current position within the list of indexes.</param>
        /// <param name="lBounds">The list of lower bounds to use as limit.</param>
        /// <param name="UBounds">The list of upper bounds to use as limit.</param>
        /// <returns>The current position or -1 if the operation failed which means 
        /// there is no next element to copy.</returns>
        private static int CalculateIndexes(ref int[] indexes, int pos, int[] lBounds, int[] UBounds)
        {
            indexes[pos]++;
            if (indexes[pos] > UBounds[pos])
            {
                indexes[pos] = lBounds[pos];
                pos--;
                if (pos >= 0)
                {
                    pos = CalculateIndexes(ref indexes, pos, lBounds, UBounds);
                    if (pos >= 0)
                        pos++;
                }
            }

            return pos;
        }

        /// <summary>
        /// Run some basic verifications on the parameters sent to RedimPreserve function.
        /// </summary>
        /// <param name="arrayPrototype">The source array to verify.</param>
        /// <param name="arrayType">The type of the source array.</param>
        /// <param name="lengths">The length of the dimensions.</param>
        /// <param name="lowerBounds">The lower bound of each dimension.</param>
        private static void RunRedimPreserveVerifications(object arrayPrototype, Type arrayType, int[] lengths, int[] lowerBounds)
        {
            if (!arrayType.IsArray)
                throw new Exception("AIS-Exception. Array type is expected as parameter");

            Type itemType = arrayType.GetElementType();
            if (itemType == null)
                throw new NullReferenceException("AIS-Exception. itemType for the array couldn't be resolved");

            if ((lengths == null) || (lowerBounds == null))
                throw new NullReferenceException("AIS-Exception. Either 'lengths' or 'lowerBounds' parameter is null");

            if (lengths.Length != lowerBounds.Length)
                throw new Exception("AIS-Exception. The length of 'lengths' and 'lowerBounds' parameters is different");

            if ((arrayPrototype != null) && (arrayType.GetArrayRank() != lengths.Length))
                throw new Exception("AIS-Exception. Can't change the number of dimensions of the current array");
            Array array = (Array)arrayPrototype;
            for (int i = 0; i < lengths.Length - 1; i++)
            {
                if (array.GetLength(i) != lengths[i])
                    throw new Exception("AIS-Exception.  Only last dimension can be modified.");
                if (array.GetLowerBound(i) != lowerBounds[i])
                    throw new Exception("AIS-Exception.  Only last dimension can be modified.");
            }
        }

        /// <summary>
        /// Gets the size for the first dimension for an array.
        /// </summary>
        /// <param name="array">The array to process.</param>
        /// <returns>The size of the first dimension of the array.</returns>
        private static int GetFirstDimensionsSize(Array array)
        {
            int result = 1;
            for (int i = 0; i < array.Rank - 1; i++)
                result = result * array.GetLength(i);
            return result;
        }

        /// <summary>
        /// Gets the size for the last dimension for an array.
        /// </summary>
        /// <param name="array">The array to process.</param>
        /// <returns>The size of the last dimension of the array.</returns>
        private static int GetLastDimensionSize(Array array)
        {
            return array.GetLength(array.Rank - 1);
        }

        /// <summary>
        /// The InitialValueProvider provides an initial value from several methods.
        /// Used for initialization of element types of arrays.
        /// </summary>
        private class InitialValueProvider
        {
            /// <summary>
            /// The Enumeration of the different kind of methods of initialization.
            /// </summary>
            private enum InitialValueMethod { String, Constructor, ValueType, CreateInstanceValueType, CSFactory };
            /// <summary>
            /// The Type of array's elements.
            /// </summary>
            private readonly Type elementType;
            /// <summary>
            /// The list of values to be sent to the constructor used in the method CreateInstanceValueType.
            /// </summary>
            private Object[] constructorParams;
            /// <summary>
            /// Indicates if provider was already initialized.
            /// </summary>
            private bool initialized;
            /// <summary>
            /// The InitializeMethod for the current provider.
            /// </summary>
            private InitialValueMethod initializeMethod = InitialValueMethod.String;
            /// <summary>
            /// The Constructor method if constructor is gotten from elementType.
            /// </summary>
            private ConstructorInfo constructor;
            /// <summary>
            /// Some Method used for initialization of the elementType, like "CreateInstance".
            /// </summary>
            private MethodInfo method;

            /// <summary>
            /// Constructor for IniatialValueProvider.
            /// </summary>
            /// <param name="elementType">The type of the array's elements.</param>
            /// <param name="constructorParams">The list of values to be sent to the constructor of 
            /// the item type of the array.</param>
            public InitialValueProvider(Type elementType, Object[] constructorParams)
            {
                this.elementType = elementType;
                if (constructorParams == null)
                    this.constructorParams = new object[0];
                else
                    this.constructorParams = constructorParams;

                initialized = false;
            }

            /// <summary>
            /// Gets the value of initialization according to the InitialValueMethod of this provider.
            /// </summary>
            /// <returns>The value of initialization.</returns>
            public Object GetInitialValue()
            {
                Initialize();
                switch (initializeMethod)
                {
                    case InitialValueMethod.CSFactory:
                        try
                        {
                            Type factoryType = null;
                            foreach (Type possibleFactory in elementType.Assembly.GetExportedTypes())
                            {
                                if (possibleFactory.BaseType == typeof(UpgradeHelpers.VB6.Activex.ComponentServerFactory))
                                {
                                    factoryType = possibleFactory;
                                }
                            }
                            MethodInfo mi = factoryType.GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public);
                            MethodInfo miGeneric = mi.MakeGenericMethod(elementType);
                            return miGeneric.Invoke(null, new object[] { });
                        }
                        catch (Exception ex)
                        {
                            new Exception("Error while trying to get initial value for an array", ex);
                        }
                        break;
                    case InitialValueMethod.String:
                        return string.Empty;
                    case InitialValueMethod.Constructor:
                        return constructor.Invoke(constructorParams);
                    case InitialValueMethod.ValueType:
                        return Activator.CreateInstance(elementType);
                    case InitialValueMethod.CreateInstanceValueType:
                        return method.Invoke(null, new object[] { });
                }
                return null;
            }

            /// <summary>
            /// Initialize this provider to be able to gets the intialization value.
            /// </summary>
            private void Initialize()
            {
                if (!initialized)
                {
                    initialized = true;
                    if (elementType.BaseType == typeof(UpgradeHelpers.VB6.Activex.ComponentClassHelper)
                        || elementType.BaseType == typeof(UpgradeHelpers.VB6.Activex.ComponentSingleUseClassHelper) 
                        || elementType.BaseType == typeof(UpgradeHelpers.VB6.Activex.GlbComponentSingleUseClassHelper))
                    {
                        initializeMethod = InitialValueMethod.CSFactory;
                    }
                    else
                    if (!elementType.Equals(typeof(String)))
                    {
                        //try for a constructor method
                        if (constructorParams == null)
                            constructorParams = new object[] { };
                        if ((constructor = elementType.GetConstructor(Type.GetTypeArray(constructorParams))) == null)
                        {
                            if (elementType.IsValueType && (constructorParams == null || constructorParams.Length == 0))
                                initializeMethod = (method = elementType.GetMethod("CreateInstance")) == null ? InitialValueMethod.ValueType : InitialValueMethod.CreateInstanceValueType;
                        }
                        else
                            initializeMethod = InitialValueMethod.Constructor;
                    }
                }
            }


        }

        /// <summary>
        /// Makes a deep copy of an array.
        /// </summary>
        /// <param name="objectToCopy">Array to copy.</param>
        /// <returns>A deep copy of the array.</returns>
        public static object DeepCopy(object objectToCopy)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (object)binaryFormatter.Deserialize(memoryStream);
            }
        }

    }

    /// <summary>
    /// This Helper class is used to assist in the StructForAPI feature.
    /// When an structure has to do some interop with unmanaged code, we must provide
    /// a way to marshal arrays of fixed length strings.
    /// The way to do that is define an array of chars in the struct and a property 
    /// using this helper will facilitate 
    /// </summary>
    public class FixedLengthStringArrayHelper : IEnumerable<String>
    {
        char[] _buffer;
        int _element_size;
        int[] _dimensions;

        /// <summary>
        /// Builds an instance of a helper that is used when the StructForAPI feature is used.
        /// In those cases, arrays of FixedLength Strings have to be generated as char arrays (for marshalling reasons),
        /// and this helper provides a simplified way to "view" that array just any ordinary multidimensional array
        /// </summary>
        /// <param name="buffer">The array where the data will be</param>
        /// <param name="element_size">the size of each of the fixed strings</param>
        /// <param name="dimensions">the maximun length for each dimension</param>
        public FixedLengthStringArrayHelper(char[] buffer, int element_size,params int[] dimensions)
        {
            _buffer = buffer;
            _element_size = element_size;
            _dimensions = dimensions;
            if (dimensions.Length == 0)
            {
                throw new NotSupportedException("At least one dimension must be specified for this array");
            }
        }

        /// <summary>
        /// This indexer allows a simple access to the array elements, making it more natural
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String this[int index]
        {
            get
            {
                if (_dimensions.Length>1 || index < 0 || index > _dimensions[0])
                    throw new IndexOutOfRangeException();
                char[] temp = new char[_element_size];
                Array.Copy(_buffer, index * _element_size, temp, 0, _element_size);
                return new String(temp);
            }
            set
            {
                if (_dimensions.Length>1 || index < 0 || index > _dimensions[0])
                    throw new IndexOutOfRangeException();
                char[] temp = new char[_element_size];
                char[] aux = value.ToCharArray();
                Array.Copy(aux, 0, temp, 0, aux.Length);
                Array.Copy(temp, 0, _buffer, index * _element_size, temp.Length);
            }
        }

        /// <summary>
        /// Generic method used by the multidimensional indexer to validate indexes bounds
        /// </summary>
        /// <param name="indexDimension1">First Dimension index</param>
        /// <param name="indexDimension2">Second Dimension index</param>
        /// <param name="restIndexes">All other possible indexes</param>
        private void CheckValidIndexes(int indexDimension1, int indexDimension2, int[] restIndexes)
        {
                if (_dimensions.Length != 2 + restIndexes.Length ||
                    indexDimension1 < 0 || indexDimension1 > _dimensions[0] ||
                    indexDimension2 < 0 || indexDimension2 > _dimensions[1])
                    throw new IndexOutOfRangeException();
                //Check other dimensions we already know that _dimensions Length is equal to
                // 2 + restIndexes.Length
                int startCheckDimension = 0;
                foreach (int dimensionMax in restIndexes)
                {
                    if (restIndexes[startCheckDimension] > _dimensions[2 + startCheckDimension] ||
                        restIndexes[startCheckDimension] < 0)
                        throw new IndexOutOfRangeException();
                    startCheckDimension++;
                }
        }

        /// <summary>
        /// This indexer allows a simple access to the array elements, making it more natural.
        /// Accessing 2 or more dimensions is supposed to be less frequent that accessing just one dimension, for
        /// that reason a second indexer is provided to support accessing more than one dimension
        /// </summary>
        /// <param name="indexDimension1"></param>
        /// <param name="indexDimension2"></param>
        /// <param name="restIndexes"></param>
        /// <returns></returns>
        public String this[int indexDimension1, int indexDimension2, params int[] restIndexes]
        {
            get
            {
                CheckValidIndexes(indexDimension1, indexDimension2, restIndexes);
                char[] temp = new char[_element_size];
                int index = (indexDimension1 * _element_size) + (indexDimension2 * _element_size) ;
                foreach (int additionalIndex in restIndexes) { index = index + (_element_size * additionalIndex); }
                Array.Copy(_buffer, index * _element_size, temp, 0, _element_size);
                return new String(temp);
            }
            set
            {
                CheckValidIndexes(indexDimension1, indexDimension2, restIndexes);
                char[] temp = new char[_element_size];
                char[] aux = value.ToCharArray();
                Array.Copy(aux, 0, temp, 0, aux.Length);
                int index = (indexDimension1 * _element_size) + (indexDimension2 * _element_size) ;
                foreach (int additionalIndex in restIndexes) { index = index + (_element_size * additionalIndex); }
                Array.Copy(temp, 0, _buffer, index * _element_size, temp.Length);
            }
        }
        /// <summary>
        /// Returns the length of the array.
        /// For example if this helper is used for an array definition like:
        /// testarray(4) as String * 10
        /// 
        /// Then this property will return 5. (Remember that unless specified arrays in VB6 started at 0)
        /// </summary>
        public int Length
        {
            get
            {
                int size = 0;

                foreach (int dimension_size in _dimensions)
                {
                    if (size == 0)
                        size = dimension_size;
                    else
                        size *= dimension_size;
                }
                return size;
            }
        }

        #region IEnumerable<string> Members

        class FixedLengthStringArrayHelperEnumerator : IEnumerator<string>
        {
            FixedLengthStringArrayHelper _instance;
            public FixedLengthStringArrayHelperEnumerator(FixedLengthStringArrayHelper instance)
            {
                _instance = instance;
            }

            int current_position = -1;
            #region IEnumerator<string> Members

            public string Current
            {
                get
                {
                    if (current_position == -1 && current_position < _instance._buffer.Length)
                        throw new Exception("Invalid state");
                    else
                        return _instance[current_position];
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (current_position < _instance._dimensions[0] - 1)
                {
                    current_position++;
                    return true;
                }
                else
                {
                    return false;
                }

            }

            public void Reset()
            {
                current_position = -1;
            }

            #endregion
        }

        /// <summary>
        /// Returns an enumerator to facilitate transversal of all elements in array.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return new FixedLengthStringArrayHelperEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new FixedLengthStringArrayHelperEnumerator(this);
        }

        #endregion
    }


}
