using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient
{
    public static class XmlFormatter
    {
        public static string Format(object value)
        {
            using (var stream = new MemoryStream())
            {
                var writer = XmlWriter.Create(new XmlCustomWriter(stream, Encoding.UTF8, false, true, true));
                new XmlSerializer(value.GetType()).Serialize(writer, value, NoNamespaces);
                writer.Close();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static object Parse(string value, Type type)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
            {
                return new XmlSerializer(type).Deserialize(stream);
            }
        }

        private class XmlCustomWriter : XmlTextWriter
        {
            private readonly bool _includeNamespaces;
            private readonly bool _includeHeader;

            public XmlCustomWriter(Stream stream, Encoding encoding, bool includeNamespaces, bool includeHeader, bool indent)
                : base(stream, encoding)
            {
                _includeNamespaces = includeNamespaces;
                _includeHeader = includeHeader;
                Formatting = indent ? Formatting.Indented : Formatting.None;
            }

            public override void WriteStartDocument()
            {
                if (_includeHeader)
                    base.WriteStartDocument();
            }

            public override void WriteStartElement(string prefix, string localName, string ns)
            {
                base.WriteStartElement(_includeNamespaces ? prefix : string.Empty, localName, _includeNamespaces ? ns : string.Empty);
            }
        }

        private static readonly XmlSerializerNamespaces NoNamespaces;

        static XmlFormatter()
        {
            NoNamespaces = new XmlSerializerNamespaces();
            NoNamespaces.Add("", "");
        }
    }
}