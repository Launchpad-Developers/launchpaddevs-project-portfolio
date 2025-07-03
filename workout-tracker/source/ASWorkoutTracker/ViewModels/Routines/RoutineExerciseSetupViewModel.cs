using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AS.Forms.Controls.Extensions;
using AS.Forms.Controls.Validation;
using ASCommonServices.Interfaces;
using ASCommonServices.Utils;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;

namespace ASWorkoutTracker.ViewModels.Routines
{
    public class RoutineExerciseSetupViewModel : ASWTModelBaseViewModel<RoutineExercise>, INavigationAware
    {
        public RoutineExerciseSetupViewModel(IASWTDatastoreService datastoreService,
                                             IDialogService dialogService,
                                             IPageDialogService pageDialogService,
                                             IEventAggregator eventAggregator,
                                             INavigationService navigationService,
                                             IJsonConsumerService jsonConsumerService)
            : base(jsonConsumerService, eventAggregator, pageDialogService, dialogService)
        {
            _datastoreService = datastoreService;
            _navigationService = navigationService;

            InfoButton = new DelegateCommand<string>((key) => OnInfoButton(key));
            ValidateSpecialInstructions = new DelegateCommand(OnValidateSpecialInstructions);

            StringProperties = new Dictionary<string, string>();
            SetPickerOptions = new ObservableCollection<NumericPickerOption>();

            AddValidations();
        }

