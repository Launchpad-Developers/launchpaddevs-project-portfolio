using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using System.Collections.Generic;
using ASCommonServices.Enums;
using Prism.Events;
using AS.Forms.Controls.Validation;
using System.Threading.Tasks;
using ASWorkoutTracker.Validation;
using Prism.Services.Dialogs;

namespace ASWorkoutTracker.ViewModels.Support
{
    public class AddNewExerciseTypeViewModel : ASWTModelBaseViewModel<ExerciseType>
    {
        public AddNewExerciseTypeViewModel(INavigationService navigationService,
                                           IDialogService dialogService,
                                           IPageDialogService pageDialogService,
                                           IEventAggregator eventAggregator,
                                           IJsonConsumerService jsonConsumerService)
            : base(jsonConsumerService, eventAggregator, pageDialogService, dialogService)
        {
            _navigationService = navigationService;
            //_session = session;

            Title = "New Exercise Type";

            AddValidations();
        }

        protected override void AddValidations()
        {
            Name = new ValidatableObject<string>();
            Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Exercise name is required." });
            Name.Validations.Add(new UniqueRecordNameRule<string, ExerciseType> { ValidationMessage = "Exercise already exists.", ASWTDatastoreService = _datastoreService });
        }

        protected override async Task<bool> Validate()
        {
            bool isValidName = Name.Validate() && await Name.ValidateAsync();
            return isValidName;
        }

        protected override void SetModelValues()
        {
            Model.Name = Name.Value;
        }
    }
}
