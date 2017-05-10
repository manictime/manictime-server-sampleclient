using System.Net;
using System.Net.Http.Headers;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class ServerHttpResponse<TResource>
    {
        public HttpStatusCode StatusCode { get; }
        public HttpHeaders Headers { get; }
        public TResource Resource { get; }

        public ServerHttpResponse(HttpStatusCode statusCode, HttpHeaders headers, TResource resource)
        {
            StatusCode = statusCode;
            Headers = headers;
            Resource = resource;
        }
    }
}