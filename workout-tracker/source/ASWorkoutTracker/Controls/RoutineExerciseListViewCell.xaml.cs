using System;
using System.Windows.Input;
using ASWorkoutTracker.Datastore.Models;
using ASWorkoutTracker.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ASWorkoutTracker.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutineExerciseListViewCell : ViewCell
    {
        public RoutineExerciseListViewCell()
        {
            InitializeComponent();
        }

        private void RemoveAction_Clicked(System.Object sender, System.EventArgs e)
        {
            RemoveCommand?.Execute(CommandParameter);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RoutineExerciseListViewCell), Color.Transparent, propertyChanged: OnBorderColorPropertyChanged);
        public static readonly BindableProperty OpenCommandProperty = BindableProperty.Create(nameof(OpenCommand), typeof(ICommand), typeof(RoutineExerciseListViewCell), null, propertyChanged: OnOpenCommandPropertyChanged);
        public static readonly BindableProperty RemoveCommandProperty = BindableProperty.Create(nameof(RemoveCommand), typeof(ICommand), typeof(RoutineExerciseListViewCell), null);
        public static readonly BindableProperty MoveUpCommandProperty = BindableProperty.Create(nameof(MoveUpCommand), typeof(ICommand), typeof(RoutineExerciseListViewCell), null, propertyChanged: OnMoveUpCommandPropertyChanged);
        public static readonly BindableProperty MoveDownCommandProperty = BindableProperty.Create(nameof(MoveDownCommand), typeof(ICommand), typeof(RoutineExerciseListViewCell), null, propertyChanged: OnMoveDownCommandPropertyChanged);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(RoutineExercise), typeof(RoutineExerciseListViewCell), null, propertyChanged: OnCommandParameterPropertyChanged);
        public static readonly BindableProperty IsFirstProperty = BindableProperty.Create(nameof(IsFirst), typeof(bool), typeof(RoutineExerciseListViewCell), false, propertyChanged: OnIsFirstPropertyChanged);
        public static readonly BindableProperty IsLastProperty = BindableProperty.Create(nameof(IsLast), typeof(bool), typeof(RoutineExerciseListViewCell), false, propertyChanged: OnIsLastPropertyChanged);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public ICommand OpenCommand
        {
            get { return (ICommand)GetValue(OpenCommandProperty); }
            set { SetValue(OpenCommandProperty, value); }
        }

        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public ICommand MoveUpCommand
        {
            get { return (ICommand)GetValue(MoveUpCommandProperty); }
            set { SetValue(MoveUpCommandProperty, value); }
        }

        public ICommand MoveDownCommand
        {
            get { return (ICommand)GetValue(MoveDownCommandProperty); }
            set { SetValue(MoveDownCommandProperty, value); }
        }

        public RoutineExercise CommandParameter
        {
            get { return (RoutineExercise)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool IsFirst
        {
            get { return (bool)GetValue(IsFirstProperty); }
            set { SetValue(IsFirstProperty, value); }
        }

        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }

        private static void OnBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            if (context != null)
            {
                context.Border.BackgroundColor = (Color)newValue;
                context.Border.BorderColor = (Color)newValue;
                context.EndBrush.Color = (Color)newValue;
            }
        }

        private static void OnOpenCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            var command = (ICommand)newValue;

            if (context != null && command != null)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => {
                    command?.Execute(context.CommandParameter);
                };

                context.Tapper.GestureRecognizers.Add(tapGesture);
            }
        }

        private static void OnMoveUpCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            var command = (ICommand)newValue;

            if (context != null && command != null)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => {
                    command?.Execute(new MoveRoutineExercise
                    {
                        RoutineExercise = context.CommandParameter,
                        OldPosition = context.CommandParameter.Order,
                        NewPosition = context.CommandParameter.Order - 1
                    });                    
                };

                context.MoveUpIcon.GestureRecognizers.Add(tapGesture);
            }
        }

        private static void OnMoveDownCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            var command = (ICommand)newValue;

            if (context != null && command != null)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => {
                    command?.Execute(new MoveRoutineExercise
                    {
                        RoutineExercise = context.CommandParameter,
                        OldPosition = context.CommandParameter.Order,
                        NewPosition = context.CommandParameter.Order + 1
                    });
                };

                context.MoveDownIcon.GestureRecognizers.Add(tapGesture);
            }
        }

        private static void OnCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            var exercise = (RoutineExercise)newValue;
            if (context != null && exercise != null)
            {
                switch (exercise.Exercise.ExerciseTypeID)
                {
                    case 1000001: //Endurance
                        context.TypeBorder.BackgroundColor = (Color)Application.Current.Resources["GreenAccent"];
                        context.TypeLabel.TextColor = Color.White;
                        break;
                    case 1000002: //Strength
                        context.TypeBorder.BackgroundColor = (Color)Application.Current.Resources["GrayDarkest"];
                        context.TypeLabel.TextColor = (Color)Application.Current.Resources["GrayLightest"];
                        break;
                    case 1000003: //Flexibility
                        context.TypeBorder.BackgroundColor = (Color)Application.Current.Resources["BeigeAccent"];
                        context.TypeLabel.TextColor = (Color)Application.Current.Resources["GrayDarkest"];
                        break;
                    case 1000004: //Balance
                        context.TypeBorder.BackgroundColor = (Color)Application.Current.Resources["Primary"];
                        context.TypeLabel.TextColor = Color.White;
                        break;
                    default: //User Defined
                        context.TypeBorder.BackgroundColor = (Color)Application.Current.Resources["YellowAccent"];
                        context.TypeLabel.TextColor = (Color)Application.Current.Resources["PrimaryDarkest"];
                        break;
                }
            }
        }

        private static void OnIsFirstPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            var isFirst = (bool)newValue;

            if (context != null)
                context.MoveUpIcon.IsVisible = !isFirst;
        }

        private static void OnIsLastPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineExerciseListViewCell)bindable;
            var isLast = (bool)newValue;

            if (context != null)
                context.MoveDownIcon.IsVisible = !isLast;
        }
    }

    public class MoveRoutineExercise
    {
        public RoutineExercise RoutineExercise { get; set; }
        public int OldPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
