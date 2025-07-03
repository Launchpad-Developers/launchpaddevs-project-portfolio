using System;
using ASWorkoutTracker.Datastore.Models;
using ASWorkoutTracker.ViewModels.Routines;
using Xamarin.Forms;

namespace ASWorkoutTracker.Views.Routines
{
    public partial class RoutineExerciseSetupView : ContentPage
    {
        public RoutineExerciseSetupView()
        {
            InitializeComponent();
        }

        protected void SetsButton_Clicked(System.Object sender, System.EventArgs e)
        {
            SetsPicker.IsOpen = true;
        }

        private void SetsPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            var model = (RoutineExerciseSetupViewModel)BindingContext;
            model.SetsPickerSelectedItem = (NumericPickerOption)e.NewValue;
            return;
        }


        private void TimeToCompleteButton_Tapped(System.Object sender, System.EventArgs e)
        {
            TimeToComplete.IsOpen = true;
        }

        private void TimeToComplete_TimeSelected(System.Object sender, Syncfusion.XForms.Pickers.TimeChangedEventArgs e)
        {
            var model = (RoutineExerciseSetupViewModel)BindingContext;
            model.TimeToComplete = (TimeSpan)e.NewValue;
            return;
        }


        private void BreakAfterSetButton_Tapped(System.Object sender, System.EventArgs e)
        {
            BreakAfterSetTime.IsOpen = true;
        }

        private void BreakAfterSetTime_TimeSelected(System.Object sender, Syncfusion.XForms.Pickers.TimeChangedEventArgs e)
        {
            var model = (RoutineExerciseSetupViewModel)BindingContext;
            model.TimeBetweenSets = (TimeSpan)e.NewValue;
            return;
        }


        private void BreakAfterExerciseButton_Tapped(System.Object sender, System.EventArgs e)
        {
            BreakAfterWorkoutTime.IsOpen = true;
        }

        private void BreakAfterWorkoutTime_TimeSelected(System.Object sender, Syncfusion.XForms.Pickers.TimeChangedEventArgs e)
        {
            var model = (RoutineExerciseSetupViewModel)BindingContext;
            model.TimeAfterExercise = (TimeSpan)e.NewValue;
            return;
        }
    }
}
