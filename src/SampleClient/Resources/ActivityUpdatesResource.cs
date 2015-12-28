namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public class ActivityUpdatesResource
    {
        public ClientEnvironmentResource ClientEnvironment { get; set; }
        public string TimelineKey { get; set; }
        public string CurrentUpdateKey { get; set; }
        public string NewUpdateKey { get; set; }
        public string UpdateData { get; set; }
        public ActivityResource[] Activities { get; set; }
        public string[] DeletedActivityIds { get; set; }
        public GroupResource[] Groups { get; set; }
        public string[] DeletedGroupIds { get; set; }
        public GroupListResource[] GroupLists { get; set; }
        public string[] DeletedGroupListIds { get; set; }
        public FolderResource[] Folders { get; set; }
        public string[] DeletedFolderIds { get; set; }
    }
}
