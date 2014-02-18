using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class HttpSession
    {
        public HttpMethod RequestMethod { get; set; }
        public string RequestUrl { get; private set; }
        public HttpRequestHeaders RequestHeaders { get; private set; }
        public HttpContentHeaders RequestContentHeaders { get; private set; }
        public string RequestContent { get; private set; }
        
        public HttpStatusCode? ResponseStatusCode { get; private set; }
        public HttpHeaders ResponseHeaders { get; private set; }
        public HttpContentHeaders ResponseContentHeaders { get; private set; }
        public string ResponseContent { get; private set; }

        public Exception Exception { get; set; }

        public HttpSession(
            HttpMethod requestMethod, 
            string requestUrl,
            HttpRequestHeaders requestHeaders, 
            HttpContentHeaders requestContentHeaders, 
            string requestContent,
            HttpStatusCode? responseStatusCode, 
            HttpHeaders responseHeaders, 
            HttpContentHeaders responseContentHeaders,
            string responseContent,
            Exception exception)
        {
            RequestMethod = requestMethod;
            RequestUrl = requestUrl;
            RequestHeaders = requestHeaders;
            RequestContentHeaders = requestContentHeaders;
            RequestContent = requestContent;
            ResponseStatusCode = responseStatusCode;
            ResponseHeaders = responseHeaders;
            ResponseContentHeaders = responseContentHeaders;
            ResponseContent = responseContent;
            Exception = exception;
        }
    }
}