using Prism.Navigation;
using Prism.Services;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Events;
using AS.Forms.Controls.Validation;
using System.Threading.Tasks;
using ASWorkoutTracker.Validation;
using System.Windows.Input;
using Prism.Services.Dialogs;

namespace ASWorkoutTracker.ViewModels.Support
{
    public class AddNewBodyAreaTypeViewModel : ASWTModelBaseViewModel<BodyAreaType>
    {
        public AddNewBodyAreaTypeViewModel(INavigationService navigationService,
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

            Title = "New Body Area";

            AddValidations();
        }

        protected override void AddValidations()
        {
            Name = new ValidatableObject<string>();
            Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Body area name is required." });
            Name.Validations.Add(new UniqueRecordNameRule<string, BodyAreaType> { ValidationMessage = "Body area already exists.", ASWTDatastoreService = _datastoreService });
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
