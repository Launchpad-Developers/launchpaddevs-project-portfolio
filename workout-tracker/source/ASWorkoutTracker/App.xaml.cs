using System;
using System.Globalization;
using System.Reflection;
using Prism.Unity;
using Prism.Ioc;
using Prism;
using Prism.Mvvm;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Diagnostics;
using ASWorkoutTracker.Views.Navigation;
using ASWorkoutTracker.ViewModels.Navigation;
using ASWorkoutTracker.Views.Login;
using ASWorkoutTracker.ViewModels.Login;
using ASWorkoutTracker.Views.Support;
using ASWorkoutTracker.ViewModels.Support;
using ASWorkoutTracker.Views.Exercises;
using ASWorkoutTracker.ViewModels.Exercises;
using ASWorkoutTracker.Views.Routines;
using ASWorkoutTracker.ViewModels.Routines;
using AS.Forms.Controls.Views;
using AS.Forms.Controls.ViewModels;
using ASCommonServices;
using ASCommonServices.Interfaces;
using Unity;
using AS.Forms.Controls.Converters;
using ASWorkoutTracker.Datastore;
using ASCommonServices.Services;
using Prism.Navigation;

namespace ASWorkoutTracker
{
    public partial class App : PrismApplication
    {
        private string AppCenterAndroid;
        private string AppCenteriOS;
        private string SyncfusionLicense;
        protected IFirebaseAuthentication _firebaseAuthentication;
        //protected IFederatedAuthentication _federatedAuthentication;

        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            return;
        }

