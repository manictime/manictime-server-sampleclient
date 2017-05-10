using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Finkit.ManicTime.Server.SampleClient.Resources;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class Client : IDisposable
    {
        public ClientSettings ClientSettings { get; private set; }

        public Action<HttpSession> Log { get; set; }

        private readonly string _serverUrl;
        private readonly HttpClient _client;

        public Client(string serverUrl, ClientSettings clientSettings)
        {
            _serverUrl = serverUrl;
            ClientSettings = clientSettings;
            _client = new HttpClient(new HttpClientHandler
            {
                PreAuthenticate = true,
                Credentials = clientSettings.Credentials.Credentials
            });
        }

        public async Task<string> AuthenticateOauthAsync(string username, string password, CancellationToken cancellationToken)
        {
            ServerHttpResponse<HomeResource> challengeResponse = 
                await SendHttpAsync<HomeResource>(_serverUrl, HttpMethod.Get, null, cancellationToken).ConfigureAwait(false);
            string authenticationType = GetAuthenticationType(challengeResponse.Headers);
            if (authenticationType != AuthenticationTypes.Bearer)
                throw new InvalidOperationException("Server is not configured for ManicTime authentication.");
            string tokenUrl = challengeResponse.Resource?.Links.Url(Relations.Token);
            if (tokenUrl == null)
                throw new InvalidOperationException("Token url not found.");

            HttpContent tokenContent = new StringContent(
                $"grant_type=password&username={Uri.EscapeDataString(username ?? "")}&password={Uri.EscapeDataString(password ?? "")}",
                null, MediaTypes.FormUrlEncoded);
            ServerHttpResponse<AccessTokenResource> tokenResponse = 
                await SendHttpAsync<AccessTokenResource>(tokenUrl, HttpMethod.Post, tokenContent, cancellationToken).ConfigureAwait(false);


            if (tokenResponse.StatusCode != HttpStatusCode.OK && tokenResponse.StatusCode != HttpStatusCode.BadRequest)
                throw new InvalidOperationException($"Invalid status code received: {tokenResponse.StatusCode}");
            if (tokenResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                if (tokenResponse.Resource?.Error.Equals("invalid_grant", StringComparison.OrdinalIgnoreCase) == false)
                    throw new InvalidOperationException($"Unknown error: {tokenResponse.Resource?.Error}");
                return null;
            }
            if (tokenResponse.Resource?.Token == null)
                throw new InvalidOperationException("Token not received");
            return tokenResponse.Resource.Token;
        }

        public Task<HomeResource> GetHomeAsync(CancellationToken cancellationToken)
        {
            return SendAsync<HomeResource>(_serverUrl, HttpMethod.Get, null, cancellationToken);
        }

        public async Task<TimelinesResource> GetTimelinesAsync(CancellationToken cancellationToken)
        {
            HomeResource home = await GetHomeAsync(cancellationToken);
            string timelinesUrl = home == null ? null : home.Links.Url(Relations.Timelines);
            if (timelinesUrl != null)
                return await SendAsync<TimelinesResource>(timelinesUrl, HttpMethod.Get, null, cancellationToken);
            return null;
        }

        public async Task<TimelineResource> GetActivitiesByTimelineIdAsync(string timelineId, DateTime fromTime, DateTime toTime, CancellationToken cancellationToken)
        {
            TimelinesResource timelines = await GetTimelinesAsync(cancellationToken);
            var timeline = timelines == null || timelines.Timelines == null
                ? null
                : timelines.Timelines.SingleOrDefault(tr => tr.TimelineId == timelineId);
            string activitiesUrl = timeline == null ? null : timeline.Links.Url(Relations.Activities);
            if (activitiesUrl == null)
                return null;
            return await GetActivitiesByUrlAsync(activitiesUrl, fromTime, toTime, cancellationToken);
        }

        public Task<TimelineResource> GetActivitiesByUrlAsync(string activitiesUrl, DateTime fromTime, DateTime toTime, CancellationToken cancellationToken)
        {
            string url = new UriBuilder(activitiesUrl)
                .WithQueryParameter("fromTime", fromTime.FormatIso8601())
                .WithQueryParameter("toTime", toTime.FormatIso8601())
                .ToString();
            return SendAsync<TimelineResource>(url, HttpMethod.Get, null, cancellationToken);
        }

        public Task<TimelineResource> GetUpdatedActivitiesAsync(string updatedActivitiesUrl, CancellationToken cancellationToken)
        {
            return SendAsync<TimelineResource>(updatedActivitiesUrl, HttpMethod.Get, null, cancellationToken);
        }

        public async Task<TagCombinationListResource> GetTagCombinationsAsync(CancellationToken cancellationToken)
        {
            HomeResource home = await GetHomeAsync(cancellationToken);
            string tagCombinationListUrl = home == null ? null : home.Links.Url(Relations.TagCombinationList);
            if (tagCombinationListUrl == null)
                return null;
            return await SendAsync<TagCombinationListResource>(tagCombinationListUrl, HttpMethod.Get, null, cancellationToken);
        }

        public async Task<TagCombinationListResource> PostTagCombinationsAsync(TagCombinationListResource tagCombinationList, CancellationToken cancellationToken)
        {
            HomeResource home = await GetHomeAsync(cancellationToken);
            string tagCombinationListUrl = home == null ? null : home.Links.Url(Relations.TagCombinationList);
            if (tagCombinationListUrl == null)
                return null;
            return await SendAsync<TagCombinationListResource>(tagCombinationListUrl, HttpMethod.Post, tagCombinationList, cancellationToken);
        }

        public async Task<TimelineResource> PublishTimeline(TimelineResource timeline, CancellationToken cancellationToken)
        {
            HomeResource home = await GetHomeAsync(cancellationToken);
            string timelinesUrl = home == null ? null : home.Links.Url(Relations.Timelines);
            if (timelinesUrl == null)
                throw new InvalidOperationException("Cannot publish timeline. Timelines url not found.");

            return await SendAsync<TimelineResource>(timelinesUrl, HttpMethod.Post, timeline, cancellationToken);
        }

        public async Task<UpdateStateResource> SendActivityUpdates(string timelineId, ActivityUpdatesResource activityUpdates, CancellationToken cancellationToken)
        {
            TimelinesResource timelines = await GetTimelinesAsync(cancellationToken);
            var timeline = timelines == null || timelines.Timelines == null
                ? null
                : timelines.Timelines.SingleOrDefault(tr => tr.TimelineId == timelineId);
            if (timeline == null)
                throw new InvalidOperationException("Cannot update activities. Timeline not found.");
            string activityUpdatesUrl = timeline.Links.Url(Relations.ActivityUpdates);
            if (activityUpdatesUrl == null)
                throw new InvalidOperationException("Cannot update activities. Activity updates url not found.");
            return await SendAsync<UpdateStateResource>(activityUpdatesUrl, HttpMethod.Post, activityUpdates, cancellationToken);
        }

        private async Task<T> SendAsync<T>(string url, HttpMethod method, object value, CancellationToken cancellationToken) where T : class
        {
            var content = value == null
                ? null
                : new StringContent(JsonFormatter.Format(value), Encoding.UTF8, MediaTypes.ManicTimeJson);
            ServerHttpResponse<T> serverResponse = await SendHttpAsync<T>(url, method, content, cancellationToken).ConfigureAwait(false);
            return serverResponse?.Resource;
        }

        private async Task<ServerHttpResponse<T>> SendHttpAsync<T>(string url, HttpMethod method, HttpContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = null;
            string requestContent = null;
            HttpResponseMessage response = null;
            string responseContent = null;
            Exception exception = null;
            object resource = null;
            try
            {
                requestContent = content == null 
                    ? null 
                    : await content.ReadAsStringAsync().ConfigureAwait(false);
                request = new HttpRequestMessage(method, url) {Content = content};
                foreach (KeyValuePair<string, string> header in ClientSettings.Credentials.Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.ManicTimeJson));

                response = await _client.SendAsync(request, cancellationToken);
                responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(responseContent))
                {
                    if (response.Content.Headers.ContentType.MediaType == MediaTypes.ManicTimeJson ||
                        response.Content.Headers.ContentType.MediaType == MediaTypes.ApplicationJson)
                        resource = JsonFormatter.Parse(responseContent, typeof(T));
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            LogSession(request, requestContent, response, responseContent, exception);
            return response == null ? null : new ServerHttpResponse<T>(response.StatusCode, response.Headers, (T)resource);
        }

        private void LogSession(
            HttpRequestMessage request, string requestContent, 
            HttpResponseMessage response, string responseContent,
            Exception exception)
        {
            if (Log != null)
            {
                Log(new HttpSession(
                    request == null ? null : request.Method,
                    request == null ? null : request.RequestUri.ToString(),
                    request == null ? null : request.Headers,
                    request == null ? null : request.Content == null ? null : request.Content.Headers,
                    requestContent,
                    response == null ? (HttpStatusCode?) null : response.StatusCode,
                    response == null ? null : response.Headers,
                    response == null || response.Content == null ? null : response.Content.Headers,
                    responseContent,
                    exception));
            }
        }

        public void Dispose()
        {   
            _client.Dispose();
        }

        private static string GetAuthenticationType(HttpHeaders headers)
        {
            IEnumerable<string> authenticateValues;
            return headers.TryGetValues("WWW-Authenticate", out authenticateValues)
                ? GetAuthenticationType(authenticateValues.FirstOrDefault())
                : null;
        }

        private static string GetAuthenticationType(string wwwAuthenticateHeaderValue)
        {
            int pos = wwwAuthenticateHeaderValue.IndexOfAny(new[] { ' ', ',' });
            return pos == -1
                ? wwwAuthenticateHeaderValue.Trim().ToLowerInvariant()
                : wwwAuthenticateHeaderValue.Substring(0, pos - 1).Trim().ToLowerInvariant();
        }
    }
}
