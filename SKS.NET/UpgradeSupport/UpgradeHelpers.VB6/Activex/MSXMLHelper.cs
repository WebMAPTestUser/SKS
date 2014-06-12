using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Security.Policy;
using System.IO;
using System.Globalization;


namespace UpgradeHelpers.VB6.Activex
{
    /// <summary>
    /// This class implements support for common functionality provided by VB6 MSXML
    /// class members provide support to interact with an xml document
    /// setting or retrieving information to/from an xml document;
    /// applying specific transformation (XSLT) to an xml document.
    /// </summary>
    public class MSXMLHelper
    {
        /// <summary>Transforms the XML data using the specified XSLT stylesheet, applies it using the provided memory stream.</summary>
        /// <param name="Node">The XmlNode that contains the XML data to be transformed.</param>
        /// <param name="Stylesheet">The XSLT stylesheet to be used.</param>
        /// <param name="theStream">The memory stream to be used.</param>
        /// <returns>Returns the string representation of the transformation.</returns>
        private static void CommonTransformation(XmlNode Node, XmlNode Stylesheet, Stream theStream)
        {
            XslCompiledTransform Transformation = new XslCompiledTransform();
            XmlUrlResolver resolver = new XmlUrlResolver();
            Transformation.Load(Stylesheet.CreateNavigator(), new XsltSettings(), resolver);
            Transformation.Transform(Node.CreateNavigator(), new XsltArgumentList(), theStream);
        }

        /// <summary>Transforms the XML data using the specified XSLT stylesheet.</summary>
        /// <param name="Node">The XmlNode that contains the XML data to be transformed.</param>
        /// <param name="Stylesheet">The XSLT stylesheet to be used.</param>
        /// <returns>Returns the string representation of the transformation.</returns>
        public static String TransformNode(XmlNode Node, XmlNode Stylesheet)
        {
            MemoryStream theStream = new MemoryStream();
            CommonTransformation(Node, Stylesheet, (Stream)theStream);
            //It is also possible to use the specific encoding:
            //     - Encoding.GetEncoding("Windows-1252") or
            //     - Encoding.GetEncoding(1252)
            return Encoding.Default.GetString(theStream.ToArray());
        }

         /// <summary>Transforms the XML data using the specified XSLT stylesheet.</summary>
         /// <param name="Node">The XmlNode that contains the XML data to be transformed.</param>
         /// <param name="Stylesheet">The XSLT stylesheet to be used.</param>
         /// <param name="Output">The XmlNode used to return the transformation.</param>
        public static void TransformNodeToObject(XmlNode Node, XmlNode Stylesheet, XmlNode Output)
        {
            MemoryStream theStream = new MemoryStream();
            CommonTransformation(Node, Stylesheet, (Stream)theStream);
            XmlDocument Document = new System.Xml.XmlDocument();
            //It is also possible to use the specific encoding:
            //     - Encoding.GetEncoding("Windows-1252") or
            //     - Encoding.GetEncoding(1252)
            Document.LoadXml(Encoding.Default.GetString(theStream.ToArray()));
            Output = Document;
        }

        /// <summary>Transforms the XML data using the specified XSLT stylesheet.</summary>
        /// <param name="Node">The XmlNode that contains the XML data to be transformed.</param>
        /// <param name="Stylesheet">The XSLT stylesheet to be used.</param>
        /// <param name="Output">The Stream used to return the transformation.</param>
        public static void TransformNodeToObject(XmlNode Node, XmlNode Stylesheet, ref  Stream Output)
        {
            CommonTransformation(Node, Stylesheet, Output);
        }

