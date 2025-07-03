using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using ASWorkoutTracker.Enums;
using System.Linq;
using System.Threading.Tasks;
using Prism.Events;
using AS.Forms.Controls.Validation;
using System.Windows.Input;
using Prism.Commands;
using AS.Forms.Controls.Models;
using ASWorkoutTracker.Validation;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Constants;
using ASCommonServices.Models;
using System;
using Xamarin.Essentials;

namespace ASWorkoutTracker.ViewModels.Exercises
{
    public class AddEditExerciseViewModel : ASWTModelBaseViewModel<Exercise>, INavigationAware
    {
        public AddEditExerciseViewModel(INavigationService navigationService,
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

            BodyAreaTypes = new ObservableCollection<BodyAreaType>();
            EquipmentTypes = new ObservableCollection<EquipmentType>();
            ExerciseTypes = new ObservableCollection<ExerciseType>();
            Levels = new ObservableCollection<Level>();

            ValidateBriefDescription = new DelegateCommand(OnValidateBriefDescription);
            ValidateDetailedDescription = new DelegateCommand(OnValidateDetailedDescription);
            OpenUrl = new DelegateCommand<string>(async (url) => await OnOpenUrl(url));
            PickerItemSelected = new DelegateCommand<PickerDefinition>((definition) => OnPickerItemSelected(definition));

            AddValidations();
        }

