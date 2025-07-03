using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Events;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using ASCommonServices.Interfaces;
using AS.Forms.Controls.ViewModels;
using ASWorkoutTracker.Datastore;
using ASCommonServices.Events;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;
using System;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using ASWorkoutTracker.Datastore.Models;
using Xamarin.Essentials;
using System.Timers;
using Plugin.StoreReview;
using ASCommonServices.Models;

namespace ASWorkoutTracker.ViewModels.Navigation
{
    [Preserve(AllMembers = true)]
    public class ASMasterDetailViewModel : BaseViewModel, INavigationAware
    {
        protected IASWTDatastoreService _datastoreService;
        protected IFirebaseAuthentication _firebaseAuthentication;

        public ASMasterDetailViewModel(INavigationService navigationService,
                                       IDialogService dialogService,
                                       IEventAggregator eventAggregator,
                                       IAppVersionService appVersionService,
                                       IASWTDatastoreService datastoreService,
                                       ISession session,
                                       IFirebaseAuthentication firebaseAuthentication,
                                       INotificationService notificationService) : base()
        {
            _appVersionService = appVersionService;
            _notificationService = notificationService;
            EventAggregator = eventAggregator;
            _datastoreService = datastoreService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _session = session;
            _firebaseAuthentication = firebaseAuthentication;

            Logout = new DelegateCommand(OnLogout);
            DetailNav = new DelegateCommand<string>(async (route) => await OnDetailNav(route));
            TakePhoto = new DelegateCommand(OnTakePhoto);
            
#if DEBUG
            ShowDebugFunctions = true;
#endif
        }

        public ICommand DetailNav { get; }
        public ICommand Logout { get; }
        public ICommand TakePhoto { get; }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; RaisePropertyChanged(() => Username); }
        }

        private int _goldMedals;
        public int GoldMedals
        {
            get { return _goldMedals; }
            set { _goldMedals = value; RaisePropertyChanged(() => GoldMedals); }
        }

        private int _silverMedals;
        public int SilverMedals
        {
            get { return _silverMedals; }
            set { _silverMedals = value; RaisePropertyChanged(() => SilverMedals); }
        }

        private int _bronzeMedals;
        public int BronzeMedals
        {
            get { return _bronzeMedals; }
            set { _bronzeMedals = value; RaisePropertyChanged(() => BronzeMedals); }
        }

        private string _appVersion;
        public string AppVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; RaisePropertyChanged(() => AppVersion); }
        }

        private bool _showDebugFunctions;
        public bool ShowDebugFunctions
        {
            get { return _showDebugFunctions; }
            set { _showDebugFunctions = value; RaisePropertyChanged(() => ShowDebugFunctions); }
        }

        private bool _showProfile;
        public bool ShowProfile
        {
            get { return _showProfile; }
            set { _showProfile = value; RaisePropertyChanged(() => ShowProfile); }
        }

        private async void OnLogout()
        {
            bool confirm = await _dialogService.ConfirmAsync("Confirm Logout", "Are you sure?");

            if (confirm)
            {
                SetLoadingState(true, "Logging Out");
                var result = _firebaseAuthentication.SignOut();
                SetLoadingState(false, null);

                if (!result)
                {
                    _dialogService.ShowMessage("Error", "Logout Failed");
                }
                else
                {
                    //await Task.Run(() => _notificationService.SendUnregistrationToServer());
                    //CrossToastPopUp.Current.ShowToastSuccess(TranslationManager.Translate("SUCCESSderegisterpushnotifications"), Plugin.Toast.Abstractions.ToastLength.Short);
                }

                EventAggregator.GetEvent<ErrorEvent>().Unsubscribe(OnErrorEvent);
                EventAggregator.GetEvent<InfoEvent>().Unsubscribe(OnInfoEvent);
                EventAggregator.GetEvent<LoadingStateChangedEvent>().Unsubscribe(OnLoadingStateChangedEvent);
                EventAggregator.GetEvent<SimpleMessageEvent>().Unsubscribe(OnSimpleMessageEvent);

                await _navigationService.NavigateAsync($"/{AppConstants.Navigation.LoginNav}/{AppConstants.Navigation.Login}");
            }
        }

        private async Task OnDetailNav(string route)
        {
            var parameters = new NavigationParameters { { AppConstants.ParameterKeys.Route, route } };
            var result = await _navigationService.NavigateAsync($"{AppConstants.Navigation.Nav}/{route}", parameters);
            return;
        }

        //private void SetBarBackgroundColorForLoginWorkflow()
        //{
        //    Application.Current.Resources["BarBackgroundColor"] = Application.Current.Resources["LoginNavBarBackgroundColor"];
        //    Application.Current.Resources["BarTextColor"] = Application.Current.Resources["LoginNavBarTextColor"];
        //}

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            EventAggregator.GetEvent<ErrorEvent>().Unsubscribe(OnErrorEvent);
            EventAggregator.GetEvent<InfoEvent>().Unsubscribe(OnInfoEvent);
            EventAggregator.GetEvent<LoadingStateChangedEvent>().Unsubscribe(OnLoadingStateChangedEvent);
            EventAggregator.GetEvent<SimpleMessageEvent>().Unsubscribe(OnSimpleMessageEvent);

            base.OnNavigatedFrom(parameters);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            EventAggregator.GetEvent<ErrorEvent>().Subscribe(OnErrorEvent);
            EventAggregator.GetEvent<InfoEvent>().Subscribe(OnInfoEvent);
            EventAggregator.GetEvent<LoadingStateChangedEvent>().Subscribe(OnLoadingStateChangedEvent);
            EventAggregator.GetEvent<SimpleMessageEvent>().Subscribe(OnSimpleMessageEvent);

            Application.Current.Resources["BarBackgroundColor"] = Application.Current.Resources["PrimaryDarker"];
            Application.Current.Resources["BarTextColor"] = Application.Current.Resources["GrayLightest"];
            AppVersion = $"Version {_appVersionService.VersionNumber}";
            
            var dbIsIntact = _datastoreService.DatabaseIsIntact();
            if (!dbIsIntact)
            {
                SetLoadingState(true, "Downloading Database...");
                await _datastoreService.RestoreDatabase();
                await Task.Delay(2000);
                await _datastoreService.RefreshDatabase();
                SetLoadingState(false, null);
            }
            else
            {
                var versionCheck = await _datastoreService.CheckForRemoteDatabaseUpdates();
                if (versionCheck)
                {
                    bool confirm = await _dialogService.ConfirmAsync("New Data Available", "There is a newer version of the database available.  Would you like to update now?");
                    if (!confirm)
                    {
                        _dialogService.ShowMessage("No Worries", "You can update at any time by opening the Settings and choosing 'Refresh Database'.");
                    }
                    else
                    {
                        SetLoadingState(true, "Updating...");
                        var refresh = await _datastoreService.RefreshDatabase();
                        SetLoadingState(false, null);

                        if (!refresh)
                            _dialogService.ShowMessage("Oops.", "Something went wrong. It's not you, it's us.");
                    }
                }
            }

            if (parameters.ContainsKey(AppConstants.ParameterKeys.LoginResult))
            {
                var loginResult = parameters.GetValue<LoginResult>(AppConstants.ParameterKeys.LoginResult);
                await _datastoreService.FinishLogin(loginResult);
            }

            if (_session.User != null)
            {
                Username = _session.User.UserName;
                GoldMedals = ((ASWTUser)_session.User).GoldCount;
                SilverMedals = ((ASWTUser)_session.User).SilverCount;
                BronzeMedals = ((ASWTUser)_session.User).BronzeCount;
                ShowProfile = true;
            }

