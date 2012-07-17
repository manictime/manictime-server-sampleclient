using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [DataContract(Name = "timelineType")]
    public class TimelineTypeResource
    {
        [XmlElement("typeName")]
        [DataMember(Name = "typeName")]
        public string TypeName { get; set; }

        [XmlElement("genericTypeName")]
        [DataMember(Name = "genericTypeName")]
        public string GenericTypeName { get; set; }
    }
}
