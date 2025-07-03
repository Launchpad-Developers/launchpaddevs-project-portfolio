using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Events;
using Xamarin.Essentials;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using ASCommonServices.Models;
using System;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;

namespace ASWorkoutTracker.ViewModels.Exercises
{
    public class ExercisesViewModel : ASWTListBaseViewModel<Exercise>, INavigationAware
    {
        public ExercisesViewModel(INavigationService navigationService,
                                  IASWTDatastoreService datastoreService,
                                  IDialogService dialogService,
                                  IEventAggregator eventAggregator)
            : base(eventAggregator, dialogService)
        {
            _datastoreService = datastoreService;
            _navigationService = navigationService;

            Filters = new ObservableCollection<Filter>();

            SetLoadingState(true, "Loading Exercises");
            Title = "All Exercises";

            AddEdit = new DelegateCommand<Exercise>(async (item) => await OnAddEdit(item));
            Open = new DelegateCommand<Exercise>(async (exercise) => await OnOpen(exercise));
            ModelUpdated = new DelegateCommand<Exercise>(async (exercise) => await OnModelUpdated(exercise));
            ShowHideDetails = new DelegateCommand<Exercise>((exercise) => OnShowHideDetails(exercise));
            SelectedFilterChanged = new DelegateCommand<Filter>(async (filter) => await OnSelectedFilterChanged(filter));
            OpenUrl = new DelegateCommand<string>(async (url) => await OnOpenUrl(url));
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            
            if (!Filters.Any())
                await UpdateFilters();
                        
            if (parameters.ContainsKey(AppConstants.ParameterKeys.SelectedRoutineExercise))
            {
                RoutineExercise routineExercise = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.SelectedRoutineExercise);
                routineExercise.Order = CollectionCount;

                var navigationParameters = new NavigationParameters { { AppConstants.ParameterKeys.RoutineExercise, routineExercise } };
                await _navigationService.NavigateAsync(AppConstants.Navigation.RoutineExerciseSetup, navigationParameters, useModalNavigation: true);
                return;
            }
            else if (!parameters.ContainsKey(AppConstants.ParameterKeys.Reload) ||
                      parameters.GetValue<bool>(AppConstants.ParameterKeys.Reload))
            {
                await ReloadCollection();
            }

            SetLoadingState(false, string.Empty);
        }

        public async override Task ReloadCollection()
        {
            List<Exercise> exercises = (List<Exercise>)await PopulateCollection();

            if (SelectedFilter == null || SelectedFilter.FilterID == 0)
                exercises = exercises.OrderByDescending(x => x.IsFavorite).ThenBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();
            else if (SelectedFilter.FilterID == 1)
                exercises = exercises.Where(x => x.IsFavorite).OrderBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();
            else
                exercises = exercises.Where(x => x.ExerciseTypeID == SelectedFilter.FilterID).OrderByDescending(x => x.IsFavorite).ThenBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();

            ListCollection.Clear();

            foreach (var exercise in exercises)
            {
                exercise.LastSets = await _datastoreService.ReadLastWorkout(exercise.ID);
                ListCollection.Add(exercise);
            }

            NoRecords = !ListCollection.Any();

            RaisePropertyChanged(() => CollectionCount);
        }

        protected override async Task UpdateFilters()
        {
            var filters = await _datastoreService.ReadExerciseFilters();
            foreach (var filter in filters)
                Filters.Add(filter);

            RaisePropertyChanged(() => Filters);

            Filter selectedFilter = JsonConvert.DeserializeObject<Filter>(Preferences.Get(AppConstants.App.EXERCISES_FILTER, string.Empty));
            if (selectedFilter != null)
            {
                SelectedFilter = selectedFilter;
                SelectedFilterIndex = Filters.IndexOf(Filters.Where(x => x.FilterID == selectedFilter.FilterID).FirstOrDefault());
            }            
        }

