using System;
using System.Linq;
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
                CancelButton.IsEnabled = CancellationTokenSource != null;
            });
        }

        public MainWindow()
        {
            InitializeComponent();
            EnableControls();
        }

        private async void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting home...");
            await ExecuteAsync((client, cancellationToken) => client.GetHomeAsync(cancellationToken));
        }

        private async void TimelinesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting timelines...");
            await ExecuteAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));
        }

        private async void GetActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting timelines...");
            TimelinesResource timelines = await ExecuteAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));

            if (timelines == null)
                return;
            if (timelines.Timelines == null || timelines.Timelines.Length == 0)
            {
                Output("No timelines");
                return;
            }
            var window = new TimelinePickerWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Timelines = timelines.Timelines,
                SelectedTimeline = timelines.Timelines.FirstOrDefault(),
                FromTime = DateTime.Today,
                ToTime = DateTime.Today,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            if (window.ShowDialog() == true && window.FromTime != null && window.ToTime != null)
            {
                ClearOutput();
                Output("Getting activities...");
                TimelineResource timeline = await ExecuteAsync((client, cancellationToken) => client.GetActivitiesByTimelineIdAsync(
                    window.SelectedTimeline.TimelineId, window.FromTime.Value, window.ToTime.Value.AddDays(1), cancellationToken));
                RefreshUpdatedActivitiesUrl(timeline);
            }
        }

        private async void GetUpdatedActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting updated activities...");
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
            ClearOutput();
            Output("Getting tag combination list...");
            await ExecuteAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));
        }

        private async void UpdateTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting tag combination list...");
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
                ClearOutput();
                Output("Sending tag combination list...");
                await ExecuteAsync((client, cancellationToken) => client.PostTagCombinationsAsync(newList, cancellationToken));
            }
        }

        private async Task<T> ExecuteAsync<T>(Func<Client, CancellationToken, Task<T>> send) 
        {
            try
            {
                CancellationTokenSource = new CancellationTokenSource();
                string url = ServerUrlTextBox.Text;
                var client = new Client(url, _clientSettings);
                try
                {
                    T result = await send(client, CancellationTokenSource.Token);
                    Output("Result received:\r\n{0}", _clientSettings.MediaType == MediaTypes.ApplicationJson
                        ? ResultFormatter.FormatAsJson(result)
                        : ResultFormatter.FormatAsXml(result));
                    return result;
                }
                catch (OperationCanceledException)
                {
                    Output("Canceled.");
                }
                finally
                {
                    client.Dispose();
                    DisposeCanncellationTokenSource();
                }
            }
            catch (Exception ex)
            {
                Output(ex.ToString());
                DisposeCanncellationTokenSource();
            }
            return default(T);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                Output("Canceling...");
            }
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _clientSettings = SettingsWindow.Show(this, _clientSettings) ?? _clientSettings;
        }

        private void DisposeCanncellationTokenSource()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
        }

        private void ClearOutput()
        {
            Invoke(() => OutputTextBox.Clear());
        }

        private void Output(string format, params object[] args)
        {
            Invoke(() =>
            {
                if (OutputTextBox.Text != "")
                    OutputTextBox.AppendText("\r\n");
                OutputTextBox.AppendText(string.Format(format, args));
                OutputTextBox.ScrollToEnd();
            });
        }

        private void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerUrlTextBox.Focus();
            ServerUrlTextBox.CaretIndex = ServerUrlTextBox.Text.Length;
        }
    }
}