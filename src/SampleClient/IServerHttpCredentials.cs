using System.Collections.Generic;
using System.Net;

namespace Finkit.ManicTime.Server.SampleClient
{
    public interface IServerHttpCredentials
    {
        ICredentials Credentials { get; }
        KeyValuePair<string, string>[] Headers { get; }
    }
}