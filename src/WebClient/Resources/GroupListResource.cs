using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("groupList")]
    [DataContract(Name = "groupList")]
    public class GroupListResource
    {
        [XmlElement("groupListId")]
        [DataMember(Name = "groupListId")]
        public string GroupListId { get; set; }

        [XmlElement("displayKey")]
        [DataMember(Name = "displayKey")]
        public string DisplayKey { get; set; }

        [XmlElement("color")]
        [DataMember(Name = "color")]
        public int? Color { get; set; }

        [XmlArray("groupIds")]
        [XmlArrayItem("groupId")]
        [DataMember(Name = "groupIds")]
        public string[] GroupIds { get; set; }
    }
}
