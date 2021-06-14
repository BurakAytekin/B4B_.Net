using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace B2b.Web.v4.Models.Helper
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        ///     Serializes a generic object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>A string giving the serialized object</returns>
        public static string Serialize<T>(this T obj)
        {
            string returnString = null;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                // Look pretty and use UTF-8
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                    Encoding = Encoding.UTF8
                };

                var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                ns.Add("", "");

                using (StringWriter sw = new Utf8StringWriter())
                {
                    using (var textWriter = XmlWriter.Create(sw, settings))
                    {
                        xmlSerializer.Serialize(textWriter, obj, ns);
                    }
                    sw.Flush();
                    returnString = sw.ToString();
                }
            }
            catch { }
            return returnString;
        }

        /// <summary>
        ///     Takes a string and returns an object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="str">The string</param>
        /// <returns>An object of type T</returns>
        public static T Deserialize<T>(string str)
        {
            T returnValue = default(T);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var xmlReader = new StringReader(str))
                {
                    returnValue = (T)xmlSerializer.Deserialize(xmlReader);
                }
            }
            catch { }
            return returnValue;
        }
    }



    /// <summary>
    ///     A class only to override encoding with UTF8.
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}