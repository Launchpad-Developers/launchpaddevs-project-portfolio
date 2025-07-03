using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Events;
using AS.Forms.Controls.Validation;
using ASWorkoutTracker.Enums;
using Xamarin.Forms;
using System.Diagnostics;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;
using System.Linq;
using Xamarin.Essentials;

namespace ASWorkoutTracker.ViewModels.Exercises
{
    public class AddEditSetViewModel : ASWTModelBaseViewModel<ExerciseLog>, INavigationAware
    {
        public AddEditSetViewModel(INavigationService navigationService,
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

            CurrentListCollection = new ObservableCollection<ExerciseLog>();

            #region Commands Setup
            AddSet = new DelegateCommand(async () => await OnAddSet());
            AddWeight = new DelegateCommand<string>((value) => OnAddWeight(value));
            AddReps = new DelegateCommand<string>((value) => OnAddReps(value));
            TimeCommand = new DelegateCommand<TimerControlCommands?>(OnTimeCommand);
            AddDistance = new DelegateCommand<string>((value) => OnAddDistance(value));
            AddSteps = new DelegateCommand<string>((value) => OnAddSteps(value));
            AddCalories = new DelegateCommand<string>((value) => OnAddCalories(value));
            AddElevation = new DelegateCommand<string>((value) => OnAddElevation(value));
            AddHeartRate = new DelegateCommand<string>((value) => OnAddHeartRate(value));
            Reset = new DelegateCommand(OnReset);
            EditExerciseLog = new DelegateCommand<ExerciseLog>((item) => OnEditExerciseLog(item));
            NextExerciseCommand = new DelegateCommand(async () => await OnNextExercise());
            SkipRecoveryCommand = new DelegateCommand(OnSkipRecoveryCommand);
            EndExerciseCommand = new DelegateCommand(async () => await OnEndExerciseCommand());
            EndWorkoutCommand = new DelegateCommand(async () => await OnEndWorkoutCommand());
            StartSetCommand = new DelegateCommand(OnStartSetCommand);
            EndSetCommand = new DelegateCommand(async () => await OnEndSetCommand());

            AddValidations();
            #endregion
        }

        #region Commands & Validations

        public ICommand EditExerciseLog { get; }
        public ICommand AddSet { get; }
        public ICommand AddWeight { get; }
        public ICommand AddReps { get; }
        public ICommand AddSteps { get; }
        public ICommand AddDistance { get; }
        public ICommand AddCalories { get; }
        public ICommand AddHeartRate { get; }
        public ICommand AddElevation { get; }
        public ICommand TimeCommand { get; }
        public ICommand Reset { get; }

        public ICommand NextExerciseCommand { get; }
        public ICommand SkipRecoveryCommand { get; }
        public ICommand EndExerciseCommand { get; }
        public ICommand EndWorkoutCommand { get; }
        public ICommand StartSetCommand { get; }
        public ICommand EndSetCommand { get; }


        protected override void AddValidations()
        {
            Weight = new ValidatableObject<double> { DefaultValue = 0.0, Value = 0.0 };
            Weight.Validations.Add(new IsGreaterThanOrEqualToZeroRule<double> { ValidationMessage = "Cannot be negative." });
            Reps = new ValidatableObject<int> { DefaultValue = 0, Value = 0 };
            Reps.Validations.Add(new IsGreaterThanZeroRule<int> { ValidationMessage = "Must be greater than zero." });
            StopWatch = new ValidatableObject<Stopwatch>();
            StopWatch.Validations.Add(new RequiredTimeRule<Stopwatch> { ValidationMessage = "Time is required." });
            StopWatch.Value = new Stopwatch();
            Distance = new ValidatableObject<double> { DefaultValue = 0, Value = 0 };
            Distance.Validations.Add(new IsGreaterThanZeroRule<double> { ValidationMessage = "Must be greater than zero." });
            Steps = new ValidatableObject<int> { DefaultValue = 0, Value = 0 };
            Steps.Validations.Add(new IsGreaterThanOrEqualToZeroRule<int> { ValidationMessage = "Cannot be negative." });
            Calories = new ValidatableObject<int> { DefaultValue = 0, Value = 0 };
            Calories.Validations.Add(new IsGreaterThanOrEqualToZeroRule<int> { ValidationMessage = "Cannot be negative." });
            Elevation = new ValidatableObject<double> { DefaultValue = 0, Value = 0 };
            Elevation.Validations.Add(new IsGreaterThanOrEqualToZeroRule<double> { ValidationMessage = "Cannot be negative." });
            HeartRate = new ValidatableObject<int> { DefaultValue = 0, Value = 0 };
            HeartRate.Validations.Add(new IsGreaterThanOrEqualToZeroRule<int> { ValidationMessage = "Cannot be negative." });
        }

