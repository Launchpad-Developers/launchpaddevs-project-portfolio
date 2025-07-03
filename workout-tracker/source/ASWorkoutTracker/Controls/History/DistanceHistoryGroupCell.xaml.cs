using System;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore.Models;
using Xamarin.Forms;

namespace ASWorkoutTracker.Controls
{
    public partial class DistanceHistoryGroupCell : ContentView
    {
        public DistanceHistoryGroupCell()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty GroupProperty = BindableProperty.Create(nameof(Group), typeof(Grouping<DateTime, ExerciseLog>), typeof(DistanceHistoryGroupCell), null, propertyChanged: OnGroupPropertyChanged);

        public Grouping<DateTime, ExerciseLog> Group
        {
            get { return (Grouping<DateTime, ExerciseLog>)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        private static void OnGroupPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (DistanceHistoryGroupCell)bindable;
            Grouping<DateTime, ExerciseLog> group = (Grouping<DateTime, ExerciseLog>)newValue;

            if (context != null && newValue != null)
            {
                //Header
                context.Date.Text = group.Key.ToString("dddd, MMMM d");

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
