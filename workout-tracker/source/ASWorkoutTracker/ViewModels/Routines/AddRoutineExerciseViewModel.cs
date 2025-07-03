using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Events;
using Xamarin.Essentials;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Prism.Services.Dialogs;

namespace ASWorkoutTracker.ViewModels.Routines
{
    public class AddRoutineExerciseViewModel : ASWTListBaseViewModel<Exercise>, INavigationAware
    {
        public AddRoutineExerciseViewModel(INavigationService navigationService,
                                           IASWTDatastoreService datastoreService,
                                           IDialogService dialogService,
                                           IEventAggregator eventAggregator)
            : base(eventAggregator, dialogService)
        {
            _datastoreService = datastoreService;
            _navigationService = navigationService;

            Filters = new ObservableCollection<Filter>();

            Title = "Available Exercises";

            Select = new DelegateCommand<Exercise>(async (item) => { await OnSelect(item); });
            ModelUpdated = new DelegateCommand<Exercise>(async (exercise) => await OnModelUpdated(exercise));
            ShowHideDetails = new DelegateCommand<Exercise>((exercise) => OnShowHideDetails(exercise));
            SelectedFilterChanged = new DelegateCommand<Filter>(async (filter) => await OnSelectedFilterChanged(filter));

            SetLoadingState(true, "Loading Exercises");
        }

        public ICommand Select { get; }
        private int _routineID;
        private int _routineOrder;
        //private List<int> _existingExerciseIDs;

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineID))
                _routineID = parameters.GetValue<int>(AppConstants.ParameterKeys.RoutineID);

            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineOrder))
                _routineOrder = parameters.GetValue<int>(AppConstants.ParameterKeys.RoutineOrder);

            //if (parameters.ContainsKey(AppConstants.ParameterKeys.ExerciseIDs))
            //    _existingExerciseIDs = parameters.GetValue<List<int>>(AppConstants.ParameterKeys.ExerciseIDs);

            if (!Filters.Any())
                await UpdateFilters();
            
            await ReloadCollection();

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

            NoRecords = exercises.Count == 0;

            RaisePropertyChanged(() => CollectionCount);
        }

        protected override async Task<IEnumerable<Exercise>> PopulateCollection()
        {
            return await _datastoreService.ReadAllExercises();
            //return await _datastoreService.ReadAvailableRoutineExercises(_existingExerciseIDs, null, AppConstants.App.EXERCISES_PAGE_SIZE);
        }

        protected override async Task UpdateFilters()
        {
            var filters = await _datastoreService.ReadExerciseFilters();
            foreach (var filter in filters)
                Filters.Add(filter);

            RaisePropertyChanged(() => Filters);

            Filter selectedFilter = JsonConvert.DeserializeObject<Filter>(Preferences.Get(AppConstants.App.ADD_ROUTINE_EXERCISE_FILTER, string.Empty));
            if (selectedFilter != null)
            {
                //SelectedFilter = selectedFilter;
                SelectedFilterIndex = Filters.IndexOf(Filters.Where(x => x.FilterID == selectedFilter.FilterID).FirstOrDefault());                
            }
        }

        #region Command Methods
        private async Task OnSelect(Exercise item)
        {
            RoutineExercise routineExercise = new RoutineExercise
            {
                Exercise = item,
                ExerciseID = item.ID,
                RoutineID = _routineID,
                Order = _routineOrder,
                DateCreated = DateTime.Today
            };
            var saveResult = await _datastoreService.Create(routineExercise);

            var result = await _navigationService.GoBackAsync(
                new NavigationParameters {
                    { AppConstants.ParameterKeys.SelectedRoutineExercise, routineExercise } });
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

            foreach (var e in ListCollection)
                e.IsShowingDetails = e.ID == exercise.ID && exercise.IsShowingDetails;

            MessagingCenter.Send(this, AppConstants.ParameterKeys.ExerciseIsShowingDetailsChanged);
        }

        protected override async Task OnSelectedFilterChanged(Filter filter)
        {
            Preferences.Set(AppConstants.App.ADD_ROUTINE_EXERCISE_FILTER, JsonConvert.SerializeObject(filter));
            await ReloadCollection();
            return;
        }
        #endregion 
    }
}

