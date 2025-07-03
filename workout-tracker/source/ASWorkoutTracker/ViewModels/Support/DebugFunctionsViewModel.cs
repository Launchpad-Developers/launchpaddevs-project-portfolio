using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Events;
using System;
using Xamarin.Forms.Internals;
using AS.Forms.Controls.ViewModels;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using System.Threading.Tasks;
using ASWorkoutTracker.Repository;
using ASWorkoutTracker.Enums;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;

namespace ASWorkoutTracker.ViewModels.Support
{
    [Preserve(AllMembers = true)]
    public class DebugFunctionsViewModel : BaseViewModel, INavigationAware
    {
        protected IASWTDatastoreService _datastoreService;
        protected IASWTRepositoryService _repositoryService;

        public DebugFunctionsViewModel(INavigationService navigationService,
                                       IDialogService dialogService,
                                       IEventAggregator eventAggregator,
                                       IASWTDatastoreService datastoreService,
                                       IASWTRepositoryService repositoryService,
                                       ISession session) : base()
        {
            EventAggregator = eventAggregator;
            _datastoreService = datastoreService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _repositoryService = repositoryService;
            _session = session;

            FirebaseTestCommand = new DelegateCommand<string>(async (parameter) => await OnFirebaseTest(parameter));
            SQLiteTestCommand = new DelegateCommand<string>(async (parameter) => await OnSQLiteTest(parameter));
        }

        public ICommand FirebaseTestCommand { get; }
        public ICommand SQLiteTestCommand { get; }


        private async Task OnFirebaseTest(string param)
        {
            object result = null;
            var message = string.Empty;
            try
            {
                if (param == "Test")
                    result = await _repositoryService.GetAllSystemData();
                else if (param == "DatabaseInfo")
                    result = await _repositoryService.GetRemoteDatabaseInfoJson();
                else if (param == "ExerciseTypes")
                    result = await _repositoryService.GetExerciseTypes();
                else if (param == "Exercises")
                    result = await _repositoryService.GetExercises();
                else if (param == "Routines")
                    result = await _repositoryService.GetRoutines();
                else if (param == "BodyAreaTypes")
                    result = await _repositoryService.GetBodyAreaTypes();
                else if (param == "RoutineExercises")
                    result = await _repositoryService.GetRoutineExercises();

                message = result.ToString();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("Error", ex.Message);
            }

            _dialogService.ShowMessage("Success", message);
        }

        private async Task OnSQLiteTest(string param)
        {
            object result = true;
            var message = string.Empty;
            try
            {
                if (param == "Test")
                    result = _datastoreService.DatabaseIsIntact();
                else if (param == "CreateDatabase")
                    result = await _datastoreService.RestoreDatabase();
                else if (param == "CheckForUpdates")
                    await _datastoreService.CheckForRemoteDatabaseUpdates();
                else if (param == "RefreshAll")
                    result = await _datastoreService.RefreshDatabase();
                else if (param == "Exercises")
                    result = await _datastoreService.RefreshFirebaseData(FirebaseTable.Exercise, true);
                else if (param == "Routines")
                    result = await _datastoreService.RefreshFirebaseData(FirebaseTable.Routine, true);
                else if (param == "ExerciseTypes")
                    result = await _datastoreService.RefreshFirebaseData(FirebaseTable.ExerciseType, true);
                else if (param == "BodyAreaTypes")
                    result = await _datastoreService.RefreshFirebaseData(FirebaseTable.BodyAreaType, true);
                else if (param == "RoutineExercises")
                    result = await _datastoreService.RefreshFirebaseData(FirebaseTable.RoutineExercise, true);
                else if (param == "EquipmentTypes")
                    result = await _datastoreService.RefreshFirebaseData(FirebaseTable.EquipmentType, true);

                message = result.ToString();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("Error", ex.Message);
            }

            _dialogService.ShowMessage("Success", message);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
