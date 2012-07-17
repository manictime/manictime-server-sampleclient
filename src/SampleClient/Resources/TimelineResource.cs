using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("timeline")]
    [DataContract(Name = "timeline")]
    public class TimelineResource
    {
        [XmlElement("timelineId")]
        [DataMember(Name = "timelineId")]
        public string TimelineId { get; set; }

        [XmlElement("owner")]
        [DataMember(Name = "owner", EmitDefaultValue = false)]
        public UserResource Owner { get; set; }

        [XmlElement("displayName")]
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [XmlElement("color")]
        [DataMember(Name = "color")]
        public string Color { get; set; }

        [XmlElement("updateInterval")]
        [DataMember(Name = "updateInterval")]
        public int? UpdateInterval { get; set; }
        public bool ShouldSerializeUpdateInterval() { return UpdateInterval.HasValue; }

        [XmlElement("timelineType")]
        [DataMember(Name = "timelineType")]
        public TimelineTypeResource TimelineType { get; set; }

        [XmlElement("clientName")]
        [DataMember(Name = "clientName")]
        public string ClientName { get; set; }

        [XmlElement("updateState")]
        [DataMember(Name = "updateState")]
        public UpdateStateResource UpdateState { get; set; }

        [XmlElement("changeTrackingStartTimestamp")]
        [DataMember(Name = "changeTrackingStartTimestamp", EmitDefaultValue = false)]
        public int? ChangeTrackingStartTimestamp { get; set; }
        public bool ShouldSerializeChangeTrackingStartTimestamp() { return ChangeTrackingStartTimestamp.HasValue; }

        [XmlElement("filterHash")]
        [DataMember(Name = "filterHash", EmitDefaultValue = false)]
        public string FilterHash { get; set; }
        public bool ShouldSerializeFilterHash() { return FilterHash != null; }

        [XmlElement("updatedAfterTimestamp")]
        [DataMember(Name = "updatedAfterTimestamp", EmitDefaultValue = false)]
        public int? UpdatedAfterTimestamp { get; set; }
        public bool ShouldSerializeUpdatedAfterTimestamp() { return UpdatedAfterTimestamp.HasValue; }

        [XmlArray("activities")]
        [XmlArrayItem("activity")]
        [DataMember(Name = "activities")]
        public ActivityResource[] Activities { get; set; }

        [XmlArray("deletedActivityIds")]
        [XmlArrayItem("activityId")]
        [DataMember(Name = "deletedActivityIds")]
        public string[] DeletedActivityIds { get; set; }

        [XmlArray("groups")]
        [XmlArrayItem("group")]
        [DataMember(Name = "groups")]
        public GroupResource[] Groups { get; set; }

        [XmlArray("deletedGroupIds")]
        [XmlArrayItem("groupId")]
        [DataMember(Name = "deletedGroupIds")]
        public string[] DeletedGroupIds { get; set; }

        [XmlArray("groupLists")]
        [XmlArrayItem("groupList")]
        [DataMember(Name = "groupLists")]
        public GroupListResource[] GroupLists { get; set; }

        [XmlArray("deletedGroupListIds")]
        [XmlArrayItem("groupListId")]
        [DataMember(Name = "deletedGroupListIds")]
        public string[] DeletedGroupListIds { get; set; }

        [XmlArray("folders")]
        [XmlArrayItem("folder")]
        [DataMember(Name = "folders")]
        public FolderResource[] Folders { get; set; }

        [XmlArray("deletedFolderIds")]
        [XmlArrayItem("folderId")]
        [DataMember(Name = "deletedFolderIds")]
        public string[] DeletedFolderIds { get; set; }

        [XmlElement("link")]
        [DataMember(Name = "links")]
        public LinkResource[] Links { get; set; }
    }
}
