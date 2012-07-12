using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
                TagCombinationsButton.IsEnabled = CancellationTokenSource == null;
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
            GetAsync((client, cancellationToken) => client.GetHomeAsync(cancellationToken));
        }

        private void TimelinesButton_OnClick(object sender, RoutedEventArgs e)
        {
            GetAsync((client, cancellationToken) => client.GetTimelinesAsync(cancellationToken));
        }

        private void TagCombinationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            GetAsync((client, cancellationToken) => client.GetTagCombinationsAsync(cancellationToken));
        }

        private void GetAsync<T>(Func<Client, CancellationToken, Task<T>> get)
        {
            if (CancellationTokenSource != null)
                return;
            CancellationTokenSource = new CancellationTokenSource();
            string url = ServerUrlTextBox.Text;
            Output("Getting {0} from {1}...", typeof(T).Name, url);
            var client = new Client(url);
            get(client, CancellationTokenSource.Token)
                .ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                            Output(t.Exception.ToString());
                        else if (t.Status == TaskStatus.Canceled)
                            Output("Canceled.");
                        else
                            Output("Result received:\r\n{0}", JsonConvert.SerializeObject(t.Result, Formatting.Indented));
                    }
                    finally
                    {
                        client.Dispose();
                        DisposeCanncellationTokenSource();
                    }
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
