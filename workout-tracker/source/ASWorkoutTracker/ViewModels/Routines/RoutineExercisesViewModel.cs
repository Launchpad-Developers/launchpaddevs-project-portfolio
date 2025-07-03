using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using ASCommonServices.Events;
using Prism.Events;
using Xamarin.Essentials;
using System.Windows.Input;
using ASWorkoutTracker.Controls;
using Prism.Services.Dialogs;
using Prism.Services;
using AS.Forms.Controls.Extensions;
using ASCommonServices.Models;
using System;
using AS.Forms.Controls.Constants;

namespace ASWorkoutTracker.ViewModels.Routines
{
    public class RoutineExercisesViewModel : ASWTListBaseViewModel<RoutineExercise>, INavigationAware
    {
        public RoutineExercisesViewModel(INavigationService navigationService,
                                         IASWTDatastoreService datastoreService,
                                         IDialogService dialogService,
                                         IEventAggregator eventAggregator)
            : base(eventAggregator, dialogService)
        {
            _datastoreService = datastoreService;
            _navigationService = navigationService;

            Title = "Routine Exercises";

            AddEdit = new DelegateCommand<RoutineExercise>(async (item) => await OnAddEdit(item));
            Open = new DelegateCommand<RoutineExercise>(async (item) => await OnOpen(item));
            Move = new DelegateCommand<MoveRoutineExercise>(async (mover) => await OnMove(mover));
            Remove = new DelegateCommand<RoutineExercise>(async (routine) => await OnRemove(routine));
            StartRoutine = new DelegateCommand(async () => await OnStartRoutine());

            SetLoadingState(true, "Loading Exercises");
        }

        public ICommand Remove { get; }
        public ICommand Move { get; }
        public ICommand StartRoutine { get; }
        private Routine _routine;

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey(AppConstants.ParameterKeys.Item))
            {
                _routine = parameters.GetValue<Routine>(AppConstants.ParameterKeys.Item); ;
                Title = _routine.Name;
                await ReloadCollection();
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.SelectedRoutineExercise))
            {
                RoutineExercise routineExercise = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.SelectedRoutineExercise);

                var navigationParameters = new NavigationParameters { { AppConstants.ParameterKeys.RoutineExercise, routineExercise } };
                await _navigationService.NavigateAsync(AppConstants.Navigation.RoutineExerciseSetup, navigationParameters, useModalNavigation: true);
                return;
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercise))
            {
                await ReloadCollection();
            }

            SetLoadingState(false, string.Empty);
        }

        public async override Task ReloadCollection()
        {
            List<RoutineExercise> routineExercises = (List<RoutineExercise>)await PopulateCollection();
            routineExercises = routineExercises.OrderBy(x => x.Order).ToList();

            ListCollection.Clear();

            foreach (var re in routineExercises)
            {
                re.Exercise.LastSets = await _datastoreService.ReadLastWorkout(re.Exercise.ID, re.ID);
                ListCollection.Add(re);
            }

            NoRecords = routineExercises.Count == 0;

            RaisePropertyChanged(() => CollectionCount);

            foreach (var exercise in ListCollection)
            {
                exercise.IsFirst = exercise.Order == 0;
                exercise.IsLast = exercise.Order == ListCollection.Count - 1;
            }
        }

        protected override async Task<IEnumerable<RoutineExercise>> PopulateCollection()
        {
            return await _datastoreService.ReadRoutineExercises(_routine.ID, null, AppConstants.App.ROUTINE_EXERCISES_PAGE_SIZE);
        }

        #region Command Methods

        private async Task OnAddEdit(RoutineExercise item)
        {
            if (item != null)
            {
                var result = await _navigationService.NavigateAsync(AppConstants.Navigation.RoutineExerciseSetup,
                                        new NavigationParameters {
                                            { AppConstants.ParameterKeys.RoutineExercise, item },
                                            { AppConstants.ParameterKeys.Add, false },
                                            { AppConstants.ParameterKeys.Edit, true } },
                                        useModalNavigation: true);
            }
            else
            {
                //List<int> exerciseIDs = ListCollection.Select(x => x.ExerciseID).ToList();
                //var result = await _navigationService.NavigateAsync(AppConstants.Navigation.AddRoutineExercise,
                //                        new NavigationParameters {
                //                            { AppConstants.ParameterKeys.RoutineID, _routine.ID },
                //                            { AppConstants.ParameterKeys.ExerciseIDs, exerciseIDs } });
                
                var result = await _navigationService.NavigateAsync(AppConstants.Navigation.AddRoutineExercise,
                                        new NavigationParameters {
                                            { AppConstants.ParameterKeys.RoutineID, _routine.ID },
                                            { AppConstants.ParameterKeys.RoutineOrder, CollectionCount } });
            }
            return;
        }

        private async Task OnOpen(RoutineExercise routineExercise)
        {
            List<Grouping<DateTime, ExerciseLog>> results = await _datastoreService.ReadGroupedExerciseLogs(routineExercise.Exercise, DateTime.MinValue, 0, AppConstants.App.GROUPED_HISTORY_PAGE_SIZE, routineExercise.ID);
            var parameters = new NavigationParameters
                {
                    {AppConstants.ParameterKeys.RoutineExercise, routineExercise },
                    {AppConstants.ParameterKeys.Exercise, routineExercise.Exercise },
                    {AppConstants.ParameterKeys.GroupedResults, results },
                    {AppConstants.ParameterKeys.Add, false },
                    {AppConstants.ParameterKeys.Edit, false }
                };
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.Tab, parameters);
            return;
        }

        private async Task OnMove(MoveRoutineExercise mover)
        {
            if (mover.NewPosition < 0 || mover.NewPosition >= ListCollection.Count)
                return;

            var itemToMove = ListCollection[mover.OldPosition];
            var itemToSwap = ListCollection[mover.NewPosition];

            itemToMove.Order = mover.NewPosition;
            itemToSwap.Order = mover.OldPosition;
            var updated = await _datastoreService.SwapRoutineExercises(itemToMove, itemToSwap);

            if (updated)
            {
                ListCollection.Move(mover.NewPosition, mover.OldPosition);

                if (DeviceInfo.Platform == DevicePlatform.Android)
                    OnPropertyChanged(nameof(ListCollection));
            }
            else
            {
                itemToMove.Order = mover.OldPosition;
                itemToSwap.Order = mover.NewPosition;
            }

            foreach (var exercise in ListCollection)
            {
                exercise.IsFirst = exercise.Order == 0;
                exercise.IsLast = exercise.Order == ListCollection.Count - 1;
            }
        }

        private async Task OnRemove(RoutineExercise routine)
        {
            bool confirm = await _dialogService.ConfirmAsync("Confirm", $"Are you sure you want to remove {routine.Exercise.Name} from {Title}?");
            if (confirm)
                await _datastoreService.Delete(routine);

            await ReloadCollection();
        }

        private async Task OnStartRoutine()
        {
            if (!ListCollection.Any()) return;

            var parameters = new NavigationParameters
            {
                {AppConstants.ParameterKeys.Routine, _routine },
                {AppConstants.ParameterKeys.RoutineExercises, ListCollection.ToList() }
            };
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.AddEditSet, parameters);
            return;
        }

        #endregion
    }
}
