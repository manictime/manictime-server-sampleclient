using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Finkit.ManicTime.Server.SampleClient.Resources;
using Newtonsoft.Json;

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

        private void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting home...");
            SendAsync((client, cancellationToken) => client.GetHomeAsync(cancellationToken));
        }

        private void TimelinesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting timelines...");
            SendAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));
        }

        private void GetActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting timelines...");
            Task<TimelinesResource> task = SendAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));

            if (task == null)
                return;

            task.ContinueWith(t =>
                {
                    TimelinesResource timelines = t.Result;
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
                    if (window.ShowDialog() == true)
                    {
                        ClearOutput();
                        Output("Getting activities...");
                        SendAsync(
                            (client, cancellationToken) =>
                                client.GetActivitiesByTimelineIdAsync(window.SelectedTimeline.TimelineId, window.FromTime.Value,
                                                                      window.ToTime.Value.AddDays(1), cancellationToken))
                            .ContinueWith(t1 => RefreshUpdatedActivitiesUrl(t1.Result), TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void GetUpdatedActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting updated activities...");
            SendAsync((client, cancellationToken) => client.GetUpdatedActivities(_updatedActivitiesUrl, cancellationToken))
                .ContinueWith(t => RefreshUpdatedActivitiesUrl(t.Result));

        }

        private void RefreshUpdatedActivitiesUrl(TimelineResource result)
        {
            UpdatedActivitiesUrl = result == null ? null : result.Links.Url(Relations.UpdatedActivities);
        }

        private void GetTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting tag combination list...");
            SendAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));
        }

        private void UpdateTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearOutput();
            Output("Getting tag combination list...");
            Task<TagCombinationListResource> task = SendAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));

            if (task == null)
                return;
            task.ContinueWith(t =>
                {
                    TagCombinationListResource combinations = t.Result;
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
                            TagCombinations = window.TagCombinations.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                        };
                        ClearOutput();
                        Output("Sending tag combination list...");
                        SendAsync((client, cancellationToken) => client.PostTagCombinationsAsync(newList, cancellationToken));
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<T> SendAsync<T>(Func<Client, CancellationToken, Task<T>> send) 
        {
            try
            {
                CancellationTokenSource = new CancellationTokenSource();
                string url = ServerUrlTextBox.Text;
                var client = new Client(url, _clientSettings);
                return send(client, CancellationTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        try
                        {
                            if (t.Exception != null)
                                Output(t.Exception.ToString());
                            else if (t.Status == TaskStatus.Canceled)
                                Output("Canceled.");
                            else
                            {
                                Output("Result received:\r\n{0}", _clientSettings.MediaType == MediaTypes.ApplicationJson 
                                    ? ResultFormatter.FormatAsJson(t.Result)
                                    : ResultFormatter.FormatAsXml(t.Result));
                                return t.Result;
                            }

                        }
                        finally
                        {
                            client.Dispose();
                            DisposeCanncellationTokenSource();
                        }
                        return default(T);
                    });
            }
            catch (Exception ex)
            {
                Output(ex.ToString());
                DisposeCanncellationTokenSource();
                return null;
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CancellationTokenSource != null)
                CancellationTokenSource.Cancel();
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
