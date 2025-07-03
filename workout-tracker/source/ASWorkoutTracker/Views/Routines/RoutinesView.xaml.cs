using ASWorkoutTracker.ViewModels.Routines;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ASWorkoutTracker.Views.Routines
{
    public partial class RoutinesView : ContentPage
    {
        private RoutinesViewModel _routinesViewModel;
        private string _lastUrl;

        public RoutinesView()
        {
            InitializeComponent();
            RecordList.On<iOS>().SetRowAnimationsEnabled(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DetailsContent.Navigating += (s, e) => OpenUrl(e);
            MessagingCenter.Subscribe<RoutinesViewModel>(this, AppConstants.ParameterKeys.RoutineIsShowingDetailsChanged, async (sender) =>
            {
                if (_routinesViewModel == null)
                    _routinesViewModel = (RoutinesViewModel)BindingContext;

                if (_routinesViewModel != null && _routinesViewModel.IsShowingDetails && _routinesViewModel.Details != null)
                {
                    DetailsContent.Source = _routinesViewModel.Details;

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
            MessagingCenter.Unsubscribe<RoutinesViewModel>(this, AppConstants.ParameterKeys.RoutineIsShowingDetailsChanged);
        }

        protected void Filter_Clicked(System.Object sender, System.EventArgs e)
        {
            FilterPicker.IsOpen = true;
        }

        protected void FilterPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            if (_routinesViewModel == null)
                _routinesViewModel = (RoutinesViewModel)BindingContext;

            _routinesViewModel.SelectedFilterChanged.Execute(e.NewValue);
            return;
        }

        protected void OpenUrl(WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith("http") || e.Url.Equals(_lastUrl))
                return;

            if (_routinesViewModel == null)
                _routinesViewModel = (RoutinesViewModel)BindingContext;

            _routinesViewModel.OpenUrl?.Execute(e.Url);
            e.Cancel = true;
        }
    }
}
