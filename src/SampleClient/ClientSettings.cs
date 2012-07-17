namespace Finkit.ManicTime.Server.SampleClient
{
    public static class MediaTypes
    {
        public static string ApplicationJson = "application/json";
        public static string ApplicationXml = "application/xml";
    }

    public class ClientSettings
    {
        public string MediaType { get; set; }

        public ClientSettings()
        {
            MediaType = MediaTypes.ApplicationJson;
        }
    }
}
