using ASWorkoutTracker.ViewModels.Routines;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ASWorkoutTracker.Views.Routines
{
    public partial class AddRoutineExerciseView : ContentPage
    {
        public AddRoutineExerciseView()
        {
            InitializeComponent();
            RecordList.On<iOS>().SetRowAnimationsEnabled(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<AddRoutineExerciseViewModel>(this, AppConstants.ParameterKeys.ExerciseIsShowingDetailsChanged, async (sender) =>
            {
                var model = sender as AddRoutineExerciseViewModel;
                if (model != null && model.IsShowingDetails && model.Details != null)
                {
                    DetailsContent.Source = model.Details;

                    if (DetailsContainer.Y < 0)
                    {
                        DetailsContainer.TranslateTo(0, 250, 500, Easing.CubicIn);
                        RecordList.TranslateTo(0, 250, 500, Easing.CubicIn);
                    }
                }
                else
                {
                    DetailsContainer.TranslateTo(0, 0, 500, Easing.CubicOut);
                    await RecordList.TranslateTo(0, 0, 500, Easing.CubicOut);

                    HtmlWebViewSource source = new HtmlWebViewSource { Html = string.Empty };
                    DetailsContent.Source = source;
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<RoutineExercisesViewModel>(this, AppConstants.ParameterKeys.ExerciseIsShowingDetailsChanged);
        }

        protected void Filter_Clicked(System.Object sender, System.EventArgs e)
        {
            FilterPicker.IsOpen = true;
        }

        protected void FilterPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            var model = (AddRoutineExerciseViewModel)BindingContext;
            model.SelectedFilterChanged.Execute(e.NewValue);
            return;
        }
    }
}
