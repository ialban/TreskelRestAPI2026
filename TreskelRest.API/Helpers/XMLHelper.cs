using System.Xml.Serialization;

namespace TreskelRest.API.Helpers
{
    public static class XMLHelper
    {
        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("El XML está vacío.");

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using StringReader reader = new StringReader(xml);

            return (T)serializer.Deserialize(reader)!;
        }
    }
}