        protected async override void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionLicense);
            InitializeComponent();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(FindViewModel);

            AppCenter.Start($"android={AppCenterAndroid};ios={AppCenteriOS}",
                typeof(Analytics),
                typeof(Crashes));

            var id = await AppCenter.GetInstallIdAsync();
            Debug.WriteLine($"UUID: {id}");

            INavigationResult result = new NavigationResult();

            if (_firebaseAuthentication.IsLoggedIn())
                result = await NavigationService.NavigateAsync($"/{AppConstants.Navigation.Master}/{AppConstants.Navigation.Nav}/{AppConstants.Navigation.Exercises}");
            else
                result = await NavigationService.NavigateAsync($"/{AppConstants.Navigation.LoginNav}/{AppConstants.Navigation.Login}");

            return;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            CommonServicesInitializer.RegisterTypes(containerRegistry);
            RegisterNavigation(containerRegistry);
            RegisterDependencies(containerRegistry);
        }

        private void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            //Navigation
            containerRegistry.RegisterForNavigation<ASNavigationView, ASNavigationViewModel>(AppConstants.Navigation.Nav);
            containerRegistry.RegisterForNavigation<ASTransparentNavigationView, ASTransparentNavigationViewModel>(AppConstants.Navigation.LoginNav);
            containerRegistry.RegisterForNavigation<ASTabbedView, ASTabbedViewModel>(AppConstants.Navigation.Tab);
            containerRegistry.RegisterForNavigation<ASMasterDetailPage, ASMasterDetailViewModel>(AppConstants.Navigation.Master);

            //Login
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>(AppConstants.Navigation.Login);
            containerRegistry.RegisterForNavigation<ForgotPasswordView, ForgotPasswordViewModel>(AppConstants.Navigation.ForgotPassword);
            containerRegistry.RegisterForNavigation<SignUpView, SignUpViewModel>(AppConstants.Navigation.SignUp);

            //Support
            containerRegistry.RegisterForNavigation<ASPickerView, ASPickerViewModel>(AppConstants.Navigation.Picker);
            containerRegistry.RegisterForNavigation<ASPhotosView, ASPhotosViewModel>(AppConstants.Navigation.Photos);
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>(AppConstants.Navigation.Settings);
            containerRegistry.RegisterForNavigation<AddNewBodyAreaTypeView, AddNewBodyAreaTypeViewModel>(AppConstants.Navigation.AddNewBodyAreaType);
            containerRegistry.RegisterForNavigation<AddNewEquipmentTypeView, AddNewEquipmentTypeViewModel>(AppConstants.Navigation.AddNewEquipmentType);
            containerRegistry.RegisterForNavigation<AddNewExerciseTypeView, AddNewExerciseTypeViewModel>(AppConstants.Navigation.AddNewExerciseType);
            containerRegistry.RegisterForNavigation<DebugFunctionsView, DebugFunctionsViewModel>(AppConstants.Navigation.DebugFunctions);

            //Exercises
            containerRegistry.RegisterForNavigation<AddEditExerciseView, AddEditExerciseViewModel>(AppConstants.Navigation.AddEditExercise);
            containerRegistry.RegisterForNavigation<ExercisesView, ExercisesViewModel>(AppConstants.Navigation.Exercises);
            containerRegistry.RegisterForNavigation<AddEditSetView, AddEditSetViewModel>(AppConstants.Navigation.AddEditSet);
            containerRegistry.RegisterForNavigation<HistoryView, HistoryViewModel>(AppConstants.Navigation.History);
            containerRegistry.RegisterForNavigation<GraphView, GraphViewModel>(AppConstants.Navigation.Graph);

            //Routines
            containerRegistry.RegisterForNavigation<AddRoutineExerciseView, AddRoutineExerciseViewModel>(AppConstants.Navigation.AddRoutineExercise);
            containerRegistry.RegisterForNavigation<RoutineExercisesView, RoutineExercisesViewModel>(AppConstants.Navigation.RoutineExercises);
            containerRegistry.RegisterForNavigation<AddEditRoutineView, AddEditRoutineViewModel>(AppConstants.Navigation.AddEditRoutine);
            containerRegistry.RegisterForNavigation<RoutinesView, RoutinesViewModel>(AppConstants.Navigation.Routines);
            containerRegistry.RegisterForNavigation<RoutineExerciseSetupView, RoutineExerciseSetupViewModel>(AppConstants.Navigation.RoutineExerciseSetup);

            //Dialogs
            containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>(AS.Forms.Controls.Constants.Constants.Navigation.MessageDialogView);
            containerRegistry.RegisterDialog<HtmlDialogView, HtmlDialogViewModel>(AS.Forms.Controls.Constants.Constants.Navigation.HtmlDialogView);
            containerRegistry.RegisterDialog<AlertDialogView, AlertDialogViewModel>(AS.Forms.Controls.Constants.Constants.Navigation.AlertDialogView);
        }

        private void RegisterDependencies(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISession, ASWTSession>();
            containerRegistry.RegisterSingleton<IEnumToStringConverter, EnumToStringConverter>();
            containerRegistry.RegisterSingleton<IFederatedAuthentication, FederatedAuthentication>();

            //Get secret keys
            var container = containerRegistry.GetContainer();
            var jsonConsumerService = container.Resolve<IJsonConsumerService>();
            _firebaseAuthentication = container.Resolve<IFirebaseAuthentication>();
            //_federatedAuthentication = container.Resolve<IFederatedAuthentication>();

            AppCenterAndroid = jsonConsumerService.GetSecretSettingValue(Constants.Misc.AppCenterAndroid, "@ppsm1+h5");
            AppCenteriOS = jsonConsumerService.GetSecretSettingValue(Constants.Misc.AppCenteriOS, "@ppsm1+h5");
            SyncfusionLicense = jsonConsumerService.GetSecretSettingValue(Constants.Misc.SyncfusionSecretKey, "@ppsm1+h5");
        }

        private Type FindViewModel(Type viewType)
        {
            var viewName = viewType.FullName
                .Replace("Page", string.Empty)
                .Replace("Views", "ViewModels")
                .Replace("Edit", string.Empty)
                .Replace("Create", string.Empty);

            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, viewAssemblyName);

            return Type.GetType(viewModelName);
        }
    }
}