        /// <summary>Obtains a typed node value from the specified XmlNode.</summary>
        /// <param name="node">The XmlNode that contains the XML data.</param>
        /// <returns>Returns the typed node value.</returns>
        public static Object GetNodeTypedValue(XmlNode node)
        {

            String dataType = String.Empty;
	if (node.Attributes != null)
            if (node.Attributes["dt:dt"] != null)
            {
                dataType = node.Attributes["dt:dt"].Value;
            }
            else if (node.Attributes["dt"] != null)
            {
                dataType = node.Attributes["dt"].Value;
            }
            switch (dataType)
            {
                case "bin.base64":
                    //MIME-style Base64 encoded binary BLOB
                    return Convert.FromBase64String(node.InnerText);
                case "bin.hex":
                    //Hexadecimal-encoded binary data
                    String nodeInnerText = node.InnerText;
                    int arrayLength = (int)Math.Ceiling((double)nodeInnerText.Length / 2);
                    byte[] result = new byte[arrayLength - 1];
                    nodeInnerText = nodeInnerText.PadLeft(arrayLength * 2, '0');
                    for (int index = arrayLength - 1; index >= 0; index--)
                    {
                        String hexValue = nodeInnerText.Substring((index * 2) - 2, 2);
                        result[index - 1] = Byte.Parse(hexValue, NumberStyles.HexNumber);
                    }
                    return result;
                case "boolean":
                    //A value of either 0 or 1
                    return Convert.ToBoolean(node.InnerText);
                case "char":
                    //A number corresponding to the Unicode representation of a single character
                    return Convert.ToInt32(Convert.ToChar(node.InnerText));
                case "date":
                    //A date in a subset of the ISO 8601 format, without the time data
                    return Convert.ToDateTime(node.InnerText);
                case "dateTime":
                    //A date in a subset of the ISO 8601 format, with optional time and no optional zone. Fractional seconds can be as precise as nanoseconds
                    return Convert.ToDateTime(node.InnerText);
                case "dateTime.tz":
                    //A date in a subset of the ISO 8601 format, with optional time and optional zone. Fractional seconds can be as precise as nanoseconds
                    return Convert.ToDateTime(node.InnerText);
                case "entity":
                    //A reference to an unparsed entity
                    return node.InnerText;
                case "entities":
                    //A list of entities delimited by white space
                    return node.InnerText;
                case "enumeration":
                    //Similar to nmtoken, but with an explicit list of allowed values 
                    return node.InnerText;
                case "fixed.14.4":
                    //A number with no more than 14 digits to the left of the decimal point and no more than 4 to the right
                    return Convert.ToDecimal(node.InnerText);
                case "float":
                    //A real number, with no limit on the digits (2.2250738585072014E-308 - 1.7976931348623157E+308). 
                    return Convert.ToDouble(node.InnerText);
                case "i1":
                    //A one-byte integer, with optional sign 
                    return Convert.ToSByte(node.InnerText);
                case "i2":
                    //A two-byte integer with optional sign 
                    return Convert.ToInt16(node.InnerText);
                case "i4":
                    //A four-byte integer with optional sign 
                    return Convert.ToInt32(node.InnerText);
                case "i8":
                    //An eight-byte integer with optional sign 
                    return Convert.ToInt64(node.InnerText);
                case "id":
                    //A value that identifies an attribute as an id type attribute
                    return node.InnerText;
                case "idref":
                    //A value corresponding to an id type, thus enabling intra-document links
                    return node.InnerText;
                case "idrefs":
                    //Similar to idref, except it contains multiple id type values separated by white space
                    return node.InnerText;
                case "int":
                    //A signed integer
                    return Convert.ToInt32(node.InnerText);
                case "nmtoken":
                    //Values that conform to the rules of the name token
                    return node.InnerText;
                case "nmtokens":
                    //Similar to nmtoken, except it can have a list of nmtoken values separated by white space
                    return node.InnerText;
                case "notation":
                    //A NOTATION type
                    return node.InnerText;
                case "number":
                    //A number with no limits on the digits 
                    return node.InnerText;
                case "r4":
                    //Same as float but only four-byte encoding (1.17549435E-38 - 3.40282347E+38).
                    return Convert.ToSingle(node.InnerText);
                case "r8":
                    //A floating point number. This data type only supports 15 digits of precision 
                    return Convert.ToDouble(node.InnerText);
                case "string":
                    //A string
                    return node.InnerText;
                case "time":
                    //A time in a subset of the ISO 8601 format with no date and no time zone
                    return Convert.ToDateTime(node.InnerText);
                case "time.tz":
                    //A time in a subset of the ISO 8601 format with no date but optional time zone
                    return Convert.ToDateTime(node.InnerText);
                case "ui1":
                    //A one-byte unsigned integer 
                    return Convert.ToByte(node.InnerText);
                case "ui2":
                    //A two-byte unsigned integer 
                    return Convert.ToUInt16(node.InnerText);
                case "ui4":
                    //A four-byte unsigned integer 
                    return Convert.ToUInt32(node.InnerText);
                case "ui8":
                    //An eight-byte unsigned integer 
                    return Convert.ToUInt64(node.InnerText);
                case "uri":
                    //A Uniform Resource Identifier (URI). 
                    return node.InnerText;
                case "uuid":
                    //Hexadecimal digits representing octets with optional embedded hyphens that are ignored
                    return node.InnerText;
                default:
                    //Nothing
                    return node.InnerText;
            }
        }