        protected override async Task<bool> Validate()
        {
            bool isValidWeight = !CurrentRoutineExercise.RecordsWeight || Weight.Validate();
            bool isValidRep = !CurrentRoutineExercise.RecordsReps || Reps.Validate();
            bool isValidTime = !CurrentRoutineExercise.RecordsTime || StopWatch.Validate();
            bool isValidDistance = !CurrentRoutineExercise.RecordsDistance || Distance.Validate();
            bool isValidSteps = !CurrentRoutineExercise.RecordsSteps || Steps.Validate();
            bool isValidCalories = !CurrentRoutineExercise.RecordsCalories || Calories.Validate();
            bool isValidElevation = !CurrentRoutineExercise.RecordsElevation || Elevation.Validate();
            bool isValidHeartRate = !CurrentRoutineExercise.RecordsHeartRate || HeartRate.Validate();

            return isValidWeight && isValidRep && isValidTime && isValidDistance && isValidSteps && isValidCalories && isValidElevation && isValidHeartRate;
        }

        #endregion

        public ObservableCollection<ExerciseLog> CurrentListCollection { get; }
                                
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            //DO NOT CALL BASE HERE
            List<ExerciseLog> results = new List<ExerciseLog>();
            RoutineExercises = new List<RoutineExercise>();

            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercise))
            {
                Title = "Add Set";
                RoutineType = RoutineType.SingleExercise;
                CurrentRoutineExercise = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.RoutineExercise);
                CurrentRoutineExercise.IsLast = true;
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercises))
            {
                RoutineType = RoutineType.RoutineWorkout;
                RoutineExercises = parameters.GetValue<List<RoutineExercise>>(AppConstants.ParameterKeys.RoutineExercises);
                if (RoutineExercises.Any())
                {
                    Routine = parameters.GetValue<Routine>(AppConstants.ParameterKeys.Routine);
                    CurrentRoutineExercise = RoutineExercises.Where(x => x.Order == 0).First();
                    CurrentRoutineExercise.IsLast = false;
                }
            }

            KickOffNewExercise();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            TimerRunning = false;
        }

        protected override async Task OnBackCommand()
        {
            await OnEndWorkoutCommand();
        }

        #region Command Methods

        private async Task OnAddSet()
        {
            StopWatch.Value?.Stop();
            TimerRunning = false;

            if (!await Validate())
                return;

            TimeSpan time = TimeSpan.Zero;
            if (CurrentRoutineExercise.TimeToComplete > TimeSpan.Zero)
                time = CurrentRoutineExercise.TimeToComplete.Add(-CountdownTime);
            else if (CurrentRoutineExercise.RecordsTime)
                time = StopWatch.Value.Elapsed;

            DateTime createDate = DateTime.Now;
            ExerciseLog log = new ExerciseLog
            {
                ExerciseID = CurrentRoutineExercise.Exercise.ID,
                RoutineExerciseID = CurrentRoutineExercise.ID,
                RoutineID = CurrentRoutineExercise.RoutineID,
                Reps = Reps.Value,
                Weight = Weight.Value,
                Time = time,
                Steps = Steps.Value,
                Distance = Distance.Value,
                Calories = Calories.Value,
                Elevation = Elevation.Value,
                HeartRate = HeartRate.Value,
                ExerciseDate = createDate,
                DateCreated = createDate,
                DateLastUpdated = createDate,
                MeasurementSystem = (MeasurementSystem)Preferences.Get(AppConstants.App.MEASUREMEMT_SYSTEM, (int)MeasurementSystem.Imperial),
                WeightSystemString = Preferences.Get(AppConstants.App.WEIGHT_SYSTEM, AppConstants.App.DEFAULT_WEIGHT_SYSTEM),
                ElevationSystemString = Preferences.Get(AppConstants.App.ELEVATION_SYSTEM, AppConstants.App.DEFAULT_ELEVATION_SYSTEM),
                DistanceSystemString = Preferences.Get(AppConstants.App.DISTANCE_SYSTEM, AppConstants.App.DEFAULT_DISTANCE_SYSTEM)
                //UserID = _session.User.UserID
            };
            var result = await _datastoreService.Create(log);

            if (result)            
                CurrentListCollection.Add(log);

            await PostSetAction();
        }

        private void OnAddWeight(string value)
        {
            if (double.TryParse(value, out var change))
                Weight.Value = Weight.Value + change;
        }

        private void OnAddReps(string value)
        {
            if (int.TryParse(value, out var change))
                Reps.Value = Reps.Value + change;
        }

        private void OnAddDistance(string value)
        {
            if (float.TryParse(value, out var change))
                Distance.Value = Distance.Value + change;
        }

        private void OnAddSteps(string value)
        {
            if (int.TryParse(value, out var change))
                Steps.Value = Steps.Value + change;
        }

        private void OnAddCalories(string value)
        {
            if (int.TryParse(value, out var change))
                Calories.Value = Steps.Value + change;
        }

        private void OnAddElevation(string value)
        {
            if (int.TryParse(value, out var change))
                Elevation.Value = Steps.Value + change;
        }

        private void OnAddHeartRate(string value)
        {
            if (int.TryParse(value, out var change))
                HeartRate.Value = Steps.Value + change;
        }

        private void OnReset()
        {
            Weight.Reset();
            Reps.Reset();
            Distance.Reset();
            Steps.Reset();
            Calories.Reset();
            Elevation.Reset();
            HeartRate.Reset();

            StopWatch.Value.Stop();
            StopWatch.Value.Reset();
            TimerRunning = false;
        }

        private void OnEditExerciseLog(ExerciseLog log)
        {
            _dialogService.ShowMessage("Not Implemented", "Implement Edit Log");
        }

        private async Task OnNextExercise()
        {
            await MoveToNextExercise();
        }

        private void OnSkipRecoveryCommand()
        {
            CountdownTimerRunning = false;
            EndRecoveryPeriod();
        }

        private async Task OnEndExerciseCommand()
        {
            if (TimerRunning)
            {
                OnTimeCommand(TimerControlCommands.StopTimer);
                await OnAddSet();
            }
            await MoveToNextExercise();
        }

        private async Task OnEndWorkoutCommand()
        {

            if (RoutineExerciseType == RoutineExerciseType.Timed &&
                RoutineState == RoutineState.Timed_InProgress)
            {
                IActionSheetButton saveAction = ActionSheetButton.CreateButton("Save Current Set", new Action(async () => { await OnAddSet(); }));
                IActionSheetButton continueAction = ActionSheetButton.CreateButton("Quit Workout", new Action(async () => { await FinishWorkout(); }));
                IActionSheetButton cancelAction = ActionSheetButton.CreateCancelButton("Cancel", new Action(() => { return; }));

                await _pageDialogService.DisplayActionSheetAsync("Choose Action", saveAction, continueAction, cancelAction);
                return;
            }
            else
            {
                await FinishWorkout();
            }

        }

        private void OnStartSetCommand()
        {
            ExecuteTimedSetTimer();
        }

        private async Task OnEndSetCommand()
        {
            CountdownTimerRunning = false;
            await Task.Delay(1100);
            await OnAddSet();
        }

