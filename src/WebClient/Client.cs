using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finkit.ManicTime.WebClient.Resources;

namespace Finkit.ManicTime.WebClient
{
    public class Client : IDisposable
    {
        private readonly string _serverUrl;
        private readonly HttpClient _client;

        public Client(string serverUrl)
        {
            _serverUrl = serverUrl;
            _client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true, PreAuthenticate = true });
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
            return _client
                .GetAsync(new Uri(url), cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    t.Result.EnsureSuccessStatusCode();
                    return t.Result.Content
                        .ReadAsAsync<T>()
                        .ContinueWith(t1 => t1.Result, cancellationToken);
                }, cancellationToken)
                .Unwrap();
        }

        public Task<T> PostAsync<T>(string url, object value, CancellationToken cancellationToken)
        {
            return _client
                .PostAsJsonAsync(url, value, cancellationToken)
                .ContinueWith(t =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    t.Result.EnsureSuccessStatusCode();
                    return t.Result.Content
                        .ReadAsAsync<T>()
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
