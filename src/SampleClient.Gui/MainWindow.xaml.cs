using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Finkit.ManicTime.Server.SampleClient.Resources;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public partial class MainWindow
    {
        private ClientSettings _clientSettings = new ClientSettings();

        private string _updatedActivitiesUrl;
        private string UpdatedActivitiesUrl
        {
            get { return _updatedActivitiesUrl; }
            set
            {
                _updatedActivitiesUrl = value;
                EnableControls();
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource CancellationTokenSource
        {
            get { return _cancellationTokenSource; }
            set 
            { 
                _cancellationTokenSource = value;
                EnableControls();
            }
        }

        private string SelectedTimelineId { get; set; }

        private void EnableControls()
        {
            Invoke(() =>
            {
                ServerUrlTextBox.IsEnabled = CancellationTokenSource == null;
                SettingsButton.IsEnabled = CancellationTokenSource == null;
                HomeButton.IsEnabled = CancellationTokenSource == null;
                TimelinesButton.IsEnabled = CancellationTokenSource == null;
                GetActivitiesButton.IsEnabled = CancellationTokenSource == null;
                GetUpdatedActivitiesButton.IsEnabled = CancellationTokenSource == null && !string.IsNullOrEmpty(UpdatedActivitiesUrl);
                GetTagCombinationsButton.IsEnabled = CancellationTokenSource == null;
                UpdateTagCombinationsButton.IsEnabled = CancellationTokenSource == null;
                PublishTimelineButton.IsEnabled = CancellationTokenSource == null;
                SendActivityUpdatesButton.IsEnabled = CancellationTokenSource == null;
                CancelButton.IsEnabled = CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested;
            });
        }

        public MainWindow()
        {
            InitializeComponent();
            EnableControls();
        }

        private async void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            await ExecuteAsync((client, cancellationToken) => client.GetHomeAsync(cancellationToken));
        }

        private async void TimelinesButton_OnClick(object sender, RoutedEventArgs e)
        {
            await ExecuteAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));
        }

        private async void GetActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            TimelinesResource timelines = await ExecuteAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));

            if (timelines == null)
                return;
            if (timelines.Timelines == null || timelines.Timelines.Length == 0)
            {
                return;
            }
            var window = new TimelinePickerWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Timelines = timelines.Timelines,
                SelectedTimeline = timelines.Timelines.SingleOrDefault(t => t.TimelineId == SelectedTimelineId) ??  timelines.Timelines.FirstOrDefault(),
                FromTime = DateTime.Today,
                ToTime = DateTime.Today,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            if (window.ShowDialog() == true && window.FromTime != null && window.ToTime != null)
            {
                SelectedTimelineId = window.SelectedTimeline.TimelineId;
                TimelineResource timeline = await ExecuteAsync((client, cancellationToken) => client.GetActivitiesByTimelineIdAsync(
                    SelectedTimelineId, window.FromTime.Value, window.ToTime.Value.AddDays(1), cancellationToken));
                RefreshUpdatedActivitiesUrl(timeline);
            }
        }

        private async void GetUpdatedActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            TimelineResource timeline = 
                await ExecuteAsync((client, cancellationToken) => client.GetUpdatedActivitiesAsync(_updatedActivitiesUrl, cancellationToken));
            RefreshUpdatedActivitiesUrl(timeline);
        }

        private void RefreshUpdatedActivitiesUrl(TimelineResource result)
        {
            UpdatedActivitiesUrl = result == null ? null : result.Links.Url(Relations.UpdatedActivities);
        }

        private async void GetTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            await ExecuteAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));
        }

        private async void UpdateTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            TagCombinationListResource combinations = 
                await ExecuteAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));

            if (combinations == null)
                return;
            var window = new TagCombinationsEditWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                TagCombinations = combinations.TagCombinations == null ? "" : string.Join("\r\n", combinations.TagCombinations)
            };
            if (window.ShowDialog() == true)
            {
                var newList = new TagCombinationListResource
                {
                    TagCombinations = window.TagCombinations.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                };
                await ExecuteAsync((client, cancellationToken) => client.PostTagCombinationsAsync(newList, cancellationToken));
            }
        }

        private async Task<T> ExecuteAsync<T>(Func<Client, CancellationToken, Task<T>> send) where T : class
        {
            try
            {
                Log(null);
                CancellationTokenSource = new CancellationTokenSource();
                string url = ServerUrlTextBox.Text;
                var client = new Client(url, _clientSettings) { Log = Log };
                try
                {
                    return await send(client, CancellationTokenSource.Token);
                }
                finally
                {
                    client.Dispose();
                    DisposeCanncellationTokenSource();
                }
            }
            catch (Exception ex)
            {
                if (ErrorTextBox.Text == string.Empty)
                    ErrorTextBox.Text = ex.ToString();
                DisposeCanncellationTokenSource();
            }
            return default(T);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                EnableControls();
            }
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _clientSettings = SettingsWindow.Show(this, _clientSettings) ?? _clientSettings;
        }

        private async void PublishTimelineButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new PublishTimelineWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                TimelineName = "My test timeline",
                TimelineKey = Guid.NewGuid().ToString(),
                TimelineType = "Test/MyTestTimelineType",
                GenericTimelineTypes = new[] { "ManicTime/Generic/Group", "ManicTime/Generic/GroupList" },
                SelectedGenericTimelineType = "ManicTime/Generic/Group",
                ClientName = Environment.MachineName,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            if (window.ShowDialog() == true)
            {
                var timeline = new TimelineResource
                {
                    DisplayName = window.TimelineName,
                    TimelineType = new TimelineTypeResource
                    {
                        TypeName = window.TimelineType,
                        GenericTypeName = window.SelectedGenericTimelineType,
                    },
                    ClientName = window.ClientName,
                    TimelineKey = window.TimelineKey
                };

                await ExecuteAsync((client, cancellationToken) => client.PublishTimeline(timeline, cancellationToken));
            }
        }

        private async void SendActivityUpdatesButton_OnClick(object sender, RoutedEventArgs e)
        {
            TimelinesResource timelines = await ExecuteAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));
            if (timelines == null || timelines.Timelines == null || timelines.Timelines.Length == 0)
                return;

            var window = new SendActivityUpdatesWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Timelines = timelines.Timelines,
                SelectedTimeline = timelines.Timelines.SingleOrDefault(t => t.TimelineId == SelectedTimelineId) ?? timelines.Timelines.FirstOrDefault(),
                SizeToContent = SizeToContent.WidthAndHeight
            };
            if (window.ShowDialog() == true)
            {
                SelectedTimelineId = window.SelectedTimeline.TimelineId;
                var activityUpdates = new ActivityUpdatesResource
                {
                    ClientName = window.SelectedTimeline.ClientName,
                    TimelineKey = window.SelectedTimeline.TimelineKey,
                    Activities = window.Activities,
                    DeletedActivityIds = window.DeletedActivityIds,
                    GroupLists = window.GroupLists,
                    DeletedGroupListIds = window.DeletedGroupListIds,
                    Groups = window.Groups,
                    DeletedGroupIds = window.DeletedGroupIds
                };
                await ExecuteAsync((client, cancellationToken) => client.SendActivityUpdates(SelectedTimelineId, activityUpdates, cancellationToken));
            }
        }

        private void DisposeCanncellationTokenSource()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
        }

        private void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerUrlTextBox.Text = string.Format("http://{0}:8080", Environment.MachineName.ToLower());
            ServerUrlTextBox.Focus();
            ServerUrlTextBox.SelectionStart = 7;
            ServerUrlTextBox.SelectionLength = ServerUrlTextBox.Text.Length - 7;
        }

        private void Log(HttpSession session)
        {
            RequestUrlTextBox.Text = session == null 
                ? string.Empty 
                : session.RequestMethod + " " + session.RequestUrl;
            RequestHeadersTextBox.Text = session == null 
                ? string.Empty 
                : string.Format("{0}\r\n{1}", 
                    FormatHeaders(session.RequestHeaders, "Accept"),
                    FormatHeaders(session.RequestContentHeaders, "Content-Type")).Trim();
            RequestContentTextBox.Text = session == null 
                ? string.Empty 
                : session.RequestContent;

            ResponseStatusCodeTextBox.Text = session == null || session.ResponseStatusCode == null
                ? string.Empty
                : (int)session.ResponseStatusCode + " " + session.ResponseStatusCode;
            ResponseHeadersTextBox.Text = session == null || session.ResponseHeaders == null 
                ? string.Empty
                : FormatHeaders(session.ResponseContentHeaders, "Content-Type");
            ResponseContentTextBox.Text = session == null || string.IsNullOrEmpty(session.ResponseContent)
                ? string.Empty 
                : FormatResource(session.ResponseContent, session.ResponseContentHeaders.ContentType.MediaType);

            ErrorTextBox.Text = session == null || session.Exception == null 
                ? string.Empty 
                : session.Exception.ToString();
        }

        private string FormatHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, params string[] keys)
        {
            if (headers == null)
                return string.Empty;
            return string.Join(Environment.NewLine,
                headers.Where(kv => keys.Contains(kv.Key)).Select(kv => string.Format("{0}: {1}", kv.Key, string.Join(", ", kv.Value))));
        }

        private string FormatResource(string value, string contentType)
        {
            MediaTypeHeaderValue mediaTypeHeaderValue;
            if (MediaTypeHeaderValue.TryParse(contentType, out mediaTypeHeaderValue))
            {
                if (mediaTypeHeaderValue.MediaType == MediaTypes.ApplicationJson)
                    return ResultFormatter.FormatJson(value);
                if (mediaTypeHeaderValue.MediaType == MediaTypes.ApplicationXml)
                    return ResultFormatter.FormatXml(value);
            }
            return value;
        }
    }
}