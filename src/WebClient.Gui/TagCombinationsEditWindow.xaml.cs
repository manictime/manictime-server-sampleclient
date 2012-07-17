using System.Windows;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public partial class TagCombinationsEditWindow
    {
        public string TagCombinations
        {
            get { return (string)GetValue(TagCombinationsProperty); }
            set { SetValue(TagCombinationsProperty, value); }
        }
        public static readonly DependencyProperty TagCombinationsProperty =
            DependencyProperty.Register("TagCombinations", typeof(string), typeof(TagCombinationsEditWindow), new UIPropertyMetadata(null));

        public TagCombinationsEditWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
