using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.WebClient.Resources
{
    [Serializable]
    [XmlRoot("folder")]
    [DataContract(Name = "folder")]
    public class FolderResource
    {
        [XmlElement("folderId")]
        [DataMember(Name = "folderId")]
        public string FolderId { get; set; }

        [XmlElement("displayName")]
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [XmlElement("color")]
        [DataMember(Name = "color")]
        public string Color { get; set; }

        [XmlElement("displayKey")]
        [DataMember(Name = "displayKey")]
        public string DisplayKey { get; set; }
    }
}