        protected override async Task<IEnumerable<Exercise>> PopulateCollection()
        {
            return await _datastoreService.ReadAllExercises();
        }

        protected override async Task OnSelectedFilterChanged(Filter filter)
        {
            Preferences.Set(AppConstants.App.EXERCISES_FILTER, JsonConvert.SerializeObject(filter));
            await ReloadCollection();
            return;
        }

        #region CommandMethods

        private async Task OnAddEdit(Exercise item)
        {
            IsShowingDetails = false;
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.AddEditExercise,
                                    new NavigationParameters {
                                        { AppConstants.ParameterKeys.Add, item == null },
                                        { AppConstants.ParameterKeys.Edit, item != null && !item.IsSystem },
                                        { AppConstants.ParameterKeys.Item, item } });
            return;
        }

        private void OnShowHideDetails(Exercise exercise)
        {
            if (exercise.IsShowingDetails)
            {
                Details = exercise.WebViewHighlights;
                IsShowingDetails = true;
            }
            else
            {
                IsShowingDetails = false;
                Details = new HtmlWebViewSource();
            }

            foreach (var r in ListCollection)
                r.IsShowingDetails = r.ID == exercise.ID && exercise.IsShowingDetails;

            MessagingCenter.Send(this, AppConstants.ParameterKeys.ExerciseIsShowingDetailsChanged);
        }

        protected async Task OnOpen(Exercise exercise)
        {
            if (!_datastoreService.DatabaseIsIntact()) return;

            exercise.IsShowingDetails = false;
            IsShowingDetails = false;

            var routineExercise = await _datastoreService.ReadDefaultRoutineExercise(exercise.ID);
            routineExercise.Exercise = exercise;

            List<Grouping<DateTime, ExerciseLog>> results = await _datastoreService.ReadGroupedExerciseLogs(exercise, DateTime.MinValue, 0, AppConstants.App.GROUPED_HISTORY_PAGE_SIZE);
            var parameters = new NavigationParameters
                {
                    {AppConstants.ParameterKeys.RoutineExercise, routineExercise },
                    {AppConstants.ParameterKeys.Exercise, exercise },
                    {AppConstants.ParameterKeys.GroupedResults, results },
                    {AppConstants.ParameterKeys.Add, false },
                    {AppConstants.ParameterKeys.Edit, false }
                };
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.Tab, parameters);
            return;
        }

        private async Task OnModelUpdated(Exercise exercise)
        {
            var updated = await _datastoreService.Update(exercise);
            if (updated)
            {
                var newList = ListCollection.ToList();

                //Replace object
                var oldIndex = ListCollection.IndexOf(exercise);
                newList.RemoveAt(oldIndex);
                newList.Add(exercise);

                newList = newList.OrderByDescending(x => x.IsFavorite).ThenBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();
                var newIndex = newList.IndexOf(exercise);

                ListCollection.Move(oldIndex, newIndex);

                if (DeviceInfo.Platform == DevicePlatform.Android)
                    OnPropertyChanged(nameof(ListCollection));
            }
        }

        protected override async Task OnDeleteRecord(Exercise exercise)
        {
            if (!exercise.IsSystem)
            {
                _dialogService.ShowMessage("Oops.", "Sorry, we can't delete a System exercise.");
                return;
            }

            var confirm = await _dialogService.ConfirmAsync("Whoa!", $"This will delete {((IBaseSQLModel)exercise).Name} and all of it's exercise logs, plus remove it from any Routines currently using it. Are you sure?");
            if (confirm)
            {
                SetLoadingState(true, $"Deleting {((IBaseSQLModel)exercise).Name}");
                var result = await _datastoreService.DeleteExercise(exercise);
                SetLoadingState(false, string.Empty);

                if (!result)
                {
                    _dialogService.ShowMessage("Oops.", "Something went wrong. It's not you, it's us.");
                }

                await ReloadCollection();
            }
        }

        #endregion
    }
}
