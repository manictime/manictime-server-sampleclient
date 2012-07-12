using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.WebClient.Resources
{
    [Serializable]
    [XmlRoot("group")]
    [DataContract(Name = "group")]
    public class GroupResource
    {
        [XmlElement("groupId")]
        [DataMember(Name = "groupId")]
        public string GroupId { get; set; }

        [XmlElement("displayName")]
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [XmlElement("color")]
        [DataMember(Name = "color")]
        public string Color { get; set; }

        [XmlElement("skipColor")]
        [DataMember(Name = "skipColor")]
        public bool SkipColor { get; set; }

        [XmlElement("icon16")]
        [DataMember(Name = "icon16")]
        public byte[] Icon16 { get; set; }

        [XmlElement("icon32")]
        [DataMember(Name = "icon32")]
        public byte[] Icon32 { get; set; }

        [XmlElement("folderId")]
        [DataMember(Name = "folderId")]
        public string FolderId { get; set; }

        [XmlElement("textData")]
        [DataMember(Name = "textData")]
        public string TextData { get; set; }

        [XmlElement("displayKey")]
        [DataMember(Name = "displayKey")]
        public string DisplayKey { get; set; }
    }
}
