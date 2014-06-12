using System;
using System.Collections.Generic;
using System.Text;

namespace UpgradeHelpers.VB6.Utils
{

    /// <summary>
    /// The StringsHelper is an utility that provides funcationality related to string operations.
    /// </summary>
    public class StringsHelper
    {
        /// <summary>
        /// VbStrConv Enum used for the runtime implementation of StringsHelper.StrConv.
        /// </summary>
        [Flags]
        public enum VbStrConvEnum
        {
            /// <summary>
            /// From Unicode
            /// </summary>
            vbFromUnicode = 128,
            /// <summary>
            /// Hiragana
            /// </summary>
            vbHiragana = 32,
            /// <summary>
            /// Katakana
            /// </summary>
            vbKatakana = 16,
            /// <summary>
            /// Lower case
            /// </summary>
            vbLowerCase = 2,
            /// <summary>
            /// Narrow
            /// </summary>
            vbNarrow = 8,
            /// <summary>
            /// ProperCase
            /// </summary>
            vbProperCase = 3,
            /// <summary>
            /// Unicode
            /// </summary>
            vbUnicode = 64,
            /// <summary>
            /// Upper case
            /// </summary>
            vbUpperCase = 1,
            /// <summary>
            /// Wide char
            /// </summary>
            vbWide = 4
        }

        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Text.Encoding.Default, otherwise the encoding System.Text.Encoding.Unicode
        ///     will be used.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="Conversion">The type of the conversion to execute.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv(string str, VbStrConvEnum Conversion)
        {
            //0 is to indicate to use the default ANSI encode of the machine
            return StrConv(str, Conversion, 0);
        }

        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Text.Encoding.Default, otherwise the encoding System.Text.Encoding.Unicode
        ///     will be used.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="Conversion">The type of the conversion to execute.</param>
        /// <param name="LocaleID">The LocaleID to use in the conversion.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv(string str, VbStrConvEnum Conversion, int LocaleID)
        {
            string res = string.Empty;
            IntPtr strPtr = IntPtr.Zero;
            byte[] b;

            switch (Conversion)
            {
                //Please do not modify the implementations for vbFromUnicode and vbUnicode because they have been
                //already proveed with several systems
                case VbStrConvEnum.vbFromUnicode:
                    strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(str);
                    res = System.Runtime.InteropServices.Marshal.PtrToStringUni(strPtr);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
                    break;
                case VbStrConvEnum.vbUnicode:
                    //It is also possible to use the specific encoding:
                    //     - Encoding.GetEncoding("Windows-1252") or
                    //     - Encoding.GetEncoding(1252)
                    b = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.Unicode, System.Text.Encoding.Unicode.GetBytes(str));
                    res = System.Text.Encoding.Unicode.GetString(b);
                    break;
                default:
                    res = Microsoft.VisualBasic.Strings.StrConv(str, (Microsoft.VisualBasic.VbStrConv)((int)Conversion), LocaleID);
                    break;
            }

            return res;
        }

        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Runtime.InteropServices.Marshal.StringToHGlobalUni.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="Conversion">The type of the conversion to execute.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv2(string str, VbStrConvEnum Conversion)
        {
            //0 is to indicate to use the default ANSI encode of the machine
            return StrConv2(str, Conversion, 0);
        }


        /// <summary>
        /// Runtime implementation for VBA.Strings.StrConv VERSION 2
        /// note:
        ///     If Conversion == vbUnicode then the string returned will be encoded using
        ///     System.Runtime.InteropServices.Marshal.StringToHGlobalUni.
        /// </summary>
        /// <param name="str">Byte array representing an string.</param>
        /// <param name="Conversion">The type of the conversion to execute.</param>
        /// <param name="LocaleID">The LocaleID to use in the conversion.</param>
        /// <returns>The converted string.</returns>
        public static string StrConv2(string str, VbStrConvEnum Conversion, int LocaleID)
        {
            string res = string.Empty;
            IntPtr strPtr = IntPtr.Zero;

            switch (Conversion)
            {
                //Please do not modify the implementations for vbFromUnicode and vbUnicode because they have been
                //already proveed with several systems (C995_045)
                case VbStrConvEnum.vbFromUnicode:
                    strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(str);
                    res = System.Runtime.InteropServices.Marshal.PtrToStringUni(strPtr);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
                    break;
                case VbStrConvEnum.vbUnicode:
                    strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(str);
                    res = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr, str.Length * 2);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
                    break;
                default:
                    res = Microsoft.VisualBasic.Strings.StrConv(str, (Microsoft.VisualBasic.VbStrConv)((int)Conversion), LocaleID);
                    break;
            }

