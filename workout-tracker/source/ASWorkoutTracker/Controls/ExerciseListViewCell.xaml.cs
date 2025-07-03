using System;
using System.ComponentModel;
using System.Windows.Input;
using ASWorkoutTracker.Datastore.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ASWorkoutTracker.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseListViewCell : ViewCell
    {
        public ExerciseListViewCell()
        {
            InitializeComponent();
        }

        private void DetailsTapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            CommandParameter.IsShowingDetails = !CommandParameter.IsShowingDetails;
            ShowHideDetailsCommand?.Execute(CommandParameter);
        }

        private void DeleteAction_Clicked(System.Object sender, System.EventArgs e)
        {
            DeleteCommand?.Execute(CommandParameter);
        }

        private void EditAction_Clicked(System.Object sender, System.EventArgs e)
        {
            EditCommand?.Execute(CommandParameter);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ExerciseListViewCell), Color.Transparent, propertyChanged: OnBorderColorPropertyChanged);        
        public static readonly BindableProperty OpenCommandProperty = BindableProperty.Create(nameof(OpenCommand), typeof(ICommand), typeof(ExerciseListViewCell), null, propertyChanged: OnOpenCommandPropertyChanged);
        public static readonly BindableProperty FavoriteCommandProperty = BindableProperty.Create(nameof(FavoriteCommand), typeof(ICommand), typeof(ExerciseListViewCell), null, propertyChanged: OnFavoriteCommandPropertyChanged);
        public static readonly BindableProperty DeleteCommandProperty = BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(ExerciseListViewCell), null);
        public static readonly BindableProperty EditCommandProperty = BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(ExerciseListViewCell), null);
        public static readonly BindableProperty ShowHideDetailsCommandProperty = BindableProperty.Create(nameof(ShowHideDetailsCommand), typeof(ICommand), typeof(ExerciseListViewCell), null);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(Exercise), typeof(ExerciseListViewCell), null, propertyChanged: OnCommandParameterPropertyChanged);
        
        public static readonly BindableProperty IsShowingDetailsProperty = BindableProperty.Create(nameof(IsShowingDetails), typeof(bool), typeof(ExerciseListViewCell), false, propertyChanged: OnIsShowingDetailsPropertyChanged);

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

        public ICommand FavoriteCommand
        {
            get { return (ICommand)GetValue(FavoriteCommandProperty); }
            set { SetValue(FavoriteCommandProperty, value); }
        }

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public ICommand ShowHideDetailsCommand
        {
            get { return (ICommand)GetValue(ShowHideDetailsCommandProperty); }
            set { SetValue(ShowHideDetailsCommandProperty, value); }
        }

        public Exercise CommandParameter
        {
            get { return (Exercise)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool IsShowingDetails
        {
            get { return (bool)GetValue(IsShowingDetailsProperty); }
            set { SetValue(IsShowingDetailsProperty, value); }
        }

        public string NoDataText { get; set; }

        private static void OnBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (ExerciseListViewCell)bindable;
            if (context != null)
            {
                context.Border.BackgroundColor = (Color)newValue;
                context.Border.BorderColor = (Color)newValue;
                context.EndBrush.Color = (Color)newValue;
            }
        }

        private void ProcessFavorite()
        {
            if (CommandParameter.IsFavorite)
            {
                FavoriteIcon.Source = (ImageSource)Application.Current.Resources["FavoriteIcon_Selected"];
                FavoriteIcon.TintColor = (Color)Application.Current.Resources["YellowAccent"];
            }
            else
            {
                FavoriteIcon.Source = (ImageSource)Application.Current.Resources["FavoriteIcon_Unselected"];
                FavoriteIcon.TintColor = (Color)Application.Current.Resources["Gray"];
            }
        }

        private static void OnFavoriteCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (ExerciseListViewCell)bindable;
            var command = (ICommand)newValue;

            if (context != null && command != null)
            {
                context.ProcessFavorite();

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => {
                    context.CommandParameter.IsFavorite = !context.CommandParameter.IsFavorite;
                    context.ProcessFavorite();
                    command?.Execute(context.CommandParameter);
                };

                context.FavoriteIcon.GestureRecognizers.Add(tapGesture);
            }
        }

        private static void OnOpenCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (ExerciseListViewCell)bindable;
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

        private static void OnIsShowingDetailsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (ExerciseListViewCell)bindable;
            bool isShowingDetails = (bool)newValue;
            if (context != null)
            {
                if (isShowingDetails)
                    context.DetailsIcon.TintColor = (Color)Application.Current.Resources["Primary"];
                else
                    context.DetailsIcon.TintColor = (Color)Application.Current.Resources["Gray"];
            }
        }

        private static void OnCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (ExerciseListViewCell)bindable;
            var exercise = (Exercise)newValue;
            if (context != null && exercise != null)
            {
                switch (exercise.ExerciseTypeID)
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
    }
}
