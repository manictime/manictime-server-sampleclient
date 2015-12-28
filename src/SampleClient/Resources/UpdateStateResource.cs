using System;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public class UpdateStateResource
    {
        public string TimelineId { get; set; }
        public string UpdateKey { get; set; }
        public string UpdateData { get; set; }
        public int UpdateTimestamp { get; set; }
        public DateTimeOffset UpdateUtcTime { get; set; }
        public bool IsUpdating { get; set; }
    }
}
