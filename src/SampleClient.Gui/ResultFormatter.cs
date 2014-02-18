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
    }
}
