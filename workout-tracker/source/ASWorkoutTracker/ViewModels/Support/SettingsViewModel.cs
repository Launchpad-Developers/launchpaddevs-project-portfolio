using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using AS.Forms.Controls.ViewModels;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Enums;
using System.Linq;
using ASCommonServices.Events;
using Prism.Events;
using AS.Forms.Controls.Models;
using ASCommonServices.Enums;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;
using System;
using Xamarin.Essentials;
using System.Text;
using Plugin.StoreReview;
using AS.Forms.Controls.Constants;

namespace ASWorkoutTracker.ViewModels.Support
{
    public class SettingsViewModel : BaseViewModel, INavigationAware
    {
        protected IASWTDatastoreService _datastoreService;

        public SettingsViewModel(INavigationService navigationService,
                                 IDialogService dialogService,
                                 IAppVersionService appVersionService,
                                 IASWTDatastoreService datastoreService,
                                 IEventAggregator eventAggregator,
                                 IJsonConsumerService jsonConsumerService,
                                 ISession session) : base()
        {
            _appVersionService = appVersionService;
            _datastoreService = datastoreService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            EventAggregator = eventAggregator;
            _session = session;
            _jsonConsumerService = jsonConsumerService;

            AddNewType = new DelegateCommand<object>(async (table) => await OnAddNewType(table));
            RestoreDatabase = new DelegateCommand(async () => await OnRestoreDatabase());
            RefreshDatabase = new DelegateCommand(async () => await OnRefreshDatabase());
            MeasurementOptionChanged = new DelegateCommand<ASRadioButtonOption>(OnMeasurementOptionChanged);
            Feedback = new DelegateCommand(async () => await OnFeedback());
            Rate = new DelegateCommand(OnRate);
            OpenUrl = new DelegateCommand<string>(async (url) => await OnOpenUrl(url));

            Title = "Settings";

            StringProperties = new Dictionary<string, string>();

#if DEBUG
            //ShowUserName = true;
            ShowNewExercise = true;
            ShowNewRoutineType = true;
            ShowNewLevel = true;
#endif
        }

        public ICommand AddNewType { get; }
        public ICommand RestoreDatabase { get; }
        public ICommand RefreshDatabase { get; }
        public ICommand MeasurementOptionChanged { get; }
        public ICommand Feedback { get; }
        public ICommand Rate { get; }

        public List<ASRadioButtonOption> SystemOfMeasurementOptions { get; set; }
        public Dictionary<string, string> StringProperties { get; set; }
        
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            
            EventAggregator.GetEvent<DatastoreUpdatedEvent>().Subscribe(OnDataUpdatedEvent);
            await SetupStats();
            SetupMeasurementOptions();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            EventAggregator.GetEvent<DatastoreUpdatedEvent>().Unsubscribe(OnDataUpdatedEvent);
        }

        protected async void OnDataUpdatedEvent()
        {
            await SetupStats();
        }

        private async Task SetupStats()
        {
            var stats = await _datastoreService.GetStats();
            ShowRating = Preferences.Get(AppConstants.App.DID_RATE_APP, true);

            if (stats != null && stats.Any())
            {
                StringProperties = new Dictionary<string, string>();

                //StringProperties.Add("Username", "User_Name_Here"); //_session.User.Username);
                StringProperties.Add("AppVersion", _appVersionService.VersionNumber);
                StringProperties.Add("DatabaseVersion", (await _datastoreService.GetDatabaseVersion()).ToString());
                StringProperties.Add("ResetSubtext", "Completely erases local data including user data and exercise logs, and restores the database using latest version.");
                StringProperties.Add("RefreshSubtext", "Updates all system data to the latest versions but leaves user defined data intact.");

                StringProperties.Add("BodyAreaRecords", $"{stats["BodyAreaType"].ToString()} Records");
                StringProperties.Add("EquipmentRecords", $"{stats["EquipmentType"].ToString()} Records");
                StringProperties.Add("ExerciseRecords", $"{stats["ExerciseType"].ToString()} Records");
                StringProperties.Add("LevelRecords", $"{stats["Level"].ToString()} Records");
                StringProperties.Add("FeedbackSubtext", "We haven't thought of everything, so please help us improve this app. We really value your ideas and feedback!");
                StringProperties.Add("RateSubtext", "Enjoying the app? Please take a moment to leave a rating. Workout Tracker will never nag.");
                StringProperties.Add("FacebookLink", "http://www.facebook.com");
                StringProperties.Add("FacebookSubtext", "Visit Us on Facebook");

                RaisePropertyChanged(() => StringProperties);
            }
        }

