using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static VSCodeSnippetCreator.Core.Constants;

namespace VSCodeSnippetCreator.Core.Helpers
{
    public static class SerializeHelper
    {
        public static T Deserialize<T>(string xmlText) where T : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader stringReader = new StringReader(xmlText))
                {
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    T deserializedClass = serializer.Deserialize(xmlReader) as T;
                    return deserializedClass;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Deserialize: " + ex.Message);
                return null;
            }
        }

        public static string Serialize<T>(T classToSerialize) where T : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriterUTF8 stringWriter = new StringWriterUTF8())
                {
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Indent = true,
                        IndentChars = "\t",
                        Encoding = Encoding.UTF8
                    };
                    XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);

                    XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
                    xmlNamespaces.Add(string.Empty, Namespace.VisualStudioCodeSnippet);

                    serializer.Serialize(xmlWriter, classToSerialize, xmlNamespaces);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Serialize: " + ex.Message);
                return null;
            }
        }

        private class StringWriterUTF8 : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
