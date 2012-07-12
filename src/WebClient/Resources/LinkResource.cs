using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.WebClient.Resources
{
    [Serializable]
    [DataContract(Name = "link")]
    public class LinkResource
    {
        [XmlAttribute("rel")]
        [DataMember(Name = "rel")]
        public string Rel { get; set; }

        [XmlAttribute("href")]
        [DataMember(Name = "href")]
        public string Href { get; set; }
    }
}
