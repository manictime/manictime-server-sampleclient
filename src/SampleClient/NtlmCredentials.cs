using System;
using System.Collections.Generic;
using System.Net;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class NtlmCredentials : IServerHttpCredentials
    {
        private static readonly KeyValuePair<string, string>[] NoHeaders = new KeyValuePair<string, string>[0];

        public static NtlmCredentials Default = new NtlmCredentials(CredentialCache.DefaultCredentials);
        public ICredentials Credentials { get; }
        public KeyValuePair<string, string>[] Headers { get; } = NoHeaders;

        public NtlmCredentials(ICredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));
            Credentials = credentials;
        }
    }
}