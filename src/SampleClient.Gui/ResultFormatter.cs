using System;
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

        public static string FormatJson(string value)
        {
            if (value == null)
                return null;
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(value), Formatting.Indented);
        }

        public static String FormatXml(String value)
        {
            using (var mStream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(mStream, Encoding.Unicode) { Formatting = System.Xml.Formatting.Indented})
                {
                    var document = new XmlDocument();
                    try
                    {
                        document.LoadXml(value);

                        document.WriteContentTo(writer);
                        writer.Flush();
                        mStream.Flush();

                        mStream.Position = 0;
                        var sReader = new StreamReader(mStream);

                        return sReader.ReadToEnd();
                    }
                    catch (XmlException)
                    {
                    }
                }
            }
            return value;
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
