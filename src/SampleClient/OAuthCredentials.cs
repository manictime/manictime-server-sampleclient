using System;
using System.Collections.Generic;
using System.Net;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class OAuthCredentials : IServerHttpCredentials
    {
        public ICredentials Credentials { get; } = null;
        public KeyValuePair<string, string>[] Headers { get; }
        public string AccessToken { get; }

        public OAuthCredentials(string accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));
            Headers = new[]
            {
                new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}"),
            };
            AccessToken = accessToken;
        }
    }
}