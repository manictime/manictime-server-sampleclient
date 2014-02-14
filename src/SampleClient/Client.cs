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

        private async Task<T> SendAsync<T>(string url, HttpMethod method, object value, CancellationToken cancellationToken)
        {
            MediaTypeFormatter mediaTypeFormatter;
            if (!SupportedMediaTypeFormatters.TryGetValue(ClientSettings.MediaType, out mediaTypeFormatter))
                throw new InvalidOperationException("Media type not supported: " + ClientSettings.MediaType);
            var request = new HttpRequestMessage(method, url)
            {
                Content = value == null ? null : new ObjectContent(value.GetType(), value, mediaTypeFormatter)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ClientSettings.MediaType));
            HttpResponseMessage responseMessage = await _client.SendAsync(request, cancellationToken);
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsAsync<T>(SupportedMediaTypeFormatters.Values.ToArray());
        }

        public void Dispose()
        {   
            _client.Dispose();
        }
    }
}
