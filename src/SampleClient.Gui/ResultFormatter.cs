using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public static class ResultFormatter
    {
        public static string FormatAsJson(object value)
        {
            if (value == null)
                return null;
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static string FormatAsXml(object value)
        {
            if (value == null)
                return null;

            var sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                var noNamespaces = new XmlSerializerNamespaces();
                noNamespaces.Add("", "");
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, noNamespaces);
            }
            return sb.ToString();
        }
    }
}
