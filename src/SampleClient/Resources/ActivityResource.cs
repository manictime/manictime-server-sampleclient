using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("activity")]
    [DataContract(Name = "activity")]
    public class ActivityResource
    {
        [XmlElement("activityId")]
        [DataMember(Name = "activityId")]
        public string ActivityId { get; set; }

        [XmlElement("displayName")]
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [XmlElement("groupId")]
        [DataMember(Name = "groupId")]
        public string GroupId { get; set; }

        [XmlElement("groupListId")]
        [DataMember(Name = "groupListId")]
        public string GroupListId { get; set; }

        [XmlElement("isActive")]
        [DataMember(Name = "isActive")]
        public bool? IsActive { get; set; }

        [XmlIgnore]
        public DateTimeOffset StartTime { get; set; }

        [XmlElement("startTime")]
        [DataMember(Name = "startTime")]
        public string StartTimeTextValue
        {
            get { return StartTime.FormatIso8601(); }
            set { StartTime = DateTimeOffset.Parse(value); }
        }

        [XmlIgnore]
        public DateTimeOffset EndTime { get; set; }

        [XmlElement("endTime")]
        [DataMember(Name = "endTime")]
        public string EndTimeTextValue
        {
            get { return EndTime.FormatIso8601(); }
            set { EndTime = DateTimeOffset.Parse(value); }
        }

        [XmlElement("textData")]
        [DataMember(Name = "textData")]
        public string TextData { get; set; }
    }
}
