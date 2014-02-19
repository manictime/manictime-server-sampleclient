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
        private static readonly Dictionary<string, Func<object, string>> SupportedMediaTypeFormatters = new Dictionary<string, Func<object, string>>
        {
            { MediaTypes.ApplicationJson, JsonFormatter.Format }, 
            { MediaTypes.ApplicationXml, XmlFormatter.Format }
        };

        private static readonly Dictionary<string, Func<string, Type, object>> SupportedMediaTypeParsers = new Dictionary<string, Func<string, Type, object>>
        {
            { MediaTypes.ApplicationJson, JsonFormatter.Parse }, 
            { MediaTypes.ApplicationXml, XmlFormatter.Parse }
        };

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
                Credentials = clientSettings.Credentials,
                UseDefaultCredentials = clientSettings.Credentials == null
            });
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

        private async Task<T> SendAsync<T>(string url, HttpMethod method, object value, CancellationToken cancellationToken)
        {
            Func<object, string> mediaTypeFormatter;
            if (!SupportedMediaTypeFormatters.TryGetValue(ClientSettings.MediaType, out mediaTypeFormatter))
                throw new InvalidOperationException("Media type not supported: " + ClientSettings.MediaType);
            HttpRequestMessage request = null;
            string requestContent = null;
            HttpResponseMessage response = null;
            string responseContent = null;
            Exception exception = null;
            object resource = null;
            try
            {
                requestContent = value == null ? null : mediaTypeFormatter(value);
                request = new HttpRequestMessage(method, url);
                if (requestContent != null)
                    request.Content = new StringContent(requestContent, Encoding.UTF8, ClientSettings.MediaType);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ClientSettings.MediaType));

                response = await _client.SendAsync(request, cancellationToken);
                responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    Func<string, Type, object> mediaTypeParser;
                    if (SupportedMediaTypeParsers.TryGetValue(response.Content.Headers.ContentType.MediaType, out mediaTypeParser))
                        resource = mediaTypeParser(responseContent, typeof(T));
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            LogSession(request, requestContent, response, responseContent, exception);
            return (T)resource;
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
    }
}
