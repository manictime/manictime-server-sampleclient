namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public class ClientEnvironmentResource
    {
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string DatabaseId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceSystemName { get; set; }
        public string DeviceAddress { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string OperatingSystemPlatform { get; set; }
        public string DotNetVersion { get; set; }
    }
}