        /// <summary>Sets the specified XmlNode with the specified typed value.</summary>
        /// <param name="node">The XmlNode.</param>
        /// <param name="value">The typed node value.</param>
        public static void SetNodeTypedValue(ref XmlNode node, Object value)
        {
            String dataType = String.Empty;
            if (node.Attributes["dt:dt"] != null)
            {
                dataType = node.Attributes["dt:dt"].Value;
            }
            else if (node.Attributes["dt"] != null)
            {
                dataType = node.Attributes["dt"].Value;
            }
            if (value is String)
            {
                node.InnerText = (String)value;
            }
            else
            {
                switch (dataType)
                {
                    case "bin.base64":
                        node.InnerText = Convert.ToBase64String((byte[])value);
                        break;
                    case "bin.hex":
                        StringBuilder stringValue = new StringBuilder();
                        byte[] byteArray = value as byte[];
                        for (int index = 0; index < byteArray.Length; index++)
                        {
                            String number = Convert.ToString(byteArray[index], 16);
                            number = number.PadLeft(2, '0');
                            stringValue.Append(number);
                        }
                        node.InnerText = stringValue.ToString();
                        break;
                    default:
                        //'Other types or Nothing
                        node.InnerText = Convert.ToString(value);
                        break;
                }
            }
        }

        /// <summary>Sets the specified data type value to the specified node.</summary>
        /// <param name="node">The XmlNode.</param>
        /// <param name="value">The data type.</param>
        public static void SetDataType(ref XmlNode node, String value)
        {

            String attributeName = "dt:dt";
            Boolean attributeExists = false;

            if (node.Attributes["dt"] != null)
            {
                attributeName = "dt";
                attributeExists = true;
            }
            else if (node.Attributes["dt:dt"] != null)
            {
                attributeName = "dt:dt";
                attributeExists = true;
            }

            if (!attributeExists)
            {
                XmlAttribute attibute = node.OwnerDocument.CreateAttribute(attributeName);
                attibute.Value = value;
                node.Attributes.Append(attibute);
            }
            else
            {
                node.Attributes[attributeName].Value = value;
            }
        }

        /// <summary>Gets the specified data type value from the specified node.</summary>
        /// <param name="node">The XmlNode.</param>
        /// <returns>Returns the node data type value.</returns>
        public static string GetDataType(XmlNode node)
        {

            String attributeName = "dt:dt";
            bool attributeExists = false;
            if (node.Attributes["dt"] != null)
            {
                attributeName = "dt";
                attributeExists = true;
            }
            else if (node.Attributes["dt:dt"] != null)
            {
                attributeName = "dt:dt";
                attributeExists = true;
            }
            if (attributeExists)
                return node.Attributes[attributeName].Value;
            else
                return String.Empty;
        }
    }
}
