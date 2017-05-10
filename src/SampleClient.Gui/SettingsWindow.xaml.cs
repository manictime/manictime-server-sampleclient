using System.Windows;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public static SettingsWindowViewModel Show(Window owner, SettingsWindowViewModel viewModel)
        {
            var window = new SettingsWindow
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.SetViewModel(viewModel);
            return window.ShowDialog() == true ? window.GetViewModel() : null;
        }



        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SetViewModel(SettingsWindowViewModel viewModel)
        {
            CurrentUserRadioButton.IsChecked = viewModel.UserType == SettingsWindowUserType.WindowsUser && viewModel.Username == null;
            FollowingUserRadioButton.IsChecked = viewModel.UserType == SettingsWindowUserType.WindowsUser && viewModel.Username != null;
            FollowingManicTimeUserRadioButton.IsChecked = viewModel.UserType == SettingsWindowUserType.ManicTimeUser;
            UsernameTextBox.Text = viewModel.UserType == SettingsWindowUserType.WindowsUser ? viewModel.Username : "";
            PasswordTextBox.Password = viewModel.UserType == SettingsWindowUserType.WindowsUser ?  viewModel.Password : "";
            DomainTextBox.Text = viewModel.UserType == SettingsWindowUserType.WindowsUser ? viewModel.Domain : "";
            ManicTimeUsernameTextBox.Text = viewModel.UserType == SettingsWindowUserType.ManicTimeUser ? viewModel.Username : "";
            ManicTimePasswordTextBox.Password = viewModel.UserType == SettingsWindowUserType.ManicTimeUser ? viewModel.Password : "";
        }

        private SettingsWindowViewModel GetViewModel()
        {
            if (CurrentUserRadioButton.IsChecked == true)
                return new SettingsWindowViewModel(SettingsWindowUserType.WindowsUser, null, null, null);
            if (FollowingUserRadioButton.IsChecked == true)
                return new SettingsWindowViewModel(SettingsWindowUserType.WindowsUser, UsernameTextBox.Text, PasswordTextBox.Password, DomainTextBox.Text);
            return new SettingsWindowViewModel(SettingsWindowUserType.ManicTimeUser, ManicTimeUsernameTextBox.Text, ManicTimePasswordTextBox.Password, null);
        }
    }
}