        public ICommand ValidateBriefDescription { get; }
        public ICommand ValidateDetailedDescription { get; }
        public ICommand PickerItemSelected { get; }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(AppConstants.ParameterKeys.Exercise))
            {
                Model = parameters.GetValue<Exercise>(AppConstants.ParameterKeys.Exercise);
                Title = "Details";
                IsReadOnly = true;
            }
            else
            { 
                base.OnNavigatedTo(parameters);
            }
        }

        protected override void EditModelSetup()
        {
            Name.Value = Model.Name;
            BriefDescription.Value = Model.Notes;
            DetailedDescription.Value = Model.Description;
            URL.Value = Model.Url;

            if (Model.BodyAreaTypeID > 0)
                SetPropertyValue<BodyAreaType>(
                    AppConstants.App.DEFAULT_BODY_AREA_TYPE,
                    PickerItemPropertyNames.SelectedBodyAreaType,
                    false,
                    BodyAreaTypes.FirstOrDefault(x => x.ID == Model.BodyAreaTypeID));
            
            if (Model.EquipmentTypeID > 0)
                SetPropertyValue<EquipmentType>(
                    AppConstants.App.DEFAULT_EQUIPMENT_TYPE,
                    PickerItemPropertyNames.SelectedEquipmentType,
                    false,
                    EquipmentTypes.FirstOrDefault(x => x.ID == Model.EquipmentTypeID));

            if (Model.ExerciseTypeID > 0)
                SetPropertyValue<ExerciseType>(
                    AppConstants.App.DEFAULT_EXERCISE_TYPE,
                    PickerItemPropertyNames.SelectedExerciseType,
                    false,
                    ExerciseTypes.FirstOrDefault(x => x.ID == Model.ExerciseTypeID));

            if (Model.LevelID > 0)
                SetPropertyValue<Level>(
                    AppConstants.App.DEFAULT_LEVEL,
                    PickerItemPropertyNames.SelectedLevel,
                    false,
                    Levels.FirstOrDefault(x => x.ID == Model.LevelID));
        }

        private bool _populating;
        protected override async Task PopulateLists()
        {
            SetLoadingState(true, string.Empty);
            _populating = true;

            (await _datastoreService.ReadAllRecordsOfType<ExerciseType>()).ToList().ForEach(ExerciseTypes.Add);
            SetPropertyValue<ExerciseType>(AppConstants.App.DEFAULT_EXERCISE_TYPE, PickerItemPropertyNames.SelectedExerciseType, false);
            RaisePropertyChanged(() => ExerciseTypes);

            (await _datastoreService.ReadAllRecordsOfType<EquipmentType>()).ToList().ForEach(EquipmentTypes.Add);
            SetPropertyValue<EquipmentType>(AppConstants.App.DEFAULT_EQUIPMENT_TYPE, PickerItemPropertyNames.SelectedEquipmentType, false);
            RaisePropertyChanged(() => EquipmentTypes);

            (await _datastoreService.ReadAllRecordsOfType<BodyAreaType>()).ToList().ForEach(BodyAreaTypes.Add);
            SetPropertyValue<BodyAreaType>(AppConstants.App.DEFAULT_BODY_AREA_TYPE, PickerItemPropertyNames.SelectedBodyAreaType, false);
            RaisePropertyChanged(() => BodyAreaTypes);

            (await _datastoreService.ReadAllRecordsOfType<Level>()).ToList().ForEach(Levels.Add);
            SetPropertyValue<Level>(AppConstants.App.DEFAULT_LEVEL, PickerItemPropertyNames.SelectedLevel, false);
            RaisePropertyChanged(() => Levels);

            _populating = false;
            SetLoadingState(false, string.Empty);
        }

        #region Pickers
        
        protected override void OnPickerItemSelected(PickerDefinition definition)
        {
            if (definition == null) return;
            base.OnPickerItemSelected(definition);
        }

        protected override void ProcessSelectedItem(INavigationParameters parameters)
        {
            PickerItemPropertyNames selectedObjectProperty = (PickerItemPropertyNames)parameters[Constants.ParameterKeys.Property];
            bool clear = (bool)parameters[Constants.ParameterKeys.Clear];

            if (selectedObjectProperty == PickerItemPropertyNames.SelectedEquipmentType)
            {
                if (clear)
                {
                    SelectedEquipmentType.Value = new EquipmentType { ID = -1, Name = "-- Select --" };
                    Preferences.Set(AppConstants.App.DEFAULT_EQUIPMENT_TYPE, string.Empty);
                    foreach (var item in EquipmentTypes)
                        item.IsSelected = false;
                }
                else
                {
                    SelectedEquipmentType.Value = (EquipmentType)parameters[Constants.ParameterKeys.SelectedItem];
                    Preferences.Set(AppConstants.App.DEFAULT_EQUIPMENT_TYPE, JsonConvert.SerializeObject(SelectedEquipmentType.Value));
                    foreach (var item in EquipmentTypes)
                        item.IsSelected = item.ID == SelectedEquipmentType.Value.ID;

                    if (!_populating)
                        SelectedEquipmentType.Validate();
                }

                SelectedEquipmentTypeIndex = EquipmentTypes.IndexOf(EquipmentTypes.Where(x => x.ID == SelectedEquipmentType.Value.ID).FirstOrDefault());
                RaisePropertyChanged(() => SelectedEquipmentType);
            }
            else if (selectedObjectProperty == PickerItemPropertyNames.SelectedBodyAreaType)
            {
                if (clear)
                {
                    SelectedBodyAreaType.Value = new BodyAreaType { ID = -1, Name = "-- Select --" };
                    Preferences.Set(AppConstants.App.DEFAULT_BODY_AREA_TYPE, string.Empty);
                    foreach (var item in BodyAreaTypes)
                        item.IsSelected = false;
                }
                else
                {
                    SelectedBodyAreaType.Value = (BodyAreaType)parameters[Constants.ParameterKeys.SelectedItem];
                    Preferences.Set(AppConstants.App.DEFAULT_BODY_AREA_TYPE, JsonConvert.SerializeObject(SelectedBodyAreaType.Value));
                    foreach (var item in BodyAreaTypes)
                        item.IsSelected = item.ID == SelectedBodyAreaType.Value.ID;

                    if (!_populating)
                        SelectedBodyAreaType.Validate();
                }

                SelectedBodyAreaTypeIndex = BodyAreaTypes.IndexOf(BodyAreaTypes.Where(x => x.ID == SelectedBodyAreaType.Value.ID).FirstOrDefault());
                RaisePropertyChanged(() => SelectedBodyAreaType);
            }
            else if (selectedObjectProperty == PickerItemPropertyNames.SelectedExerciseType)
            {
                if (clear)
                {
                    SelectedExerciseType.Value = new ExerciseType { ID = -1, Name = "-- Select --" };
                    Preferences.Set(AppConstants.App.DEFAULT_EXERCISE_TYPE, string.Empty);
                    foreach (var item in ExerciseTypes)
                        item.IsSelected = false;
                }
                else
                {
                    SelectedExerciseType.Value = (ExerciseType)parameters[Constants.ParameterKeys.SelectedItem];
                    Preferences.Set(AppConstants.App.DEFAULT_EXERCISE_TYPE, JsonConvert.SerializeObject(SelectedExerciseType.Value));
                    foreach (var item in ExerciseTypes)
                        item.IsSelected = item.ID == SelectedExerciseType.Value.ID;

                    if (!_populating)
                        SelectedExerciseType.Validate();
                }

                SelectedExerciseTypeIndex = ExerciseTypes.IndexOf(ExerciseTypes.Where(x => x.ID == SelectedExerciseType.Value.ID).FirstOrDefault());
                RaisePropertyChanged(() => SelectedExerciseType);
            }
            else if (selectedObjectProperty == PickerItemPropertyNames.SelectedLevel)
            {
                if (clear)
                {
                    SelectedLevel.Value = new Level { ID = -1, Name = "-- Select --" };
                    Preferences.Set(AppConstants.App.DEFAULT_LEVEL, string.Empty);
                    foreach (var item in Levels)
                        item.IsSelected = false;
                }
                else
                {
                    SelectedLevel.Value = (Level)parameters[Constants.ParameterKeys.SelectedItem];
                    Preferences.Set(AppConstants.App.DEFAULT_LEVEL, JsonConvert.SerializeObject(SelectedLevel.Value));
                    foreach (var item in Levels)
                        item.IsSelected = item.ID == SelectedLevel.Value.ID;

                    if (!_populating)
                        SelectedLevel.Validate();
                }

                SelectedLevelIndex = Levels.IndexOf(Levels.Where(x => x.ID == SelectedLevel.Value.ID).FirstOrDefault());
                RaisePropertyChanged(() => SelectedLevel);
            }
        }
        #endregion

        #region Validation & Save
        protected override async Task OnSave()
        {
            if (!await Validate())
                return;

            SetModelValues();

            bool result = false;
            var model = Model as BaseSQLModel;

            if (model.ID > 0)
            {
                await _datastoreService.Update(Model);
                var routineExercise = await _datastoreService.ReadDefaultRoutineExercise(Model.ID);
                await _navigationService.GoBackAsync(
                    new NavigationParameters { { AppConstants.ParameterKeys.SelectedRoutineExercise, routineExercise } });
            }
            else
            {
                result = await _datastoreService.Create(Model);

                if (result)
                {
                    RoutineExercise routineExercise = new RoutineExercise
                    {
                        Exercise = Model,
                        ExerciseID = Model.ID,
                        RoutineID = AppConstants.App.DEFAULT_ROUTINE_ID,
                        DateCreated = DateTime.Today
                    };
                    var saveResult = await _datastoreService.Create(routineExercise);

                    if (saveResult)
                    {
                        await _navigationService.GoBackAsync(
                            new NavigationParameters { { AppConstants.ParameterKeys.SelectedRoutineExercise, routineExercise } });
                    }
                }
            }
        }

        protected override async Task<bool> Validate()
        {
            bool isValidName = Name.Validate();
            bool isUniqueName = CanEditName ? await Name.ValidateAsync() : true;
            bool isValidUrl = await URL.ValidateAsync();
            bool isValidExercise = SelectedExerciseType.Validate();
            bool isValidEquipment = SelectedEquipmentType.Validate();
            bool isValidLevel = SelectedLevel.Validate();
            bool isValidBody = SelectedBodyAreaType.Validate();
            bool isValidBrief = BriefDescription.Validate();
            bool isValidDesc = DetailedDescription.Validate();

            return (isValidName && isUniqueName && isValidUrl && isValidExercise && isValidEquipment && isValidLevel && isValidBody && isValidBrief && isValidDesc);
        }

        protected override void SetModelValues()
        {
            Model.Url = string.IsNullOrEmpty(URL.Value) ? "User Defined" : URL.Value;
            Model.IsSystem = false;

            Model.BodyAreaTypeID = SelectedBodyAreaType.Value.ID;
            Model.ExerciseTypeID = SelectedExerciseType.Value.ID;
            Model.EquipmentTypeID = SelectedEquipmentType.Value.ID;
            Model.LevelID = SelectedLevel.Value.ID;

            Model.Name = Name.Value;
            Model.Notes = BriefDescription.Value;
            Model.Description = DetailedDescription.Value;
        }

        protected override void AddValidations()
        {
            Name = new ValidatableObject<string>();
            Name.Validations.Add(new StringLengthBetweenRule<string> { ValidationMessage = "Exercise name is required.", MinLength = 3, MaxLength = 50 });
            Name.Validations.Add(new UniqueRecordNameRule<string, Exercise> { ValidationMessage = "Exercise already exists.", ASWTDatastoreService = _datastoreService });

            URL = new ValidatableObject<string>();
            URL.Validations.Add(new ValidUrlRule<string> { ValidationMessage = "URL is not valid." });

            SelectedExerciseType = new ValidatableObject<ExerciseType>();
            SelectedExerciseType.Validations.Add(new ValidPickerObjectRule<ExerciseType> { ValidationMessage = "Exercise type is required." });

            SelectedEquipmentType = new ValidatableObject<EquipmentType>();
            SelectedEquipmentType.Validations.Add(new ValidPickerObjectRule<EquipmentType> { ValidationMessage = "Equipment type is required." });

            SelectedBodyAreaType = new ValidatableObject<BodyAreaType>();
            SelectedBodyAreaType.Validations.Add(new ValidPickerObjectRule<BodyAreaType> { ValidationMessage = "Body area is required." });

            SelectedLevel = new ValidatableObject<Level>();
            SelectedLevel.Validations.Add(new ValidPickerObjectRule<Level> { ValidationMessage = "Level is required." });

            BriefDescription = new ValidatableObject<string>();
            BriefDescription.Validations.Add(new MaxLengthRule<string> { MaxLength = 500 });

            DetailedDescription = new ValidatableObject<string>();
            DetailedDescription.Validations.Add(new MaxLengthRule<string> { MaxLength = 5000 });
        }
        #endregion

        #region Command Methods
        private void OnValidateBriefDescription()
        {
            _briefDescription.Validate();
        }

        private void OnValidateDetailedDescription()
        {
            _detailedDescription.Validate();
        }
        #endregion

        #region Properties

        protected ValidatableObject<string> _url;
        public ValidatableObject<string> URL
        {
            get { return _url; }
            set { _url = value; RaisePropertyChanged(() => URL); }
        }

        public ObservableCollection<BodyAreaType> BodyAreaTypes { get; protected set; }
        protected ValidatableObject<BodyAreaType> _selectedBodyAreaType;
        public ValidatableObject<BodyAreaType> SelectedBodyAreaType
        {
            get { return _selectedBodyAreaType; }
            set { _selectedBodyAreaType = value; RaisePropertyChanged(() => SelectedBodyAreaType); }
        }

        private int _selectedBodyAreaTypeIndex;
        public int SelectedBodyAreaTypeIndex
        {
            get { return _selectedBodyAreaTypeIndex; }
            set { _selectedBodyAreaTypeIndex = value >= 0 ? value : 0; RaisePropertyChanged(() => SelectedBodyAreaTypeIndex); }
        }

        public ObservableCollection<EquipmentType> EquipmentTypes { get; protected set; }
        protected ValidatableObject<EquipmentType> _selectedEquipmentType;
        public ValidatableObject<EquipmentType> SelectedEquipmentType
        {
            get { return _selectedEquipmentType; }
            set { _selectedEquipmentType = value; RaisePropertyChanged(() => SelectedEquipmentType); }
        }

        private int _selectedEquipmentTypeIndex;
        public int SelectedEquipmentTypeIndex
        {
            get { return _selectedEquipmentTypeIndex; }
            set { _selectedEquipmentTypeIndex = value >= 0 ? value : 0; RaisePropertyChanged(() => SelectedEquipmentTypeIndex); }
        }

        public ObservableCollection<ExerciseType> ExerciseTypes { get; protected set; }
        protected ValidatableObject<ExerciseType> _selectedExerciseType;
        public ValidatableObject<ExerciseType> SelectedExerciseType
        {
            get { return _selectedExerciseType; }
            set { _selectedExerciseType = value; RaisePropertyChanged(() => SelectedExerciseType); }
        }

        private int _selectedExerciseTypeIndex;
        public int SelectedExerciseTypeIndex
        {
            get { return _selectedExerciseTypeIndex; }
            set { _selectedExerciseTypeIndex = value >= 0 ? value : 0; RaisePropertyChanged(() => SelectedExerciseTypeIndex); }
        }

        public ObservableCollection<Level> Levels { get; protected set; }
        protected ValidatableObject<Level> _selectedLevel;
        public ValidatableObject<Level> SelectedLevel
        {
            get { return _selectedLevel; }
            set { _selectedLevel = value; RaisePropertyChanged(() => SelectedLevel); }
        }

        private int _selectedLevelIndex;
        public int SelectedLevelIndex
        {
            get { return _selectedLevelIndex; }
            set { _selectedLevelIndex = value >= 0 ? value : 0; RaisePropertyChanged(() => SelectedLevelIndex); }
        }

        #endregion
    }
}