#if DEBUG
            BronzeMedals = 11;
            SilverMedals = 4;
            GoldMedals = 1;
#endif

            base.OnNavigatedTo(parameters);
        }

        private async void OnTakePhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                _dialogService.ShowMessage("No Camera", "This device has no camera, or the camera is not available.");
                return;
            }

            //Take photo
            DateTime dateTime = DateTime.Now;
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "WorkoutTracker",
                Name = $"{dateTime.ToLongDateString()} {dateTime.ToLongTimeString()}",
                AllowCropping = true,
                DefaultCamera = CameraDevice.Front,
                SaveToAlbum = false,
                CustomPhotoSize = 10,
                CompressionQuality = 10
            });

            CreateAttachment(file);
        }

        private void CreateAttachment(MediaFile file)
        {
            if (file == null)
                return;

            DateTime dateTime = DateTime.Now;
            //Attachment attachment = null;

            //if (IsWorkRequest)
            //    attachment = new WorkRequestAttachment(WorkRequest.ID);
            //else
            //    attachment = new AssetAttachment(Asset.ID);

            //attachment.AttachmentName = $"{dateTime.ToLongDateString()} {dateTime.ToLongTimeString()}";
            //attachment.AttachmentType = "image/jpeg";
            //attachment.IsSynced = false;
            //attachment.IsDeleted = false;
            //attachment.LastChanged = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //attachment.UserName = _session.User.UserName;

            //attachment.ImagePath = file.Path;
            //attachment.VmID = currentIndex++;

            Stream imageStream = file.GetStream();
            BinaryReader br = new BinaryReader(imageStream);
            //attachment.AttachmentContent = br.ReadBytes((int)imageStream.Length);
            ////await Task.Run(() => attachment.SetImageSource());
            //attachment.SetImageSource();

            //Images.Add(attachment);
            //_attachments.Add(attachment);

            //UpdateCountMessage();
        }
    }
}