#endregion


#region Routine Workout Functions

        private void KickOffNewExercise()
        {
            ExerciseName = CurrentRoutineExercise?.Exercise?.Name;

            if (!string.IsNullOrEmpty(CurrentRoutineExercise.SpecialInstructions))
                _dialogService.ShowMessage("Instructions", CurrentRoutineExercise.SpecialInstructions);

            if (CurrentRoutineExercise.TimeToComplete > TimeSpan.Zero)
            {
                CountdownTime = CurrentRoutineExercise.TimeToComplete;
                RoutineExerciseType = RoutineExerciseType.Timed;
                RoutineState = RoutineState.Timed_Start;
            }
            else
            {
                RoutineExerciseType = RoutineExerciseType.Manual;
                RoutineState = RoutineState.Manual_InProgress;
            }

            RaisePropertyChanged(() => CurrentRoutineExercise);
        }

        private async Task MoveToNextExercise()
        {
            CountdownTimerRunning = false;

            var max = RoutineExercises.Max(x => x.Order);

            if (CurrentOrderPosition < max)
            {
                CurrentListCollection.Clear();
                OnReset();

                CurrentOrderPosition++;

                CurrentRoutineExercise = RoutineExercises.Where(x => x.Order == CurrentOrderPosition).FirstOrDefault();
                CurrentRoutineExercise.IsLast = CurrentOrderPosition >= max;

                KickOffNewExercise();
            }
            else
            {
                await FinishWorkout();
            }
        }

        private async Task FinishWorkout()
        {
            //TODO Add call to Summary here
            await _navigationService.GoBackAsync();
        }

        private async Task PostSetAction()
        {
            if (RoutineExercises.Any())
            {
                TimeSpan brake = TimeSpan.Zero; //'break' is a reserved word

                if (CurrentRoutineExercise.TargetNumberOfSets > 1)
                {
                    if (CurrentListCollection.Count < CurrentRoutineExercise.TargetNumberOfSets &&
                        CurrentRoutineExercise.TimeBetweenSets > TimeSpan.Zero)
                    {
                        brake = CurrentRoutineExercise.TimeBetweenSets;
                        RoutineState = RoutineState.Set_Recovery;
                    }
                    else if (CurrentListCollection.Count >= CurrentRoutineExercise.TargetNumberOfSets &&
                             CurrentRoutineExercise.TimeAfterExercise > TimeSpan.Zero)
                    {
                        brake = CurrentRoutineExercise.TimeAfterExercise;
                        RoutineState = RoutineState.Exercise_Recovery;
                    }
                    else
                    {
                        await MoveToNextExercise();
                    }
                }
                else if (CurrentRoutineExercise.TimeAfterExercise > TimeSpan.Zero)
                {
                    brake = CurrentRoutineExercise.TimeAfterExercise;
                    RoutineState = RoutineState.Exercise_Recovery;
                }
                else
                {
                    await MoveToNextExercise();
                }

                if (brake > TimeSpan.Zero)
                {
                    CountdownTime = brake;
                    ExecuteRecoveryTimer();
                }
            }
        }

