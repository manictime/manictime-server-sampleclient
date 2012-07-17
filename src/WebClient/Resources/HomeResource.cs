using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("home", Namespace = "")]
    [DataContract(Name = "home")]
    public class HomeResource
    {
        [XmlElement("link")]
        [DataMember(Name = "links")]
        public LinkResource[] Links { get; set; }
    }
}
