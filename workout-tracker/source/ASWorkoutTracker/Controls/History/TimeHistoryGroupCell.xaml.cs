using System;
using System.Collections.Generic;
using System.Linq;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore.Models;
using Xamarin.Forms;

namespace ASWorkoutTracker.Controls
{
    public partial class TimeHistoryGroupCell : ContentView
    {
        public TimeHistoryGroupCell()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty GroupProperty = BindableProperty.Create(nameof(Group), typeof(Grouping<DateTime, ExerciseLog>), typeof(TimeHistoryGroupCell), null, propertyChanged: OnGroupPropertyChanged);

        public Grouping<DateTime, ExerciseLog> Group
        {
            get { return (Grouping<DateTime, ExerciseLog>)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        private static void OnGroupPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (TimeHistoryGroupCell)bindable;
            Grouping<DateTime, ExerciseLog> group = (Grouping<DateTime, ExerciseLog>)newValue;
                       
            if (context != null && newValue != null)
            {
                //Header
                context.Date.Text = group.Key.ToString("dddd, MMMM d");

                //Maxes
                var minTime = group.PublicItems.Where(x => x.Time > TimeSpan.Zero).Min(x => x.Time);
                var maxTime = group.PublicItems.Where(x => x.Time > TimeSpan.Zero).Max(x => x.Time);
                var avgTimeTicks = group.PublicItems.Where(x => x.Time > TimeSpan.Zero).Average(x => x.Time.TotalMilliseconds);
                var avgTime = avgTimeTicks > 0 ? TimeSpan.FromMilliseconds(avgTimeTicks) : TimeSpan.Zero;
                avgTime = avgTime.Add(-TimeSpan.FromMilliseconds(avgTime.Milliseconds));

                if (minTime == TimeSpan.Zero || maxTime == TimeSpan.Zero || avgTime == TimeSpan.Zero)
                {
                    context.MinTime.Text = "0:00:00";
                    context.AvgTime.Text = "0:00:00";
                    context.MaxTime.Text = "0:00:00";

                    context.ExerciseLogs.Children.Add(
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,                            
                            Margin = new Thickness(-3),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Children =
                            {
                                new Label
                                {
                                    Text = $"No Time Data",
                                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                                    VerticalOptions = LayoutOptions.CenterAndExpand,
                                    TextColor = (Color)Application.Current.Resources["GrayDarkest"],
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Bold
                                }
                            }
                        }
                    );

                    return;
                }

                context.MinTime.Text = minTime.ToString(@"h\:mm\:ss");
                context.AvgTime.Text = avgTime.ToString(@"h\:mm\:ss");
                context.MaxTime.Text = maxTime.ToString(@"h\:mm\:ss");

                //Logs
                int i = 1;
                foreach (var log in group.PublicItems)
                {
                    context.ExerciseLogs.Children.Add(
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Margin = new Thickness(-3),
                            Children =
                            {
                                new Label
                                {
                                    Text = $"Set {i++}: ",
                                    HorizontalOptions = LayoutOptions.End,
                                    VerticalOptions = LayoutOptions.Start,
                                    TextColor = (Color)Application.Current.Resources["GrayDarkest"],
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Bold
                                },
                                new Label
                                {
                                    Text = $"{log.ResultDetail}",
                                    HorizontalOptions = LayoutOptions.StartAndExpand,
                                    VerticalOptions = LayoutOptions.Start,
                                    TextColor = (Color)Application.Current.Resources["GrayDarkest"],
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Italic,
                                    LineBreakMode = LineBreakMode.TailTruncation
                                }
                            }
                        }
                    );
                }
            }
        }
    }
}
