using ASWorkoutTracker.ViewModels.Exercises;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ASWorkoutTracker.Views.Exercises
{
    public partial class ExercisesView : ContentPage
    {
        private ExercisesViewModel _exercisesViewModel;
        private string _lastUrl;

        public ExercisesView()
        {
            InitializeComponent();
            RecordList.On<iOS>().SetRowAnimationsEnabled(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DetailsContent.Navigating += (s, e) => OpenUrl(e);
            MessagingCenter.Subscribe<ExercisesViewModel>(this, AppConstants.ParameterKeys.ExerciseIsShowingDetailsChanged, async (sender) =>
            {
                if (_exercisesViewModel == null)
                    _exercisesViewModel = (ExercisesViewModel)BindingContext;

                if (_exercisesViewModel != null && _exercisesViewModel.IsShowingDetails && _exercisesViewModel.Details != null)
                {
                    DetailsContent.Source = _exercisesViewModel.Details;

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
            DetailsContent.Navigating -= (s, e) => OpenUrl(e);
            DetailsContainer.TranslateTo(0, 0, 500, Easing.CubicOut);
            RecordList.TranslateTo(0, 0, 500, Easing.CubicOut);

            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ExercisesViewModel>(this, AppConstants.ParameterKeys.ExerciseIsShowingDetailsChanged);
        }

        protected void Filter_Clicked(System.Object sender, System.EventArgs e)
        {
            FilterPicker.IsOpen = true;
        }

        protected void FilterPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            if (_exercisesViewModel == null)
                _exercisesViewModel = (ExercisesViewModel)BindingContext;

            _exercisesViewModel.SelectedFilterChanged.Execute(e.NewValue);
            return;
        }

        protected void OpenUrl(WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith("http") || e.Url.Equals(_lastUrl))
                return;

            _lastUrl = e.Url;

            if (_exercisesViewModel == null)
                _exercisesViewModel = (ExercisesViewModel)BindingContext;

            _exercisesViewModel.OpenUrl?.Execute(e.Url);
            e.Cancel = true;
        }
    }
}
