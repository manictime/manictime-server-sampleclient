using System.Net;
using System.Windows;

namespace Finkit.ManicTime.Server.SampleClient.Ui
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
                SizeToContent = SizeToContent.WidthAndHeight,
                _clientSettings = clientSettings
            };
            window.UpdateForm();
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

        private void UpdateForm()
        {
            if (_clientSettings.Credentials == null)
            {
                CurrentUserRadioButton.IsChecked = true;
            }
            else
            {
                FollowingUserRadioButton.IsChecked = true;
                UsernameTextBox.Text = _clientSettings.Credentials.UserName;
                PasswordTextBox.Password = _clientSettings.Credentials.Password;
                DomainTextBox.Text = _clientSettings.Credentials.Domain;
            }
        }

        private void UpdateSettings()
        {
            _clientSettings = new ClientSettings(CurrentUserRadioButton.IsChecked == true
                    ? null
                    : new NetworkCredential(UsernameTextBox.Text, PasswordTextBox.Password, DomainTextBox.Text));
        }
    }
}