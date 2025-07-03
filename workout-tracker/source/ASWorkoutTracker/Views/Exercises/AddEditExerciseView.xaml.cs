using System.ComponentModel;
using AS.Forms.Controls.BaseControls;
using AS.Forms.Controls.Models;
using ASWorkoutTracker.Enums;
using ASWorkoutTracker.ViewModels.Exercises;
using Syncfusion.SfPicker.XForms;
using Xamarin.Forms;

namespace ASWorkoutTracker.Views.Exercises
{
    public partial class AddEditExerciseView : ASContentPage
    {
        private AddEditExerciseViewModel _addEditExerciseViewModel;
        private string _lastUrl;

        public AddEditExerciseView()
        {
            InitializeComponent();

            INotifyPropertyChanged vm = BindingContext as AddEditExerciseViewModel;
            vm.PropertyChanged += (sender, args) => {
                if (!args.PropertyName.Equals("IsReadOnly"))
                    return;

                var model = sender as AddEditExerciseViewModel;
                if (!model.IsReadOnly)
                    return;
                else
                    this.ToolbarItems.Clear();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WebView1.Navigating += (s, e) => OpenUrl(e);
            WebView2.Navigating += (s, e) => OpenUrl(e);
        }

        protected override void OnDisappearing()
        {
            WebView1.Navigating -= (s, e) => OpenUrl(e);
            WebView2.Navigating -= (s, e) => OpenUrl(e);
            base.OnDisappearing();
        }

        private void OptionType_Tapped(System.Object sender, PickerDefinition e)
        {
            if (e.SelectedObjectProperty == PickerItemPropertyNames.SelectedEquipmentType)
                EquipmentTypePicker.IsOpen = true;
            else if (e.SelectedObjectProperty == PickerItemPropertyNames.SelectedBodyAreaType)
                BodyAreaTypePicker.IsOpen = true;
            else if (e.SelectedObjectProperty == PickerItemPropertyNames.SelectedExerciseType)
                ExerciseTypePicker.IsOpen = true;
            else if (e.SelectedObjectProperty == PickerItemPropertyNames.SelectedLevel)
                LevelPicker.IsOpen = true;
        }

        private void OptionTypePicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            var pickerDef = new PickerDefinition { SelectedItem = e.NewValue };

            if (((SfPicker)sender).Id == EquipmentTypePicker.Id)
                pickerDef.SelectedObjectProperty = PickerItemPropertyNames.SelectedEquipmentType;
            else if (((SfPicker)sender).Id == BodyAreaTypePicker.Id)
                pickerDef.SelectedObjectProperty = PickerItemPropertyNames.SelectedBodyAreaType;
            else if (((SfPicker)sender).Id == ExerciseTypePicker.Id)
                pickerDef.SelectedObjectProperty = PickerItemPropertyNames.SelectedExerciseType;
            else if (((SfPicker)sender).Id == LevelPicker.Id)
                pickerDef.SelectedObjectProperty = PickerItemPropertyNames.SelectedLevel;

            var model = (AddEditExerciseViewModel)BindingContext;
            model.PickerItemSelected.Execute(pickerDef);
        }

        protected void OpenUrl(WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith("http") || e.Url.Equals(_lastUrl))
                return;

            _lastUrl = e.Url;

            if (_addEditExerciseViewModel == null)
                _addEditExerciseViewModel = (AddEditExerciseViewModel)BindingContext;

            _addEditExerciseViewModel.OpenUrl?.Execute(e.Url);
            e.Cancel = true;
        }
    }
}
