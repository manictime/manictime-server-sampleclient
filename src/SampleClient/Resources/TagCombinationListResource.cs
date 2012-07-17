using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("tagCombinationList")]
    [DataContract(Name = "tagCombinationList")]
    public class TagCombinationListResource
    {
        [XmlArray("tagCombinations")]
        [XmlArrayItem("tagCombination")]
        [DataMember(Name = "tagCombinations")]
        public string[] TagCombinations { get; set; }

        [XmlElement("link")]
        [DataMember(Name = "links", EmitDefaultValue = false)]
        public LinkResource[] Links { get; set; }
    }
}