        private void SetupMeasurementOptions()
        {
            if (SystemOfMeasurementOptions != null && SystemOfMeasurementOptions.Any())
                return;

            MeasurementSystem selectedSystem = (MeasurementSystem)Preferences.Get(AppConstants.App.MEASUREMEMT_SYSTEM, (int)MeasurementSystem.Imperial);
            SystemOfMeasurementOptions = new List<ASRadioButtonOption>
            {
                new ASRadioButtonOption
                {
                    Title = MeasurementSystem.Imperial.ToString(),
                    ID = (int)MeasurementSystem.Imperial,
                    IsChecked = selectedSystem == MeasurementSystem.Imperial
                },
                new ASRadioButtonOption
                {
                    Title = MeasurementSystem.Metric.ToString(),
                    ID = (int)MeasurementSystem.Metric,
                    IsChecked = selectedSystem == MeasurementSystem.Metric
                }
            };
            RaisePropertyChanged(() => SystemOfMeasurementOptions);
        }


        #region Command Methods

        private async Task OnAddNewType(object table)
        {
            var result = await _navigationService.NavigateAsync($"AddNew{table}", new NavigationParameters { { "add", true } });
            return;
        }

        private async Task OnRestoreDatabase()
        {
            bool confirm = await _dialogService.ConfirmAsync("Confirm Reset", "WARNING! This will delete all custom data and restore the database back to the latest factory setting. Are you sure?");
            if (confirm)
            {
                SetLoadingState(true, "Restoring...");
                var clear = await _datastoreService.ClearData(DatabaseDataType.NonProfile);
                if (!clear)
                    return;

                var result = await _datastoreService.RestoreDatabase();
                SetLoadingState(false, null);

                if (!result)
                {
                    _dialogService.ShowMessage("Oops.", "Something went wrong. It's not you, it's us.");
                }
                else
                {
                    SetLoadingState(true, "Updating...");
                    var refresh = await _datastoreService.RefreshDatabase();
                    SetLoadingState(false, null);
                    if (refresh)
                    {
                        await SetupStats();
                        StringProperties["DatabaseVersion"] = (await _datastoreService.GetDatabaseVersion()).ToString();
                    }
                }
            }
        }

        private async Task OnRefreshDatabase()
        {
            SetLoadingState(true, "Checking...");
            var update = await _datastoreService.CheckForRemoteDatabaseUpdates();
            SetLoadingState(false, null);

            if (update)
            {
                SetLoadingState(true, "Updating...");
                var clear = await _datastoreService.ClearData(DatabaseDataType.User);
                if (!clear)
                    return;

                var refresh = await _datastoreService.RefreshDatabase();
                SetLoadingState(false, null);

                if (!refresh)
                {
                    _dialogService.ShowMessage("Oops.", "Something went wrong. It's not you, it's us.");
                }
                else
                {
                    await SetupStats();
                    StringProperties["DatabaseVersion"] = (await _datastoreService.GetDatabaseVersion()).ToString();
                }
            }
        }

