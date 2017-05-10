using System;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class ClientSettings
    {
        public IServerHttpCredentials Credentials { get; }

        public ClientSettings(IServerHttpCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));
            Credentials = credentials;
        }
    }
}
