using System.ComponentModel;
using AS.Forms.Controls.BaseControls;
using AS.Forms.Controls.Models;
using ASWorkoutTracker.Enums;
using ASWorkoutTracker.ViewModels.Routines;
using Xamarin.Forms;

namespace ASWorkoutTracker.Views.Routines
{
    public partial class AddEditRoutineView : ASContentPage
    {
        public AddEditRoutineView()
        {
            InitializeComponent();

            INotifyPropertyChanged vm = BindingContext as AddEditRoutineViewModel;
            vm.PropertyChanged += (sender, args) => {
                if (!args.PropertyName.Equals("IsReadOnly"))
                    return;

                var model = sender as AddEditRoutineViewModel;
                if (!model.IsReadOnly)
                    return;
                else
                    this.ToolbarItems.Clear();
            };
        }
        
        private void OptionTypePicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            var pickerDef = new PickerDefinition {
                SelectedItem = e.NewValue,
                SelectedObjectProperty = PickerItemPropertyNames.SelectedEquipmentType
            };

            var model = (AddEditRoutineViewModel)BindingContext;
            model.PickerItemSelected.Execute(pickerDef);
        }
    }
}
