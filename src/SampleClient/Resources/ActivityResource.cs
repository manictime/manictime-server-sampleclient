using System;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public class ActivityResource
    {
        public string ActivityId { get; set; }
        public string DisplayName { get; set; }
        public string GroupId { get; set; }
        public string GroupListId { get; set; }
        public bool? IsActive { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string TextData { get; set; }
    }
}
