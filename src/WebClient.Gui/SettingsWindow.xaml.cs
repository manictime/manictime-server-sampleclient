using System.Windows;

namespace Finkit.ManicTime.WebClient.Gui
{
    public partial class SettingsWindow
    {
        private ClientSettings _clientSettings;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public static ClientSettings Show(Window owner, ClientSettings clientSettings)
        {
            var window = new SettingsWindow
            {
                Owner = owner, 
                WindowStartupLocation = WindowStartupLocation.CenterOwner, 
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.UpdateForm(clientSettings);
            return window.ShowDialog() == true ? window._clientSettings : null;
        }



        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettings();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateForm(ClientSettings clientSettings)
        {
            if (clientSettings.MediaType == MediaTypes.ApplicationJson)
                JsonMessageFormatRadioButton.IsChecked = true;
            else
                XmlMessageFormatRadioButton.IsChecked = true;
        }

        private void UpdateSettings()
        {
            _clientSettings = new ClientSettings
            {
                MediaType = JsonMessageFormatRadioButton.IsChecked == true
                    ? MediaTypes.ApplicationJson
                    : MediaTypes.ApplicationXml
            };
        }
    }
}