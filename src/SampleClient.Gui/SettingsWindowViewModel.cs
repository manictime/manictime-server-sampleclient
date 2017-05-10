using System.Net;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public class SettingsWindowViewModel
    {
        public SettingsWindowUserType UserType { get; }
        public string Username { get; }
        public string Password { get; }
        public string Domain { get; }

        public SettingsWindowViewModel(SettingsWindowUserType userType, string username, string password, string domain)
        {
            UserType = userType;
            Username = username;
            Password = password;
            Domain = domain;
        }
    }
}