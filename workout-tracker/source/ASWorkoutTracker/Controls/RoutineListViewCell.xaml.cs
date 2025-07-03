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
    public partial class RoutineListViewCell : ViewCell
    {
        public RoutineListViewCell()
        {
            InitializeComponent();
        }

        private void DetailsTapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            CommandParameter.IsShowingDetails = !CommandParameter.IsShowingDetails;
            ShowHideDetailsCommand?.Execute(CommandParameter);
            //if (DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    ShowHideDetailsCommand?.Execute(CommandParameter);
            //}
            //else
            //{
            //    //Android
            //    if (CommandParameter.IsShowingDetails)
            //    {
            //        ForceUpdateSize();
            //        await DetailsIcon.RotateTo(180, 150);
            //        Details.IsVisible = CommandParameter.IsShowingDetails;
            //        await Details.FadeTo(1.0);
            //        TypeBorder.CornerRadius = new Thickness(0, 0, 0, 3);
            //    }
            //    else
            //    {
            //        await Details.FadeTo(0.0);
            //        Details.IsVisible = CommandParameter.IsShowingDetails;
            //        await DetailsIcon.RotateTo(0, 150);
            //        TypeBorder.CornerRadius = new Thickness(0, 0, 3, 0);
            //        ForceUpdateSize();
            //    }
            //}
        }

        private void DeleteAction_Clicked(System.Object sender, System.EventArgs e)
        {
            DeleteCommand?.Execute(CommandParameter);
        }

        private void EditAction_Clicked(System.Object sender, System.EventArgs e)
        {
            EditCommand?.Execute(CommandParameter);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RoutineListViewCell), Color.Transparent, propertyChanged: OnBorderColorPropertyChanged);
        public static readonly BindableProperty OpenCommandProperty = BindableProperty.Create(nameof(OpenCommand), typeof(ICommand), typeof(RoutineListViewCell), null, propertyChanged: OnOpenCommandPropertyChanged);
        public static readonly BindableProperty FavoriteCommandProperty = BindableProperty.Create(nameof(FavoriteCommand), typeof(ICommand), typeof(RoutineListViewCell), null, propertyChanged: OnFavoriteCommandPropertyChanged);
        public static readonly BindableProperty DeleteCommandProperty = BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(RoutineListViewCell), null);
        public static readonly BindableProperty EditCommandProperty = BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(RoutineListViewCell), null);
        public static readonly BindableProperty ShowHideDetailsCommandProperty = BindableProperty.Create(nameof(ShowHideDetailsCommand), typeof(ICommand), typeof(RoutineListViewCell), null);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(Routine), typeof(RoutineListViewCell), null);
        public static readonly BindableProperty IsShowingDetailsProperty = BindableProperty.Create(nameof(IsShowingDetails), typeof(bool), typeof(RoutineListViewCell), false, propertyChanged: OnIsShowingDetailsPropertyChanged);

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

        public Routine CommandParameter
        {
            get { return (Routine)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool IsShowingDetails
        {
            get { return (bool)GetValue(IsShowingDetailsProperty); }
            set { SetValue(IsShowingDetailsProperty, value); }
        }

        private static void OnBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (RoutineListViewCell)bindable;
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
            var context = (RoutineListViewCell)bindable;
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
            var context = (RoutineListViewCell)bindable;
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
            var context = (RoutineListViewCell)bindable;
            bool isShowingDetails = (bool)newValue;
            if (context != null)
            {
                if (isShowingDetails)
                    context.DetailsIcon.TintColor = (Color)Application.Current.Resources["Primary"];
                else
                    context.DetailsIcon.TintColor = (Color)Application.Current.Resources["Gray"];
            }
        }
    }
}
