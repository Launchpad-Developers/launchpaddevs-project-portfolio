using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Events;
using System.Collections.Generic;
using System;
using ASCommonServices.Models;
using System.Collections.ObjectModel;
using System.Linq;
using ASWorkoutTracker.Helpers;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace ASWorkoutTracker.ViewModels.Exercises
{
    public class HistoryViewModel : ASWTModelBaseViewModel<ExerciseLog>, INavigationAware
    {
        public HistoryViewModel(INavigationService navigationService,
                                IASWTDatastoreService datastoreService,
                                IDialogService dialogService,
                                IPageDialogService pageDialogService,
                                IEventAggregator eventAggregator,
                                IJsonConsumerService jsonConsumerService)
            : base(jsonConsumerService, eventAggregator, pageDialogService, dialogService)
        {
            _datastoreService = datastoreService;
            _navigationService = navigationService;
            //_session = session;

            GroupedCollection = new ObservableCollection<Grouping<DateTime, ExerciseLog>>();

            ShowNotEnoughData = true;
            Title = "History";
        }

        public int GroupedCollectionCount { get { return GroupedCollection.Count; } }
        public ObservableCollection<Grouping<DateTime, ExerciseLog>> GroupedCollection { get; protected set; }
        public Exercise Exercise { get; set; }
        public RoutineExercise RoutineExercise { get; set; }
        protected int _oneRepMax;


        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            //DO NOT CALL BASE HERE
            if (parameters.ContainsKey(AppConstants.ParameterKeys.Exercise))
            {
                Exercise = parameters.GetValue<Exercise>(AppConstants.ParameterKeys.Exercise);
            }

            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercise))
            {
                RoutineExercise = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.RoutineExercise);
            }

            if (parameters.ContainsKey(AppConstants.ParameterKeys.GroupedResults))
            {
                var results = parameters.GetValue<List<Grouping<DateTime, ExerciseLog>>>(AppConstants.ParameterKeys.GroupedResults);

                GroupedCollection.Clear();
                foreach (var result in results)
                    GroupedCollection.Add(result);

                ShowNotEnoughData = GroupedCollection.Count == 0;

                RaisePropertyChanged(() => GroupedCollectionCount);
                RaisePropertyChanged(() => GroupedCollection);
            }

            SetLoadingState(false, string.Empty);
        }

        #region Data Loads
        public void PopulateWeightRepsHeaderData()
        {
            _oneRepMax = 0;
            var lastGroup = GroupedCollection.FirstOrDefault();
            if (lastGroup != null)
            {
                var maxWeight = lastGroup.PublicItems.Max(x => x.Weight);
                var maxReps = lastGroup.PublicItems.Where(x => x.Weight == maxWeight).Max(x => x.Reps);
                if (maxWeight > 0 && maxReps > 0)
                    _oneRepMax = WeightLiftingHelper.CalculateOneRepMax((int)maxWeight, maxReps);
            }

            HasOneRepMaxData = _oneRepMax > 0;
            if (!HasOneRepMaxData) return;

            OneRepMaxLabel = $"Estimated 1RM: {_oneRepMax}{Preferences.Get(AppConstants.App.WEIGHT_SYSTEM, AppConstants.App.DEFAULT_WEIGHT_SYSTEM)}";
            FiftyValueLabel = $"{(int)(_oneRepMax * 0.5)}";
            SeventyValueLabel = $"{(int)(_oneRepMax * 0.7)}";
            EightyValueLabel = $"{(int)(_oneRepMax * 0.8)}";
            NinetyValueLabel = $"{(int)(_oneRepMax * 0.9)}";
            NinetyFiveValueLabel = $"{(int)(_oneRepMax * 0.95)}";
            HasOneRepMaxData = true;
        }

        public async void PopulateGenericHeaderData()
        {
            GenericWorkoutHistoryData data = await _datastoreService.ReadTotalWorkoutsAndSets(RoutineExercise.ExerciseID, DateTime.MinValue, RoutineExercise.ID);

            NumberOfWorkouts = data.NumberOfWorkouts;
            NumberOfSets = data.NumberOfSets;
        }

        public async void PopulateTimeHeaderData()
        {
            TimedWorkoutHistoryData data = await _datastoreService.ReadTotalWorkoutTimes(RoutineExercise.ExerciseID, DateTime.MinValue, RoutineExercise.ID);

            MinTime = data.MinTime;
            AverageTime = data.AverageTime;
            MaxTime = data.MaxTime;
        }

        public async void PopulateDistanceHeaderData()
        {
            DistanceHistoryData data = await _datastoreService.ReadDistances(RoutineExercise.ExerciseID, DateTime.MinValue, RoutineExercise.ID);

            TotalDistance = $"{data.TotalDistance} {Preferences.Get(AppConstants.App.DISTANCE_SYSTEM, AppConstants.App.DEFAULT_DISTANCE_SYSTEM)}";
            LongestDistance = $"{data.LongestDistance} {Preferences.Get(AppConstants.App.DISTANCE_SYSTEM, AppConstants.App.DEFAULT_DISTANCE_SYSTEM)}";
        }

        public async void PopulateElevationHeaderData()
        {
            ElevationHistoryData data = await _datastoreService.ReadElevations(RoutineExercise.ExerciseID, DateTime.MinValue, RoutineExercise.ID);

            TotalElevation = $"{data.TotalElevation} {Preferences.Get(AppConstants.App.ELEVATION_SYSTEM, AppConstants.App.DEFAULT_ELEVATION_SYSTEM)}";
            HighestElevation = $"{data.HighestElevation} {Preferences.Get(AppConstants.App.ELEVATION_SYSTEM, AppConstants.App.DEFAULT_ELEVATION_SYSTEM)}";
        }

        public async void PopulateStepsHeaderData()
        {
            StepsHistoryData data = await _datastoreService.ReadSteps(RoutineExercise.ExerciseID, DateTime.MinValue, RoutineExercise.ID);

            TotalSteps = $"{data.TotalSteps:n0} steps";
            MaxSteps = $"{data.MaxSteps:n0} steps";
        }
        #endregion

        #region 1RM
        private string _oneRepMaxLabel;
        public string OneRepMaxLabel
        {
            get { return _oneRepMaxLabel; }
            set { _oneRepMaxLabel = value; RaisePropertyChanged(() => OneRepMaxLabel); }
        }

        private string _fiftyValueLabel;
        public string FiftyValueLabel
        {
            get { return _fiftyValueLabel; }
            set { _fiftyValueLabel = value; RaisePropertyChanged(() => FiftyValueLabel); }
        }

        private string _seventyValueLabel;
        public string SeventyValueLabel
        {
            get { return _seventyValueLabel; }
            set { _seventyValueLabel = value; RaisePropertyChanged(() => SeventyValueLabel); }
        }

        private string _eightyValueLabel;
        public string EightyValueLabel
        {
            get { return _eightyValueLabel; }
            set { _eightyValueLabel = value; RaisePropertyChanged(() => EightyValueLabel); }
        }

        private string _ninetyValueLabel;
        public string NinetyValueLabel
        {
            get { return _ninetyValueLabel; }
            set { _ninetyValueLabel = value; RaisePropertyChanged(() => NinetyValueLabel); }
        }

        private string _ninetyFiveValueLabel;
        public string NinetyFiveValueLabel
        {
            get { return _ninetyFiveValueLabel; }
            set { _ninetyFiveValueLabel = value; RaisePropertyChanged(() => NinetyFiveValueLabel); }
        }

        private bool _hasOneRepMaxData;
        public bool HasOneRepMaxData
        {
            get { return _hasOneRepMaxData; }
            set { _hasOneRepMaxData = value; RaisePropertyChanged(() => HasOneRepMaxData); }
        }
        #endregion

        #region Generic/Shared
        private bool _showNotEnoughData;
        public bool ShowNotEnoughData
        {
            get { return _showNotEnoughData; }
            set { _showNotEnoughData = value; RaisePropertyChanged(() => ShowNotEnoughData); }
        }

        private int _numberOfWorkouts;
        public int NumberOfWorkouts
        {
            get { return _numberOfWorkouts; }
            set { _numberOfWorkouts = value; RaisePropertyChanged(() => NumberOfWorkouts); }
        }

        private int _numberOfSets;
        public int NumberOfSets
        {
            get { return _numberOfSets; }
            set { _numberOfSets = value; RaisePropertyChanged(() => NumberOfSets); }
        }
        #endregion

        #region Time
        private TimeSpan _minTime;
        public TimeSpan MinTime
        {
            get { return _minTime; }
            set { _minTime = value; RaisePropertyChanged(() => MinTime); }
        }

        private TimeSpan _avgTime;
        public TimeSpan AverageTime
        {
            get { return _avgTime; }
            set { _avgTime = value; RaisePropertyChanged(() => AverageTime); }
        }

        private TimeSpan _maxTime;
        public TimeSpan MaxTime
        {
            get { return _maxTime; }
            set { _maxTime = value; RaisePropertyChanged(() => MaxTime); }
        }
        #endregion

        #region Distance
        private string _longestDistance;
        public string LongestDistance
        {
            get { return _longestDistance; }
            set { _longestDistance = value; RaisePropertyChanged(() => LongestDistance); }
        }

        private string _totalDistance;
        public string TotalDistance
        {
            get { return _totalDistance; }
            set { _totalDistance = value; RaisePropertyChanged(() => TotalDistance); }
        }
        #endregion

        #region Elevation
        private string _highestElevation;
        public string HighestElevation
        {
            get { return _highestElevation; }
            set { _highestElevation = value; RaisePropertyChanged(() => HighestElevation); }
        }

        private string _totalElevation;
        public string TotalElevation
        {
            get { return _totalElevation; }
            set { _totalElevation = value; RaisePropertyChanged(() => TotalElevation); }
        }
        #endregion

        #region Steps
        private string _maxSteps;
        public string MaxSteps
        {
            get { return _maxSteps; }
            set { _maxSteps = value; RaisePropertyChanged(() => MaxSteps); }
        }

        private string _totalSteps;
        public string TotalSteps
        {
            get { return _totalSteps; }
            set { _totalSteps = value; RaisePropertyChanged(() => TotalSteps); }
        }
        #endregion

    }
}