            return res;
        }


        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="array">Byte array to be converted.</param>
        /// <returns>The string converted in Unicode encoding.</returns>
        public static string ByteArrayToString(byte[] array)
        {
            if (array != null)
            {
                byte[] sArray;
                if (array.Length % 2 == 0)
                    sArray = array;
                else
                {
                    sArray = new byte[array.Length + 1];
                    Array.Copy(array, sArray, array.Length);
                }

                return System.Text.Encoding.Unicode.GetString(sArray);
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Replaces a portion of a string with other string. Provides the same functionality than
        /// MidAssignment for Visual Basic 6.
        /// </summary>
        /// <param name="str">The string to be changed.</param>
        /// <param name="start">The index into the string where to start the changing.</param>
        /// <param name="length">The length of the portion of string to change.</param>
        /// <param name="val">The new string to change into the other one.</param>
        /// <returns>The changed string with the new portion.</returns>
        public static string MidAssignment(string str, int start, int length, string val)
        {
            int minTmp = Math.Min(length, Math.Min(val.Length, str.Length - (start - 1)));

            return str.Substring(0, start - 1) + val.Substring(0, minTmp) + str.Substring(start - 1 + minTmp);
        }

        /// <summary>
        /// Replaces a portion of a string with other string. Provides the same functionality than
        /// MidAssignment for Visual Basic 6.
        /// </summary>
        /// <param name="str">The string to be changed.</param>
        /// <param name="start">The index into the string where to start the replace.</param>
        /// <param name="val">The new string to change into the other one.</param>
        /// <returns>The changed string with the new portion.</returns>
        public static string MidAssignment(string str, int start, string val)
        {
            return MidAssignment(str, start, int.MaxValue, val);
        }

        /// <summary>
        /// Matches a string value with a regular expression pattern.
        /// </summary>
        /// <param name="value">The string to be matched.</param>
        /// <param name="pattern">The regular expression used to match the string.</param>
        /// <returns>True if the pattern matches into the string.</returns>
        public static bool Like(string value, string pattern)
        {
            System.Diagnostics.Trace.WriteLine("WARNING: Using VB6 Like operator pattern this affects performace. Pattern used " + pattern);
            pattern = pattern.Replace("*", @".*");
            pattern = pattern.Replace('?', '.');
            return System.Text.RegularExpressions.Regex.Match(value, pattern).Success;
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="_toFormat">The String object to format.</param>
        /// <param name="_mask">The format to apply.</param>
        /// <param name="dayOfWeek">A value chosen from the FirstDayOfWeek enumeration that specifies the first day of the week.</param>
        /// <param name="weekOfYear">A value chosen from the FirstWeekOfYear enumeration that specifies the first week of the year.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object _toFormat, object _mask, Microsoft.VisualBasic.FirstDayOfWeek dayOfWeek, Microsoft.VisualBasic.FirstWeekOfYear weekOfYear)
        {
            string toFormat = Convert.ToString(_toFormat);
            string mask = Convert.ToString(_mask);

            if (String.IsNullOrEmpty(toFormat))
                return String.Empty;
            return Microsoft.VisualBasic.Compatibility.VB6.Support.Format(toFormat, mask, dayOfWeek, weekOfYear);
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <param name="mask">The format to apply.</param>
        /// <param name="dayOfWeek">A value chosen from the FirstDayOfWeek enumeration that specifies the first day of the week.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat, object mask, Microsoft.VisualBasic.FirstDayOfWeek dayOfWeek)
        {
            return Format(toFormat, mask, dayOfWeek, Microsoft.VisualBasic.FirstWeekOfYear.Jan1);
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <param name="mask">The format to apply.</param>
        /// <param name="weekOfYear">A value chosen from the FirstWeekOfYear enumeration that specifies the first week of the year.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat, object mask, Microsoft.VisualBasic.FirstWeekOfYear weekOfYear)
        {
            return Format(toFormat, mask, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, weekOfYear);
        }

        /// <summary>
        /// Returns the String toFormat formatted with the given mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <param name="mask">The format to apply.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat, object mask)
        {
            return Format(toFormat, mask, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, Microsoft.VisualBasic.FirstWeekOfYear.Jan1);
        }

        /// <summary>
        /// Returns the String toFormat formatted with an empty mask.
        /// </summary>
        /// <param name="toFormat">The String object to format.</param>
        /// <returns>Empty String if toFormat is null or empty, othewise the formatted string.</returns>
        public static String Format(object toFormat)
        {
            return Format(toFormat, String.Empty, Microsoft.VisualBasic.FirstDayOfWeek.Sunday, Microsoft.VisualBasic.FirstWeekOfYear.Jan1);
        }

        /// <summary>
        /// Gets a double value represented by the given String value. If value contains an 
        /// invalid number then a Double.NaN is returned.  
        /// This method is used to do safe castings between strings and numeric values.
        /// It is required for comparisons between strings and primitive types which were allowed by VB6 but are invalid in .NET.
        /// </summary>
        /// <param name="value">String containing the double value to convert.</param>
        /// <returns>A double value.</returns>
        public static double ToDoubleSafe(String value)
        {
            double dValue;
            return Double.TryParse(value, System.Globalization.NumberStyles.Any, null, out dValue)? dValue: Double.NaN;
        }

    }
}
