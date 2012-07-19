using System.Net;

namespace Finkit.ManicTime.Server.SampleClient
{
    public static class MediaTypes
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
    }

    public class ClientSettings
    {
        public string MediaType { get; private set; }
        public NetworkCredential Credentials { get; private set; }

        public ClientSettings(string mediaType = MediaTypes.ApplicationJson, NetworkCredential credentials = null)
        {
            MediaType = mediaType;
            Credentials = credentials;
        }
    }
}
