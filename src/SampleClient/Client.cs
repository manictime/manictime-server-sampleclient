using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Finkit.ManicTime.Server.SampleClient.Resources;

namespace Finkit.ManicTime.Server.SampleClient
{
    public class Client : IDisposable
    {
        private static readonly Dictionary<string, MediaTypeFormatter> SupportedMediaTypeFormatters = new Dictionary<string, MediaTypeFormatter>
        {
            { MediaTypes.ApplicationJson, new JsonMediaTypeFormatter() }, 
            { MediaTypes.ApplicationXml, new XmlMediaTypeFormatter { UseXmlSerializer = true } }
        };

        public ClientSettings ClientSettings { get; private set; }

        private readonly string _serverUrl;
        private readonly HttpClient _client;

        public Client(string serverUrl) : this(serverUrl, new ClientSettings())
        {
        }

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

        public Task<HomeResource> GetHomeAsync()
        {
            return GetHomeAsync(CancellationToken.None);
        }

        public Task<HomeResource> GetHomeAsync(CancellationToken cancellationToken)
        {
            return GetAsync<HomeResource>(_serverUrl, cancellationToken);
        }

        public Task<TimelinesResource> GetTimelinesAsync()
        {
            return GetTimelinesAsync(CancellationToken.None);
        }

        public Task<TimelinesResource> GetTimelinesAsync(CancellationToken cancellationToken)
        {
            return GetHomeAsync(cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string timelinesUrl = t.Result == null ? null : t.Result.Links.Url(Relations.Timelines);
                    if (timelinesUrl != null)
                        return GetAsync<TimelinesResource>(timelinesUrl, cancellationToken);
                    return null;
                }, cancellationToken)
                .Unwrap();
        }

        public Task<TimelineResource> GetActivitiesByTimelineIdAsync(string timelineId, DateTime fromTime, DateTime toTime)
        {
            return GetActivitiesByTimelineIdAsync(timelineId, fromTime, toTime, CancellationToken.None);
        }

        public Task<TimelineResource> GetActivitiesByTimelineIdAsync(string timelineId, DateTime fromTime, DateTime toTime, CancellationToken cancellationToken)
        {
            return GetTimelinesAsync(cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var timeline = t.Result == null || t.Result.Timelines == null
                        ? null
                        : t.Result.Timelines.SingleOrDefault(tr => tr.TimelineId == timelineId);
                    string activitiesUrl = timeline == null ? null : timeline.Links.Url(Relations.Activities);
                    if (activitiesUrl != null)
                        return GetActivitiesByUrlAsync(activitiesUrl, fromTime, toTime, cancellationToken);
                    return null;
                }, cancellationToken)
                .Unwrap();
        }

        public Task<TimelineResource> GetActivitiesByUrlAsync(string activitiesUrl, DateTime fromTime, DateTime toTime)
        {
            return GetActivitiesByUrlAsync(activitiesUrl, fromTime, toTime, CancellationToken.None);
        }

        public Task<TimelineResource> GetActivitiesByUrlAsync(string activitiesUrl, DateTime fromTime, DateTime toTime, CancellationToken cancellationToken)
        {
            string url = new UriBuilder(activitiesUrl)
                .WithQueryParameter("fromTime", fromTime.FormatIso8601())
                .WithQueryParameter("toTime", toTime.FormatIso8601())
                .ToString();
            return GetAsync<TimelineResource>(url, cancellationToken);
        }

        public Task<TimelineResource> GetUpdatedActivities(string updatedActivitiesUrl)
        {
            return GetAsync<TimelineResource>(updatedActivitiesUrl, CancellationToken.None);
        }

        public Task<TimelineResource> GetUpdatedActivities(string updatedActivitiesUrl, CancellationToken cancellationToken)
        {
            return GetAsync<TimelineResource>(updatedActivitiesUrl, cancellationToken);
        }

        public Task<TagCombinationListResource> GetTagCombinationsAsync(CancellationToken cancellationToken)
        {
            return GetHomeAsync(cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string tagCombinationListUrl = t.Result == null ? null : t.Result.Links.Url(Relations.TagCombinationList);
                    if (tagCombinationListUrl != null)
                        return GetAsync<TagCombinationListResource>(tagCombinationListUrl, cancellationToken);
                    return null;
                }, cancellationToken)
                .Unwrap();
        }

        public Task<TagCombinationListResource> PostTagCombinationsAsync(TagCombinationListResource tagCombinationList, CancellationToken cancellationToken)
        {
            return GetHomeAsync(cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string tagCombinationListUrl = t.Result == null ? null : t.Result.Links.Url(Relations.TagCombinationList);
                    if (tagCombinationListUrl != null)
                        return PostAsync<TagCombinationListResource>(tagCombinationListUrl, tagCombinationList, cancellationToken);
                    return null;
                }, cancellationToken)
                .Unwrap();
        }

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(url, CancellationToken.None);
        }

        public Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            return SendAsync<T>(url, HttpMethod.Get, null, cancellationToken);
        }

        public Task<T> PostAsync<T>(string url, object value)
        {
            return PostAsync<T>(url, value, CancellationToken.None);
        }

        public Task<T> PostAsync<T>(string url, object value, CancellationToken cancellationToken)
        {
            return SendAsync<T>(url, HttpMethod.Post, value, cancellationToken);
        }

        private Task<T> SendAsync<T>(string url, HttpMethod method, object value, CancellationToken cancellationToken)
        {
            MediaTypeFormatter mediaTypeFormatter;
            if (!SupportedMediaTypeFormatters.TryGetValue(ClientSettings.MediaType, out mediaTypeFormatter))
                throw new InvalidOperationException("Media type not supported: " + ClientSettings.MediaType);
            var request = new HttpRequestMessage(method, url)
            {
                Content = value == null ? null : new ObjectContent(value.GetType(), value, mediaTypeFormatter)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ClientSettings.MediaType));
            return _client.SendAsync(request, cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    t.Result.EnsureSuccessStatusCode();
                    return t.Result.Content
                        .ReadAsAsync<T>(SupportedMediaTypeFormatters.Values.ToArray())
                        .ContinueWith(t1 => t1.Result, cancellationToken);
                }, cancellationToken)
                .Unwrap();
        }

        public void Dispose()
        {   
            _client.Dispose();
        }
    }
}
