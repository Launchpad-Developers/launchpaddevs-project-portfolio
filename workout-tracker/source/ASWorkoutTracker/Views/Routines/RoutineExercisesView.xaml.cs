using ASWorkoutTracker.ViewModels.Routines;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ASWorkoutTracker.Views.Routines
{
    public partial class RoutineExercisesView : ContentPage
    {
        public RoutineExercisesView()
        {
            InitializeComponent();
            RecordList.On<iOS>().SetRowAnimationsEnabled(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
