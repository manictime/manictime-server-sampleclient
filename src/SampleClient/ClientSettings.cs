using System.Net;

namespace Finkit.ManicTime.Server.SampleClient
{
    public static class MediaTypes
    {
        public const string ManicTimeJson = "application/vnd.manictime.v2+json";
    }

    public class ClientSettings
    {
        public NetworkCredential Credentials { get; private set; }

        public ClientSettings(NetworkCredential credentials = null)
        {
            Credentials = credentials;
        }
    }
}
