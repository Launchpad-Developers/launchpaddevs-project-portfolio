using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Events;
using AS.Forms.Controls.Validation;
using System.Windows.Input;
using Prism.Commands;
using System.Threading.Tasks;
using System;
using ASWorkoutTracker.Validation;
using System.Collections.ObjectModel;
using AS.Forms.Controls.Models;
using ASWorkoutTracker.Enums;
using System.Linq;
using Newtonsoft.Json;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Constants;
using ASCommonServices.Models;

namespace ASWorkoutTracker.ViewModels.Routines
{
    public class AddEditRoutineViewModel : ASWTModelBaseViewModel<Routine>, INavigationAware
    {
        public AddEditRoutineViewModel(INavigationService navigationService,
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

            //RoutineTypes = new ObservableCollection<RoutineType>();

            ValidateBriefDescription = new DelegateCommand(OnValidateBriefDescription);
            ValidateDetailedDescription = new DelegateCommand(OnValidateDetailedDescription);
            PickerItemSelected = new DelegateCommand<PickerDefinition>((definition) => OnPickerItemSelected(definition));

            AddValidations();
        }

        public ICommand ValidateBriefDescription { get; }
        public ICommand ValidateDetailedDescription { get; }
        public ICommand PickerItemSelected { get; }

        #region Validation
        protected override void AddValidations()
        {
            Name = new ValidatableObject<string>();
            Name.Validations.Add(new StringLengthBetweenRule<string> { ValidationMessage = "Routine name is required.", MinLength = 3, MaxLength = 50 });
            Name.Validations.Add(new UniqueRecordNameRule<string, Routine> { ValidationMessage = "Routine already exists.", ASWTDatastoreService = _datastoreService });

            BriefDescription = new ValidatableObject<string>();
            BriefDescription.Validations.Add(new MaxLengthRule<string> { MaxLength = 500 });

            DetailedDescription = new ValidatableObject<string>();
            DetailedDescription.Validations.Add(new MaxLengthRule<string> { MaxLength = 5000 });

            //SelectedRoutineType = new ValidatableObject<RoutineType>();
            //SelectedRoutineType.Validations.Add(new ValidPickerObjectRule<RoutineType> { ValidationMessage = "Routine type is required." });
        }

        protected override async Task<bool> Validate()
        {
            bool isValidName = Name.Validate();
            bool isUniqueName = CanEditName ? await Name.ValidateAsync() : true;
            bool isValidBrief = BriefDescription.Validate();
            bool isValidDesc = DetailedDescription.Validate();

            return isValidName && isUniqueName && isValidBrief && isValidDesc;
        }

        protected override void SetModelValues()
        {
            Model.DateCreated = DateTime.Now;
            Model.IsSystem = false;

            Model.Name = Name.Value;
            Model.Notes = BriefDescription.Value;
            Model.Description = DetailedDescription.Value;
        }

        protected override void EditModelSetup()
        {
            Name.Value = Model.Name;
            BriefDescription.Value = Model.Notes;
            DetailedDescription.Value = Model.Description;

            //if (Model.RoutineTypeID > 0)
            //    SetPropertyValue<RoutineType>(
            //        AppConstants.App.DEFAULT_ROUTINE_TYPE,
            //        PickerItemPropertyNames.SelectedRoutineType,
            //        false,
            //        RoutineTypes.FirstOrDefault(x => x.ID == Model.RoutineTypeID));
        }
        #endregion

        #region Picker Lists

        //private bool _populating;
        protected override async Task PopulateLists()
        {
            return;
            //SetLoadingState(true, string.Empty);
            //_populating = true;

            //(await _datastoreService.ReadAllRecordsOfType<RoutineType>()).ToList().ForEach(RoutineTypes.Add);
            //SetPropertyValue<RoutineType>(AppConstants.App.DEFAULT_ROUTINE_TYPE, PickerItemPropertyNames.SelectedRoutineType, false);
            //RaisePropertyChanged(() => RoutineTypes);
            
            //_populating = false;
            //SetLoadingState(false, string.Empty);
        }

        protected override void ProcessSelectedItem(INavigationParameters parameters)
        {
            PickerItemPropertyNames selectedObjectProperty = (PickerItemPropertyNames)parameters[Constants.ParameterKeys.Property];
            bool clear = (bool)parameters[Constants.ParameterKeys.Clear];

            //if (selectedObjectProperty == PickerItemPropertyNames.SelectedRoutineType)
            //{
            //    if (clear)
            //    {
            //        SelectedRoutineType.Value = new RoutineType { ID = -1, Name = "-- Select --" };
            //        AppSettings.AddOrUpdateValue(AppConstants.App.DEFAULT_ROUTINE_TYPE, string.Empty);
            //        foreach (var item in RoutineTypes)
            //            item.IsSelected = false;
            //    }
            //    else
            //    {
            //        SelectedRoutineType.Value = (RoutineType)parameters[Constants.ParameterKeys.SelectedItem];
            //        AppSettings.AddOrUpdateValue(AppConstants.App.DEFAULT_ROUTINE_TYPE, JsonConvert.SerializeObject(SelectedRoutineType.Value));
            //        foreach (var item in RoutineTypes)
            //            item.IsSelected = item.ID == SelectedRoutineType.Value.ID;

            //        if (!_populating)
            //            SelectedRoutineType.Validate();
            //    }

            //    SelectedRoutineTypeIndex = RoutineTypes.IndexOf(RoutineTypes.Where(x => x.ID == SelectedRoutineType.Value.ID).FirstOrDefault());
            //    RaisePropertyChanged(() => SelectedRoutineType);
            //}
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

        //public ObservableCollection<RoutineType> RoutineTypes { get; protected set; }
        //protected ValidatableObject<RoutineType> _selectedRoutineType;
        //public ValidatableObject<RoutineType> SelectedRoutineType
        //{
        //    get { return _selectedRoutineType; }
        //    set { _selectedRoutineType = value; RaisePropertyChanged(() => SelectedRoutineType); }
        //}

        private int _selectedRoutineTypeIndex;
        public int SelectedRoutineTypeIndex
        {
            get { return _selectedRoutineTypeIndex; }
            set { _selectedRoutineTypeIndex = value >= 0 ? value : 0; RaisePropertyChanged(() => SelectedRoutineTypeIndex); }
        }

        #endregion
    }
}
