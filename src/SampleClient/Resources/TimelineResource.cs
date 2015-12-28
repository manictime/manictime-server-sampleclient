namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public class TimelineResource
    {
        public string TimelineId { get; set; }
        public string Name { get; set; }
        public UserResource Owner { get; set; }
        public int? UpdateInterval { get; set; }
        public TimelineTypeResource TimelineType { get; set; }
        public string DeviceDisplayName { get; set; }
        public ClientEnvironmentResource ClientEnvironment { get; set; }
        public string TimelineKey { get; set; }
        public UpdateStateResource UpdateState { get; set; }
        public int? ChangeTrackingStartTimestamp { get; set; }
        public int? UpdatedAfterTimestamp { get; set; }
        public ActivityResource[] Activities { get; set; }
        public string[] DeletedActivityIds { get; set; }
        public GroupResource[] Groups { get; set; }
        public string[] DeletedGroupIds { get; set; }
        public GroupListResource[] GroupLists { get; set; }
        public string[] DeletedGroupListIds { get; set; }
        public FolderResource[] Folders { get; set; }
        public string[] DeletedFolderIds { get; set; }
        public LinkResource[] Links { get; set; }
    }
}
