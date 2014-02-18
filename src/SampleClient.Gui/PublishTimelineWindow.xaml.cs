using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    /// <summary>
    /// Interaction logic for PublishTimelineWindow.xaml
    /// </summary>
    public partial class PublishTimelineWindow : Window
    {
        public static readonly DependencyProperty TimelineNameProperty = DependencyProperty.Register(
            "TimelineName", typeof(string), typeof(PublishTimelineWindow), new PropertyMetadata(default(string)));
        public string TimelineName
        {
            get { return (string)GetValue(TimelineNameProperty); }
            set { SetValue(TimelineNameProperty, value); }
        }

        public static readonly DependencyProperty TimelineKeyProperty = DependencyProperty.Register(
            "TimelineKey", typeof(string), typeof(PublishTimelineWindow), new PropertyMetadata(default( string )));
        public string TimelineKey
        {
            get { return (string) GetValue(TimelineKeyProperty); }
            set { SetValue(TimelineKeyProperty, value); }
        }

        public static readonly DependencyProperty TimelineTypeProperty = DependencyProperty.Register(
            "TimelineType", typeof(string), typeof(PublishTimelineWindow), new PropertyMetadata(default( string )));
        public string TimelineType
        {
            get { return (string) GetValue(TimelineTypeProperty); }
            set { SetValue(TimelineTypeProperty, value); }
        }

        public static readonly DependencyProperty GenericTimelineTypesProperty = DependencyProperty.Register(
            "GenericTimelineTypes", typeof(string[]), typeof(PublishTimelineWindow), new PropertyMetadata(default( string[] )));
        public string[] GenericTimelineTypes
        {
            get { return (string[]) GetValue(GenericTimelineTypesProperty); }
            set { SetValue(GenericTimelineTypesProperty, value); }
        }

        public static readonly DependencyProperty SelectedGenericTimelineTypeProperty = DependencyProperty.Register(
            "SelectedGenericTimelineType", typeof(string), typeof(PublishTimelineWindow), new PropertyMetadata(default( string )));
        public string SelectedGenericTimelineType
        {
            get { return (string) GetValue(SelectedGenericTimelineTypeProperty); }
            set { SetValue(SelectedGenericTimelineTypeProperty, value); }
        }

        public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register(
            "ClientName", typeof(string), typeof(PublishTimelineWindow), new PropertyMetadata(default( string )));
        public string ClientName
        {
            get { return (string) GetValue(ClientNameProperty); }
            set { SetValue(ClientNameProperty, value); }
        }

        public PublishTimelineWindow()
        {
            InitializeComponent();
        }

        private void PublishButton_Click(object sender, RoutedEventArgs e)
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
