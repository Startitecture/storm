namespace SAF.Data.Persistence
{
    using System;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides static methods for serialization.
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        /// Serializes an object to an XML file at the specified path.
        /// </summary>
        /// <param name="source">The object to serialize</param>
        /// <param name="path">Full path and file name</param>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentNullException">The filename is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The directory to 
        /// write to is not found.</exception>
        /// <exception cref="System.ArgumentException">The encoding is not supported; 
        /// the filename is empty, contains only white space, or contains one or more 
        /// invalid characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have 
        /// the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="System.IO.IOException">The filename includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label syntax.</exception>
        /// <exception cref="System.Xml.XmlException">LOADING: There is a load or 
        /// parse error in the XML. In this case, the document remains empty. 
        /// SAVING: The operation would not result in a well formed XML document 
        /// (for example, no document element or duplicate XML declarations).</exception>
        public static void ObjectToXmlFile(object source, string path)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            XmlSerializer xs = new XmlSerializer(source.GetType());

            using (XmlWriter writer = new XmlTextWriter(path, Encoding.Unicode))
            {
                xs.Serialize(writer, source);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            xmlDoc.Save(path);
        }

        /// <summary>
        /// Creates an object from an XML file.
        /// </summary>
        /// <param name="path">Path of the XML file to deserialize from</param>
        /// <param name="type">Object type to serialize to</param>
        /// <returns>A deserialized object.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The specified file cannot 
        /// be found.</exception>
        /// <exception cref="System.Net.WebException">The remote filename cannot be 
        /// resolved.-or-An error occurred while processing the request.</exception>
        /// <exception cref="System.InvalidOperationException">url is an empty 
        /// string.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Part of the 
        /// filename or directory cannot be found.</exception>
        /// <exception cref="System.UriFormatException">url is not a valid URI.</exception>
        public static object XmlFileToObject(string path, Type type)
        {
            XmlSerializer xs = new XmlSerializer(type);
            object deserializedObj = null;

            using (XmlReader reader = new XmlTextReader(path))
            {
                deserializedObj = xs.Deserialize(reader);
            }

            return deserializedObj;
        }
    }
}