        private void OnMeasurementOptionChanged(ASRadioButtonOption option)
        {
            switch (option.ID)
            {
                case (int)MeasurementSystem.Imperial:
                    Preferences.Set(AppConstants.App.WEIGHT_SYSTEM, "lb");
                    Preferences.Set(AppConstants.App.DISTANCE_SYSTEM, "mi");
                    Preferences.Set(AppConstants.App.ELEVATION_SYSTEM, "ft");
                    Preferences.Set(AppConstants.App.MEASUREMEMT_SYSTEM, (int)MeasurementSystem.Imperial);
                    break;
                case (int)MeasurementSystem.Metric:
                    Preferences.Set(AppConstants.App.WEIGHT_SYSTEM, "kg");
                    Preferences.Set(AppConstants.App.DISTANCE_SYSTEM, "km");
                    Preferences.Set(AppConstants.App.ELEVATION_SYSTEM, "m");
                    Preferences.Set(AppConstants.App.MEASUREMEMT_SYSTEM, (int)MeasurementSystem.Metric);
                    break;
                default:
                    break;
            }
        }

        private async Task OnFeedback()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{AppInfo.Name} {DeviceInfo.Platform} Dev Team:");
            builder.AppendLine().AppendLine().AppendLine().AppendLine().AppendLine().AppendLine();
            builder.AppendLine("--------------------------");
            builder.AppendLine($"App");
            builder.AppendLine($"Package: {AppInfo.PackageName}");
            builder.AppendLine($"Version: {AppInfo.Version}");
            builder.AppendLine($"Version Long: {AppInfo.VersionString}");
            builder.AppendLine($"Device");
            builder.AppendLine($"Platform: {DeviceInfo.Platform}");
            builder.AppendLine($"Type: {DeviceInfo.DeviceType}");
            builder.AppendLine($"Name: {DeviceInfo.Name}");
            builder.AppendLine($"Model: {DeviceInfo.Model}");
            builder.AppendLine($"Version: {DeviceInfo.Version}");
            builder.AppendLine($"Version Long: {DeviceInfo.VersionString}");
            builder.AppendLine($"Idiom: {DeviceInfo.Idiom}");
            builder.AppendLine($"Manufacturer: {DeviceInfo.Manufacturer}");

            try
            {
                var message = new EmailMessage
                {
                    Subject = $"{AppInfo.Name} {DeviceInfo.Platform} Feedback",
                    Body = builder.ToString(),
                    To = new List<string> { "feedback@appsmithsllc.com" },
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }

        private void OnRate()
        {
            string appID = "";
            if (DeviceInfo.Platform == DevicePlatform.Android)
                appID = _jsonConsumerService.GetSettingValue(AppConstants.App.APPID_DROID);
            else
                appID = _jsonConsumerService.GetSettingValue(AppConstants.App.APPID_IOS);

            CrossStoreReview.Current.OpenStoreReviewPage(appID);
            Preferences.Set(AppConstants.App.DID_RATE_APP, true);
        }

        #endregion

        #region Properties

        //private bool _showUserName;
        //public bool ShowUserName
        //{
        //    get { return _showUserName; }
        //    set { _showUserName = value; RaisePropertyChanged(() => ShowUserName); }
        //}

        private bool _showNewExercise;
        public bool ShowNewExercise
        {
            get { return _showNewExercise; }
            set { _showNewExercise = value; RaisePropertyChanged(() => ShowNewExercise); }
        }

        private bool _showNewRoutineType;
        public bool ShowNewRoutineType
        {
            get { return _showNewRoutineType; }
            set { _showNewRoutineType = value; RaisePropertyChanged(() => ShowNewRoutineType); }
        }

        private bool _showNewLevel;
        public bool ShowNewLevel
        {
            get { return _showNewLevel; }
            set { _showNewLevel = value; RaisePropertyChanged(() => ShowNewLevel); }
        }

        private bool _showRating;
        public bool ShowRating
        {
            get { return _showRating; }
            set { _showRating = value; RaisePropertyChanged(() => ShowRating); }
        }

        #endregion
    }
}
