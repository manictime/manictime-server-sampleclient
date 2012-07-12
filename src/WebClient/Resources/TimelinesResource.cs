using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.WebClient.Resources
{
    [Serializable]
    [XmlRoot("timelines")]
    [DataContract(Name = "timelines")]
    public class TimelinesResource
    {
        [XmlElement("timeline")]
        [DataMember(Name = "timelines")]
        public TimelineResource[] Timelines { get; set; }
    }
}
