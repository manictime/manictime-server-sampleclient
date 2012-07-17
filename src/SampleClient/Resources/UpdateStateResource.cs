using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("updateState")]
    [DataContract(Name = "updateState")]
    public class UpdateStateResource
    {
        [XmlElement("timelineId")]
        [DataMember(Name = "timelineId")]
        public string TimelineId { get; set; }

        [XmlElement("updateKey")]
        [DataMember(Name = "updateKey")]
        public string UpdateKey { get; set; }

        [XmlElement("updateData")]
        [DataMember(Name = "updateData")]
        public string UpdateData { get; set; }

        [XmlElement("updateTimestamp")]
        [DataMember(Name = "updateTimestamp")]
        public int UpdateTimestamp { get; set; }

        [XmlIgnore]
        public DateTimeOffset UpdateUtcTime { get; set; }

        [XmlElement("updateUtcTime")]
        [DataMember(Name = "updateUtcTime")]
        public string UpdateUtcTimeTextValue
        {
            get { return UpdateUtcTime.FormatIso8601(); }
            set { UpdateUtcTime = DateTimeOffset.Parse(value); }
        }
    }
}
