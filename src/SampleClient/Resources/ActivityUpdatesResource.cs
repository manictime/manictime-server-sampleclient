using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [XmlRoot("activityUpdates")]
    [DataContract(Name = "activityUpdates")]
    public class ActivityUpdatesResource
    {
        [XmlElement("clientName")]
        [DataMember(Name = "clientName")]
        public string ClientName { get; set; }

        [XmlElement("timelineKey")]
        [DataMember(Name = "timelineKey")]
        public string TimelineKey { get; set; }

        [XmlElement("currentUpdateKey")]
        [DataMember(Name = "currentUpdateKey")]
        public string CurrentUpdateKey { get; set; }

        [XmlElement("newUpdateKey")]
        [DataMember(Name = "newUpdateKey")]
        public string NewUpdateKey { get; set; }

        [XmlElement("updateData")]
        [DataMember(Name = "updateData")]
        public string UpdateData { get; set; }

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

        public bool IsEmpty()
        {
            return
                (Activities == null || Activities.Length == 0) &&
                (DeletedActivityIds == null || DeletedActivityIds.Length == 0) &&
                (Groups == null || Groups.Length == 0) &&
                (DeletedGroupIds == null || DeletedGroupIds.Length == 0) &&
                (GroupLists == null || GroupLists.Length == 0) &&
                (DeletedGroupListIds == null || DeletedGroupListIds.Length == 0) &&
                (Folders == null || Folders.Length == 0) &&
                (DeletedFolderIds == null || DeletedFolderIds.Length == 0);
        }
    }
}
