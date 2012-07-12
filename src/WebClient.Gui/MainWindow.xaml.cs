using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Finkit.ManicTime.WebClient.Resources;
using Newtonsoft.Json;

namespace Finkit.ManicTime.WebClient.Gui
{
    public partial class MainWindow
    {
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
                HomeButton.IsEnabled = CancellationTokenSource == null;
                TimelinesButton.IsEnabled = CancellationTokenSource == null;
                GetActivitiesButton.IsEnabled = CancellationTokenSource == null;
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
            Output("Getting home...");
            SendAsync((client, cancellationToken) => client.GetHomeAsync(cancellationToken));
        }

        private void TimelinesButton_OnClick(object sender, RoutedEventArgs e)
        {
            Output("Getting timelines...");
            SendAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));
        }

        private void GetActivitiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            Output("Getting timelines...");
            SendAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken))
                .ContinueWith(t =>
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
                        Output("Getting activities...");
                        SendAsync((client, cancellationToken) => client.GetActivitiesByTimelineIdAsync(window.SelectedTimeline.TimelineId, window.FromTime.Value, window.ToTime.Value.AddDays(1), cancellationToken));
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void GetTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Output("Getting tag combination list...");
            SendAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));
        }

        private void UpdateTagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Output("Getting tag combination list...");
            SendAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken))
                .ContinueWith(t =>
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
                        Output("Sending tag combination list...");
                        SendAsync((client, cancellationToken) => client.PostTagCombinationsAsync(newList, cancellationToken));
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<T> SendAsync<T>(Func<Client, CancellationToken, Task<T>> send) 
        {
            CancellationTokenSource = new CancellationTokenSource();
            string url = ServerUrlTextBox.Text;
            var client = new Client(url);
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
                            Output("Result received:\r\n{0}", JsonConvert.SerializeObject(t.Result, Formatting.Indented));
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

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CancellationTokenSource != null)
                CancellationTokenSource.Cancel();
        }

        private void ClearLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            Invoke(() =>
            {
                OutputTextBox.Clear();
                ClearLogButton.IsEnabled = false;
            });
        }

        private void DisposeCanncellationTokenSource()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
        }

        private void Output(string format, params object[] args)
        {
            Invoke(() =>
            {
                if (OutputTextBox.Text != "")
                    OutputTextBox.AppendText("\r\n");
                OutputTextBox.AppendText(string.Format(format, args));
                OutputTextBox.ScrollToEnd();
                ClearLogButton.IsEnabled = true;
            });
        }

        private void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}
