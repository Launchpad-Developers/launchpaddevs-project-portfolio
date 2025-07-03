using System.ComponentModel;
using ASWorkoutTracker.Controls;
using ASWorkoutTracker.ViewModels.Exercises;
using Xamarin.Forms;

namespace ASWorkoutTracker.Views.Exercises
{
    public partial class HistoryView : ContentPage
    {
        public HistoryView()
        {
            InitializeComponent();

            INotifyPropertyChanged vm = BindingContext as HistoryViewModel;
            vm.PropertyChanged += (sender, args) => {
                if (!args.PropertyName.Equals("GroupedCollection"))
                    return;

                var model = sender as HistoryViewModel;

                if (model.RoutineExercise.RecordsWeight && model.RoutineExercise.RecordsReps)
                {
                    model.PopulateWeightRepsHeaderData();
                    HeaderContainer.Children.Add(new WeightRepsHistoryHeader());
                }
                else if (model.RoutineExercise.RecordsDistance)
                {
                    model.PopulateDistanceHeaderData();
                    HeaderContainer.Children.Add(new DistanceHistoryHeader());
                }
                else if (model.RoutineExercise.RecordsTime)
                {
                    model.PopulateTimeHeaderData();
                    HeaderContainer.Children.Add(new TimeHistoryHeader());
                }
                else if (model.RoutineExercise.RecordsElevation)
                {
                    model.PopulateElevationHeaderData();
                    HeaderContainer.Children.Add(new ElevationHistoryHeader());
                }
                else if (model.RoutineExercise.RecordsSteps)
                {
                    model.PopulateStepsHeaderData();
                    HeaderContainer.Children.Add(new StepHistoryHeader());
                }
                else
                {
                    model.PopulateGenericHeaderData();
                    HeaderContainer.Children.Add(new GenericHistoryHeader());
                }

                foreach (var group in model.GroupedCollection)
                {
                    if (model.RoutineExercise.RecordsWeight && model.RoutineExercise.RecordsReps)
                        GroupsContainer.Children.Add(new WeightRepsHistoryGroupCell { Group = group });
                    else if (model.RoutineExercise.RecordsDistance)
                        GroupsContainer.Children.Add(new DistanceHistoryGroupCell { Group = group });
                    else if (model.RoutineExercise.RecordsTime)
                        GroupsContainer.Children.Add(new TimeHistoryGroupCell { Group = group });
                    else if (model.RoutineExercise.RecordsElevation)
                        GroupsContainer.Children.Add(new ElevationHistoryGroupCell { Group = group });
                    else if (model.RoutineExercise.RecordsSteps)
                        GroupsContainer.Children.Add(new StepHistoryGroupCell { Group = group });
                    else
                        GroupsContainer.Children.Add(new GenericHistoryGroupCell { Group = group });
                }
            };
        }
    }
}
