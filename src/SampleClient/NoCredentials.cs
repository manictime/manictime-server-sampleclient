using System.Collections.Generic;
using System.Net;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class NoCredentials : IServerHttpCredentials
    {
        private static readonly KeyValuePair<string, string>[] NoHeaders = new KeyValuePair<string, string>[0];

        public static NoCredentials Value = new NoCredentials();
        public ICredentials Credentials { get; } = null;
        public KeyValuePair<string, string>[] Headers { get; } = NoHeaders;

        private NoCredentials()
        {
        }
    }
}