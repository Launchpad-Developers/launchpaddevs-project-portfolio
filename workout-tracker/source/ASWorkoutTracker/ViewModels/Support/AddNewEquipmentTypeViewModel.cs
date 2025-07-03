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
using System.Windows.Input;
using Prism.Services.Dialogs;

namespace ASWorkoutTracker.ViewModels.Support
{
    public class AddNewEquipmentTypeViewModel : ASWTModelBaseViewModel<EquipmentType>
    {
        public AddNewEquipmentTypeViewModel(INavigationService navigationService,
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

            Title = "New Equipment";

            AddValidations();
        }

        protected override void AddValidations()
        {
            Name = new ValidatableObject<string>();
            Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Equipment name is required." });
            Name.Validations.Add(new UniqueRecordNameRule<string, EquipmentType> { ValidationMessage = "Equipment already exists.", ASWTDatastoreService = _datastoreService });
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
