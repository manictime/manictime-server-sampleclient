using System.Windows;
using Finkit.ManicTime.WebClient.Resources;

namespace Finkit.ManicTime.WebClient.Gui
{
    public partial class TimelinePickerWindow
    {
        public TimelineResource[] Timelines
        {
            get { return (TimelineResource[])GetValue(TimelinesProperty); }
            set { SetValue(TimelinesProperty, value); }
        }
        public static readonly DependencyProperty TimelinesProperty =
            DependencyProperty.Register("Timelines", typeof(TimelineResource[]), typeof(TimelinePickerWindow), new UIPropertyMetadata(null));


        public TimelineResource SelectedTimeline
        {
            get { return (TimelineResource)GetValue(SelectedTimelineProperty); }
            set { SetValue(SelectedTimelineProperty, value); }
        }
        public static readonly DependencyProperty SelectedTimelineProperty =
            DependencyProperty.Register("SelectedTimeline", typeof(TimelineResource), typeof(TimelinePickerWindow), new UIPropertyMetadata(null));

        
        public TimelinePickerWindow()
        {
            InitializeComponent();
        }


        private void SelectButton_Click(object sender, RoutedEventArgs e)
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
