using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Finkit.ManicTime.Server.SampleClient.Resources;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public partial class SendActivityUpdatesWindow
    {
        public static readonly DependencyProperty TimelinesProperty = DependencyProperty.Register(
            "Timelines", typeof(TimelineResource[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default( TimelineResource[] )));
        public TimelineResource[] Timelines
        {
            get { return (TimelineResource[]) GetValue(TimelinesProperty); }
            set { SetValue(TimelinesProperty, value); }
        }

        public static readonly DependencyProperty SelectedTimelineProperty = DependencyProperty.Register(
            "SelectedTimeline", typeof(TimelineResource), typeof(SendActivityUpdatesWindow), 
            new UIPropertyMetadata(null, (d, e) => ((SendActivityUpdatesWindow)d).OnTimelineSelected()));
        public TimelineResource SelectedTimeline
        {
            get { return (TimelineResource) GetValue(SelectedTimelineProperty); }
            set { SetValue(SelectedTimelineProperty, value); }
        }

        public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register(
            "Groups", typeof(GroupResource[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default( GroupResource[] )));
        public GroupResource[] Groups
        {
            get { return (GroupResource[]) GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public static readonly DependencyProperty DeletedGroupIdsProperty = DependencyProperty.Register(
            "DeletedGroupIds", typeof(string[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default( string[] )));
        public string[] DeletedGroupIds
        {
            get { return (string[]) GetValue(DeletedGroupIdsProperty); }
            set { SetValue(DeletedGroupIdsProperty, value); }
        }

        public static readonly DependencyProperty GroupListsProperty = DependencyProperty.Register(
            "GroupLists", typeof(GroupListResource[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default(GroupListResource[])));
        public GroupListResource[] GroupLists
        {
            get { return (GroupListResource[])GetValue(GroupListsProperty); }
            set { SetValue(GroupListsProperty, value); }
        }

        public static readonly DependencyProperty DeletedGroupListIdsProperty = DependencyProperty.Register(
            "DeletedGroupListIds", typeof(string[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default(string[])));
        public string[] DeletedGroupListIds
        {
            get { return (string[])GetValue(DeletedGroupListIdsProperty); }
            set { SetValue(DeletedGroupListIdsProperty, value); }
        }

        public static readonly DependencyProperty ActivitiesProperty = DependencyProperty.Register(
            "Activities", typeof(ActivityResource[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default( ActivityResource[] )));
        public ActivityResource[] Activities
        {
            get { return (ActivityResource[]) GetValue(ActivitiesProperty); }
            set { SetValue(ActivitiesProperty, value); }
        }

        public static readonly DependencyProperty DeletedActivityIdsProperty = DependencyProperty.Register(
            "DeletedActivityIds", typeof(string[]), typeof(SendActivityUpdatesWindow), new PropertyMetadata(default( string[] )));
        public string[] DeletedActivityIds
        {
            get { return (string[]) GetValue(DeletedActivityIdsProperty); }
            set { SetValue(DeletedActivityIdsProperty, value); }
        }

        public SendActivityUpdatesWindow()
        {
            InitializeComponent();
        }

        private void OnTimelineSelected()
        {
            SendButton.IsEnabled = SelectedTimeline != null;
            GroupListsTextBox.IsEnabled = SelectedTimeline != null && SelectedTimeline.TimelineType.GenericTypeName == "ManicTime/Generic/GroupList";
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Parse();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error Parsing Values");
            }
        }

        private void Parse()
        {
            Tuple<GroupResource[], string[]> parsedGroups;
            try
            {
                parsedGroups = Parse(GroupsTextBox.Text, ParseGroupResource, ParseDeleteResource);
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing groups.\n" + ex.Message, ex);
            }

            Tuple<GroupListResource[], string[]> parsedGroupLists;
            try
            {
                parsedGroupLists = GroupListsTextBox.IsEnabled
                    ? Parse(GroupListsTextBox.Text, ParseGroupListResource, ParseDeleteResource)
                    : new Tuple<GroupListResource[], string[]>(null, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing group lists.\n" + ex.Message, ex);
            }

            Tuple<ActivityResource[], string[]> parsedActivities;
            try
            {
                parsedActivities = Parse(ActivitiesTextBox.Text, ParseActivityResource, ParseDeleteResource);
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing activities.\n" + ex.Message, ex);
            }

            Groups = parsedGroups.Item1;
            DeletedGroupIds = parsedGroups.Item2;
            GroupLists = parsedGroupLists.Item1;
            DeletedGroupListIds = parsedGroupLists.Item2;
            Activities = parsedActivities.Item1;
            DeletedActivityIds = parsedActivities.Item2;
        }

        private Tuple<T[], string[]> Parse<T>(string text, Func<string, T> parseAdd, Func<string, string> parseDelete)
        {
            string[] lines = text.SplitIntoLines().Select(l => l.Trim()).ToArray();
            var items = new List<T>();
            var deletedTitemIds = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    if (lines[i] == string.Empty)
                        continue;
                    if (lines[i][0] == '+')
                        items.Add(parseAdd(lines[i].Substring(1).Trim()));
                    else if (lines[i][0] == '-')
                        deletedTitemIds.Add(parseDelete(lines[i].Substring(1).Trim()));
                    else
                        throw new ParseException("Invalid first character. Should be '+' for create/update or '-' for delete.");
                }
                catch (Exception ex)
                {
                    throw new ParseException("Error in line " + (i + 1) + ".\n" + ex.Message, ex);
                }
            }
            return Tuple.Create(items.ToArray(), deletedTitemIds.ToArray());
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private GroupResource ParseGroupResource(string line)
        {
            string[] values = line.Split(';').Select(l => l.Trim()).ToArray();
            if (values.Length < 4)
                throw new ParseException("Values missing.");
            if (values.Length > 6)
                throw new ParseException("Too many values.");

            bool skipColor;
            if (values.Length < 5)
                skipColor = false;
            else if (!Boolean.TryParse(values[4], out skipColor))
                throw new ParseException("Error parsing skipColor.");
            return new GroupResource
            {
                GroupId = values[0],
                DisplayKey = values[1],
                DisplayName = values[2],
                Color = values[3],
                SkipColor = skipColor,
                TextData = values.Length < 6 ? null : values[5]
            };
        }

        private static GroupListResource ParseGroupListResource(string line)
        {
            string[] values = line.Split(';').Select(l => l.Trim()).ToArray();
            if (values.Length < 4)
                throw new ParseException("Values missing.");
            if (values.Length > 4)
                throw new ParseException("Too many values.");
            return new GroupListResource
            {
                GroupListId = values[0],
                DisplayKey = values[1],
                Color = values[2] == string.Empty ? (int?)null : values[2].ToRgb(),
                GroupIds = values[3].Split(',').Select(v => v.Trim()).ToArray()
            };
        }

        private static string ParseDeleteResource(string line)
        {
            string[] values = line.Split(';').Select(l => l.Trim()).ToArray();
            if (values.Length < 1)
                throw new ParseException("Values missing.");
            if (values.Length > 1)
                throw new ParseException("Too many values.");
            return values[0];
        }

        private ActivityResource ParseActivityResource(string line)
        {
            string[] values = line.Split(';').Select(v => v.Trim()).ToArray();
            if (values.Length < 6)
                throw new ParseException("Values missing.");
            if (values.Length > 7)
                throw new ParseException("Too many values.");

            DateTimeOffset startTime;
            if (!DateTimeOffset.TryParse(values[4], out startTime))
                throw new ParseException("Error parsing start time.");
            DateTimeOffset endTime;
            if (!DateTimeOffset.TryParse(values[5], out endTime))
                throw new ParseException("Error parsing end time.");
            if (endTime <= startTime)
                throw new ParseException("End time must be greater than start time.");
            
            var activity = new ActivityResource
            {
                ActivityId = values[0],
                DisplayName = values[1],
                GroupId = values[2] == string.Empty ? null : values[2],
                GroupListId = values[3] == string.Empty ? null : values[3],
                StartTime = startTime,
                EndTime = endTime,
                TextData = values.Length < 7 ? null : values[6]
            };

            if (SelectedTimeline.TimelineType.GenericTypeName == "ManicTime/Generic/GroupList")
            {
                if (activity.GroupListId == null)
                    throw new ParseException("Activity must contain group list ID");
                if (activity.GroupId != null)
                    throw new ParseException("Activity must not contain group ID");
            }
            else
            {
                if (activity.GroupId == null)
                    throw new ParseException("Activity must contain group ID");
                if (activity.GroupListId != null)
                    throw new ParseException("Activity must not contain group list ID");
            }

            return activity;
        }
    }
}
