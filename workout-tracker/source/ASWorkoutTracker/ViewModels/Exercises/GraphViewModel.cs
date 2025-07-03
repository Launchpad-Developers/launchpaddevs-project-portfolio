using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Events;
using ASCommonServices.Events;
using System.Collections.Generic;
using System;
using ASCommonServices.Models;
using Prism.Services.Dialogs;

namespace ASWorkoutTracker.ViewModels.Exercises
{
    public class GraphViewModel : ASWTModelBaseViewModel<ExerciseLog>, INavigationAware
    {
        public GraphViewModel(INavigationService navigationService,
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

            Title = "Graph";

            ShowNotEnoughData = true;
        }

        public Exercise Exercise { get; set; }
        public RoutineExercise RoutineExercise { get; set; }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            //DO NOT CALL BASE HERE
            SetLoadingState(true, "Loading Data");

            if (parameters.ContainsKey(AppConstants.ParameterKeys.Exercise))
            {
                Exercise = parameters.GetValue<Exercise>(AppConstants.ParameterKeys.Exercise);
            }

            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercise))
            { 
                RoutineExercise = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.RoutineExercise);
                if (parameters.ContainsKey(AppConstants.ParameterKeys.GroupedResults))
                {
                    var results = parameters.GetValue<List<Grouping<DateTime, ExerciseLog>>>(AppConstants.ParameterKeys.GroupedResults);
                    ChartData = new ChartData(results, RoutineExercise);
                    ShowNotEnoughData = results.Count <= 2;
                    RaisePropertyChanged(() => ChartData);
                }
            }

            SetLoadingState(false, string.Empty);
        }

        #region ChartData
        private ChartData _chartData;
        public ChartData ChartData
        {
            get { return _chartData; }
            set { _chartData = value; RaisePropertyChanged(() => ChartData); }
        }

        private bool _showNotEnoughData;
        public bool ShowNotEnoughData
        {
            get { return _showNotEnoughData; }
            set { _showNotEnoughData = value; RaisePropertyChanged(() => ShowNotEnoughData); }
        }
        #endregion
    }
}