#endregion


#region Timer Methods

        private void OnTimeCommand(TimerControlCommands? command)
        {
            switch (command.Value)
            {
                case TimerControlCommands.StartTimer:
                    if (StopWatch.Value == null || StopWatch.Value.IsRunning)
                        StopWatch.Value = new Stopwatch();
                    StopWatch.Value.Start();
                    TimerRunning = true;
                    ExecuteTimer();
                    break;
                case TimerControlCommands.PauseTimer:
                    if (StopWatch.Value == null || !StopWatch.Value.IsRunning)
                        return;
                    TimerRunning = false;
                    ExecuteTimer();
                    break;
                case TimerControlCommands.ResumeTimer:
                    if (StopWatch.Value == null || StopWatch.Value.IsRunning)
                        StopWatch.Value = new Stopwatch();
                    StopWatch.Value.Start();
                    TimerRunning = true;
                    ExecuteTimer();
                    break;
                case TimerControlCommands.StopTimer:
                    if (StopWatch.Value == null || !StopWatch.Value.IsRunning)
                        return;
                    StopWatch.Value.Stop();
                    TimerRunning = false;
                    ExecuteTimer();
                    break;
                case TimerControlCommands.ResetTimer:
                    if (StopWatch.Value.IsRunning)
                        StopWatch.Value.Restart();
                    else
                        StopWatch.Value.Reset();
                    RaisePropertyChanged(() => StopWatch);
                    break;
                default:
                    break;
            }
        }

        private void ExecuteTimer()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                RaisePropertyChanged(() => StopWatch);
                return TimerRunning;
            });
        }

        private void ExecuteRecoveryTimer()
        {
            CountdownTimerRunning = true;

            Device.StartTimer(_second, () =>
            {
                CountdownTime = CountdownTime.Subtract(_second);
                RaisePropertyChanged(() => CountdownTime);

                if (CountdownTime <= _zero)
                {
                    CountdownTimerRunning = false;
                    EndRecoveryPeriod();
                }

                return CountdownTimerRunning;
            });
        }

        private void ExecuteTimedSetTimer()
        {
            RoutineState = RoutineState.Timed_InProgress;
            CountdownTimerRunning = true;

            Device.StartTimer(_second, () =>
            {
                CountdownTime = CountdownTime.Subtract(_second);
                RaisePropertyChanged(() => CountdownTime);

                if (CountdownTime <= _zero)
                {
                    CountdownTimerRunning = false;
                    OnAddSet();
                }

                return CountdownTimerRunning;
            });
        }

        private async void EndRecoveryPeriod()
        {
            CountdownTimerRunning = false;
            if (RoutineState == RoutineState.Exercise_Recovery)
            {
                await MoveToNextExercise();
            }
            else if (RoutineExerciseType == RoutineExerciseType.Timed) //Set_Recovery
            {
                RoutineState = RoutineState.Timed_Start;
                CountdownTime = CurrentRoutineExercise.TimeToComplete;
            }
            else
            {
                RoutineState = RoutineState.Manual_InProgress;
            }
        }

        #endregion


        #region Properties


        #region Workflow Properties

        private RoutineType _routineType;
        public RoutineType RoutineType
        {
            get { return _routineType; }
            set { _routineType = value; RaisePropertyChanged(() => RoutineType); }
        }

        private RoutineExerciseType _routineExerciseType;
        public RoutineExerciseType RoutineExerciseType
        {
            get { return _routineExerciseType; }
            set { _routineExerciseType = value; RaisePropertyChanged(() => RoutineExerciseType); }
        }

        private RoutineState _routineState;
        public RoutineState RoutineState
        {
            get { return _routineState; }
            set { _routineState = value; RaisePropertyChanged(() => RoutineState); }
        }

        private int _currentOrderPosition;
        public int CurrentOrderPosition
        {
            get { return _currentOrderPosition; }
            set { _currentOrderPosition = value; RaisePropertyChanged(() => CurrentOrderPosition); }
        }

        private string _exerciseName;
        public string ExerciseName
        {
            get { return _exerciseName; }
            set { _exerciseName = value; RaisePropertyChanged(() => ExerciseName); }
        }

        private RoutineExercise _currentRoutineExercise;
        public RoutineExercise CurrentRoutineExercise
        {
            get { return _currentRoutineExercise; }
            set { _currentRoutineExercise = value; RaisePropertyChanged(() => CurrentRoutineExercise); }
        }

        private Routine _routine;
        public Routine Routine
        {
            get { return _routine; }
            set { _routine = value; RaisePropertyChanged(() => Routine); }
        }

        private List<RoutineExercise> _routineExercises;
        public List<RoutineExercise> RoutineExercises
        {
            get { return _routineExercises; }
            set { _routineExercises = value; RaisePropertyChanged(() => RoutineExercises); }
        }

        //Keep This
        public string DistanceUnits
        {
            get { return Preferences.Get(AppConstants.App.DISTANCE_SYSTEM, AppConstants.App.DEFAULT_DISTANCE_SYSTEM); }
        }
    #endregion


        #region Timer Properties

        private TimeSpan _second = TimeSpan.FromMilliseconds(1000);
        private TimeSpan _zero = TimeSpan.FromMilliseconds(0);

        private bool _timerRunning;
        public bool TimerRunning
        {
            get { return _timerRunning; }
            set { _timerRunning = value; RaisePropertyChanged(() => TimerRunning); }
        }

        private bool _countdownTimerRunning;
        public bool CountdownTimerRunning
        {
            get { return _countdownTimerRunning; }
            set { _countdownTimerRunning = value; RaisePropertyChanged(() => CountdownTimerRunning); }
        }

        private TimeSpan _countdownTime;
        public TimeSpan CountdownTime
        {
            get { return _countdownTime; }
            set { _countdownTime = value; RaisePropertyChanged(() => CountdownTime); }
        }

    #endregion


    #region Metrics Properties

        private ValidatableObject<double> _weight;
        public ValidatableObject<double> Weight
        {
            get { return _weight; }
            set { _weight = value; RaisePropertyChanged(() => Weight); }
        }

        private ValidatableObject<int> _reps;
        public ValidatableObject<int> Reps
        {
            get { return _reps; }
            set { _reps = value; RaisePropertyChanged(() => Reps); }
        }

        private ValidatableObject<Stopwatch> _stopWatch;
        public ValidatableObject<Stopwatch> StopWatch
        {
            get { return _stopWatch; }
            set { _stopWatch = value; RaisePropertyChanged(() => StopWatch); }
        }

        private ValidatableObject<int> _steps;
        public ValidatableObject<int> Steps
        {
            get { return _steps; }
            set { _steps = value; RaisePropertyChanged(() => Steps); }
        }

        private ValidatableObject<double> _distance;
        public ValidatableObject<double> Distance
        {
            get { return _distance; }
            set { _distance = value; RaisePropertyChanged(() => Distance); }
        }

        private ValidatableObject<int> _calories;
        public ValidatableObject<int> Calories
        {
            get { return _calories; }
            set { _calories = value; RaisePropertyChanged(() => Calories); }
        }

        private ValidatableObject<double> _elevation;
        public ValidatableObject<double> Elevation
        {
            get { return _elevation; }
            set { _elevation = value; RaisePropertyChanged(() => Elevation); }
        }

        private ValidatableObject<int> _heartRate;
        public ValidatableObject<int> HeartRate
        {
            get { return _heartRate; }
            set { _heartRate = value; RaisePropertyChanged(() => HeartRate); }
        }

        #endregion

 #endregion

    }
}