        public ICommand InfoButton { get; }
        public ICommand ValidateSpecialInstructions { get; }
        public bool ShowSetsBreak { get { return NumberOfSets > 1; } }
        public Dictionary<string, string> StringProperties { get; set; }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercise))
            {
                Model = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.RoutineExercise);
                EditModel = GenericClone.Clone(Model);
                Title = "Exercise Setup";
                SetupDataForView();
            }
            else
            {
                await _navigationService.GoBackAsync(new NavigationParameters(), useModalNavigation: true);
                return;
            }
        }

        protected void SetupDataForView()
        {
            var setPickerOptions = Enumerable.Range(1, 10).Select(x => new NumericPickerOption { Value = x }).ToList();
            foreach (var option in setPickerOptions)
                SetPickerOptions.Add(option);

            SetsPickerSelectedIndex = !IsEdit ? 0 : SetPickerOptions.IndexOf(new NumericPickerOption { Value = Model.TargetNumberOfSets });
            SetsPickerSelectedItem = SetPickerOptions[SetsPickerSelectedIndex];

            TimeAfterExercise = Model.TimeAfterExercise;
            TimeBetweenSets = Model.TimeBetweenSets;
            TimeToComplete = Model.TimeToComplete;
            NumberOfSets = Model.TargetNumberOfSets;

            HasCalories = Model.RecordsCalories;
            HasDistance = Model.RecordsDistance;
            HasElevation = Model.RecordsElevation;
            HasHeartRate = Model.RecordsHeartRate;
            HasReps = Model.RecordsReps;
            HasSteps = Model.RecordsSteps;
            HasTime = Model.RecordsTime;
            HasWeight = Model.RecordsWeight;

            SpecialInstructions.Value = Model.SpecialInstructions;

            if (StringProperties == null || !StringProperties.Any())
            {
                StringProperties = new Dictionary<string, string>();

                StringProperties.Add("SubTitle", "How will this exercise fit into your routine?");

                StringProperties.Add("ControlTitle", "Control");
                StringProperties.Add("ControlSubText", "Who will be in the driver seat?");

                StringProperties.Add("TimingTitle", "Timing");
                StringProperties.Add("TimeTitle", "Time");
                StringProperties.Add("TimeSubText", "Set a minimum time?");

                StringProperties.Add("MetricsTitle", "Metrics");

                StringProperties.Add("SetsSectionTitle", "Sets");
                StringProperties.Add("SetsTitle", "Number of Sets");
                StringProperties.Add("SetsSubText", "Target number of Sets");
                StringProperties.Add("TimeBetweenSetsInstructions", "How much recovery time is needed between sets?");

                StringProperties.Add("BreakTitle", "Break");
                StringProperties.Add("BreakCellTitle", "Downtime");
                StringProperties.Add("BreakSubText", "Take a break after this exercise?");
                StringProperties.Add("BreakInstructions", "How much recovery time is needed for this exercise?");
            }
            RaisePropertyChanged(() => StringProperties);
        }

        protected override void SetModelValues()
        {
            Model.RecordsCalories = HasCalories;
            Model.RecordsDistance = HasDistance;
            Model.RecordsElevation = HasElevation;
            Model.RecordsHeartRate = HasHeartRate;
            Model.RecordsReps = HasReps;
            Model.RecordsSteps = HasSteps;
            Model.RecordsTime = HasTime;
            Model.RecordsWeight = HasWeight;

            Model.SpecialInstructions = SpecialInstructions.Value;

            Model.TimeAfterExercise = TimeAfterExercise;
            Model.TimeBetweenSets = TimeBetweenSets;
            Model.TimeToComplete = TimeToComplete;
            Model.TargetNumberOfSets = NumberOfSets;
        }

        protected override void AddValidations()
        {
            SpecialInstructions = new ValidatableObject<string>();
            SpecialInstructions.Validations.Add(new MaxLengthRule<string> { MaxLength = 300 });
        }

        #region Command Methods

        private void OnValidateSpecialInstructions()
        {
            _specialInstructions.Validate();
        }

        protected override async Task OnSave()
        {
            SetModelValues();

            await _datastoreService.Update(Model);

            NavigationParameters parameters = new NavigationParameters { { AppConstants.ParameterKeys.RoutineExercise, Model } };
            await _navigationService.GoBackAsync(parameters, useModalNavigation: true);
        }

        private void OnInfoButton(string key)
        {
            _dialogService.ShowHtmlMessage(StringProperties[$"{key}Title"], $"<span style='font-size: 1.5em;'>{StringProperties[$"{key}Instruction"]}</span>");
        }

        #endregion

        #region Properties

        private TimeSpan _timeToComplete;
        public TimeSpan TimeToComplete
        {
            get { return _timeToComplete; }
            set { _timeToComplete = value; RaisePropertyChanged(() => TimeToComplete); }
        }

        private int _setsPickerSelectedIndex;
        public int SetsPickerSelectedIndex
        {
            get { return _setsPickerSelectedIndex; }
            set { _setsPickerSelectedIndex = value; RaisePropertyChanged(() => SetsPickerSelectedIndex); }
        }
        private NumericPickerOption _setsPickerSelectedItem;
        public NumericPickerOption SetsPickerSelectedItem
        {
            get { return _setsPickerSelectedItem; }
            set
            {
                _setsPickerSelectedItem = value;
                NumberOfSets = value.Value;
                RaisePropertyChanged(() => SetsPickerSelectedItem);
            }
        }
        private ObservableCollection<NumericPickerOption> _setPickerOptions;
        public ObservableCollection<NumericPickerOption> SetPickerOptions
        {
            get { return _setPickerOptions; }
            set { _setPickerOptions = value; RaisePropertyChanged(() => SetPickerOptions); }
        }
        private int _numberOfSets;
        public int NumberOfSets
        {
            get { return _numberOfSets; }
            set { _numberOfSets = value; RaisePropertyChanged(() => NumberOfSets); RaisePropertyChanged(() => ShowSetsBreak); }
        }
        private TimeSpan _timeBetweenSets;
        public TimeSpan TimeBetweenSets
        {
            get { return _timeBetweenSets; }
            set { _timeBetweenSets = value; RaisePropertyChanged(() => TimeBetweenSets); }
        }

        private TimeSpan _timeAfterExercise;
        public TimeSpan TimeAfterExercise
        {
            get { return _timeAfterExercise; }
            set { _timeAfterExercise = value; RaisePropertyChanged(() => TimeAfterExercise); }
        }

        private ValidatableObject<string> _specialInstructions;
        public ValidatableObject<string> SpecialInstructions
        {
            get { return _specialInstructions; }
            set { _specialInstructions = value; RaisePropertyChanged(() => SpecialInstructions); }
        }

        private bool _hasWeight;
        public bool HasWeight
        {
            get { return _hasWeight; }
            set
            {
                _hasWeight = value;
                RaisePropertyChanged(() => HasWeight);
                if (value)
                {
                    _hasReps = true;
                    RaisePropertyChanged(() => HasReps);

                    _hasSteps = false;
                    RaisePropertyChanged(() => HasSteps);
                }
            }
        }

        private bool _hasReps;
        public bool HasReps
        {
            get { return _hasReps; }
            set
            {
                _hasReps = value;
                RaisePropertyChanged(() => HasReps);
                if (value)
                {
                    _hasWeight = true;
                    RaisePropertyChanged(() => HasWeight);

                    _hasSteps = false;
                    RaisePropertyChanged(() => HasSteps);
                }
            }
        }

        private bool _hasDistance;
        public bool HasDistance
        {
            get { return _hasDistance; }
            set
            {
                _hasDistance = value;
                RaisePropertyChanged(() => HasDistance);
                if (value)
                {
                    _hasElevation = false;
                    RaisePropertyChanged(() => HasElevation);
                }
            }
        }

        private bool _hasTime;
        public bool HasTime
        {
            get { return _hasTime; }
            set { _hasTime = value; RaisePropertyChanged(() => HasTime); }
        }

        private bool _hasHeartRate;
        public bool HasHeartRate
        {
            get { return _hasHeartRate; }
            set { _hasHeartRate = value; RaisePropertyChanged(() => HasHeartRate); }
        }

        private bool _hasCalories;
        public bool HasCalories
        {
            get { return _hasCalories; }
            set { _hasCalories = value; RaisePropertyChanged(() => HasCalories); }
        }

        private bool _hasSteps;
        public bool HasSteps
        {
            get { return _hasSteps; }
            set
            {
                _hasSteps = value;
                RaisePropertyChanged(() => HasSteps);
                if (value)
                {
                    _hasWeight = false;
                    RaisePropertyChanged(() => HasWeight);

                    _hasReps = false;
                    RaisePropertyChanged(() => HasReps);
                }
            }
        }

        private bool _hasElevation;
        public bool HasElevation
        {
            get { return _hasElevation; }
            set
            {
                _hasElevation = value;
                RaisePropertyChanged(() => HasElevation);
                if (value)
                {
                    _hasDistance = false;
                    RaisePropertyChanged(() => HasDistance);

                    _hasReps = false;
                    RaisePropertyChanged(() => HasReps);

                    _hasWeight = false;
                    RaisePropertyChanged(() => HasWeight);
                }
            }
        }

        #endregion

    }
}
