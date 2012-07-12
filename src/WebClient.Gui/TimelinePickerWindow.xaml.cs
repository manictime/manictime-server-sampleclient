using System;
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
            DependencyProperty.Register("SelectedTimeline", typeof(TimelineResource), typeof(TimelinePickerWindow), 
            new UIPropertyMetadata(null, (d, e) => ((TimelinePickerWindow)d).EnableSelectButton()));

        public DateTime? FromTime
        {
            get { return (DateTime?)GetValue(FromTimeProperty); }
            set { SetValue(FromTimeProperty, value); }
        }
        public static readonly DependencyProperty FromTimeProperty =
            DependencyProperty.Register("FromTime", typeof(DateTime?), typeof(TimelinePickerWindow), 
            new UIPropertyMetadata(null, (d, e) => ((TimelinePickerWindow)d).EnableSelectButton()));


        public DateTime? ToTime
        {
            get { return (DateTime?)GetValue(ToTimeProperty); }
            set { SetValue(ToTimeProperty, value); }
        }
        public static readonly DependencyProperty ToTimeProperty =
            DependencyProperty.Register("ToTime", typeof(DateTime?), typeof(TimelinePickerWindow), 
            new UIPropertyMetadata(null, (d, e) => ((TimelinePickerWindow)d).EnableSelectButton()));

        
        public TimelinePickerWindow()
        {
            InitializeComponent();
            EnableSelectButton();
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

        private void EnableSelectButton()
        {
            SelectButton.IsEnabled = 
                SelectedTimeline != null && 
                FromTime != null && 
                ToTime != null && 
                FromTime <= ToTime &&
                ToTime - FromTime < TimeSpan.FromDays(7);
        }
    }
}
