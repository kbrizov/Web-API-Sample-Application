using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ForumDb.IntegrationTests
{
    internal static class XmlConvert
    {
        internal static string SerializeObject(object objectToSerialize)
        {
            // Create the stream.
            MemoryStream memoryStream = new MemoryStream();

            XmlSerializer xmlSerializer = new XmlSerializer(objectToSerialize.GetType());

            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, null);
            xmlSerializer.Serialize(xmlTextWriter, objectToSerialize);

            // Rewind the Stream.
            memoryStream.Seek(0, SeekOrigin.Begin);

            XmlDocument xmlDocument = new XmlDocument();

            // load from the stream and then close it.
            xmlDocument.Load(memoryStream);
            memoryStream.Close();

            string xmlString = xmlDocument.OuterXml;
            return xmlString;
        }

        internal static T DeserializeObject<T>(string xmlString)
        {
            T outObject;
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(xmlString);

            outObject = (T)deserializer.Deserialize(stringReader);
            stringReader.Close();

            return outObject;
        }
    }
}
