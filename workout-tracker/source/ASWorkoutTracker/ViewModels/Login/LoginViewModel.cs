using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Events;
using Xamarin.Forms.Internals;
using AS.Forms.Controls.ViewModels;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;
using System.Threading.Tasks;
using System;
using ASCommonServices;
using ASCommonServices.Models;
using Xamarin.Essentials;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

namespace ASWorkoutTracker.ViewModels.Login
{
    [Preserve(AllMembers = true)]
    public class LoginViewModel : BaseViewModel, INavigationAware
    {
        protected IASWTDatastoreService _datastoreService;
        protected IFirebaseAuthentication _firebaseAuthentication;
        protected IFederatedAuthentication _federatedAuthentication;

        public LoginViewModel(INavigationService navigationService,
                              IDialogService dialogService,
                              IEventAggregator eventAggregator,
                              IASWTDatastoreService datastoreService,
                              IAppVersionService appVersionService,
                              INotificationService notificationService,
                              ILoggerService loggerService,
                              IFirebaseAuthentication firebaseAuthentication,
                              IFederatedAuthentication federatedAuthentication,
                              IJsonConsumerService jsonConsumerService,
                              ISession session) : base()
        {
            EventAggregator = eventAggregator;

            _appVersionService = appVersionService;
            _notificationService = notificationService;
            _datastoreService = datastoreService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _loggerService = loggerService;
            _session = session;
            _firebaseAuthentication = firebaseAuthentication;
            _federatedAuthentication = federatedAuthentication;
            _jsonConsumerService = jsonConsumerService;

            LoginCommand = new DelegateCommand(async () => await OnLogin());
            SignUpCommand = new DelegateCommand(async () => await OnSignUp());
            ForgotPasswordCommand = new DelegateCommand(async () => await OnForgotPassword());
            //FacebookLoginCommand = new DelegateCommand(async () => await OnFacebookLogin());
            //GoogleLoginCommand = new DelegateCommand(async () => await OnGoogleLogin());
            //AppleLoginCommand = new DelegateCommand(async () => await OnAppleLogin());

            AppVersion = $"Version {_appVersionService.VersionNumber}";

#if DEBUG
            //Email = "william@appsmithsllc.com";
            //Password = "123456";
#endif
        }

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        //public ICommand FacebookLoginCommand { get; }
        //public ICommand GoogleLoginCommand { get; }
        //public ICommand AppleLoginCommand { get; }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; RaisePropertyChanged(() => Email); }
        }

        private bool _isInvalidEmail;
        public bool IsInvalidEmail
        {
            get { return _isInvalidEmail; }
            set { _isInvalidEmail = value; RaisePropertyChanged(() => IsInvalidEmail); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(() => Password); }            
        }

        private string _appVersion;
        public string AppVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; RaisePropertyChanged(() => AppVersion); }
        }

        private bool _showLoginButton;
        public bool ShowLoginButton
        {
            get { return _showLoginButton; }
            set { _showLoginButton = value; RaisePropertyChanged(() => ShowLoginButton); }
        }

        private async Task OnLogin()
        {
            ShowLoginButton = false;

            if (IsInvalidEmail)
            {
                ReportLoginIssue("Invalid Email");
                return;
            }
            else if (!_session.InternetConnected)
            {
                ReportLoginIssue("Device Offline");
                return;
            }
            else if (string.IsNullOrEmpty(Email))
            {
                ReportLoginIssue("Email is required");
                return;
            }
            else if (string.IsNullOrEmpty(Password))
            {
                ReportLoginIssue("Password is required");
                return;
            }

            SetLoadingState(true, "Logging In");
            var result = await _firebaseAuthentication.LoginWithEmailAndPassword(_email, _password);
            SetLoadingState(false, string.Empty);

            if (result.Success)
            {
                await LoginComplete(result);
            }
            else
            {
                _dialogService.ShowMessage("Login Failed", result.Message);
                Analytics.TrackEvent("Login Failed", new Dictionary<string, string> {
                    { "Info", "Login Failed." },
                    { "result.Message", result.Message }
                });
            }

            ShowLoginButton = true;
        }

        private void ReportLoginIssue(string message)
        {
            ShowLoginButton = true;
            _dialogService.ShowMessage("Error", message);
#if DEBUG
            Analytics.TrackEvent("Login Issue", new Dictionary<string, string> { { "Issue", message} });
#endif
        }

        private async Task OnSignUp()
        {
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.SignUp);
            return;
        }

        private async Task OnForgotPassword()
        {
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.ForgotPassword);
            return;
        }

        //private async Task OnFacebookLogin()
        //{
        //    bool result = await _federatedAuthentication.FederatedAuthenticator(Constants.Misc.FACEBOOK, Constants.Misc.FACEBOOK_FED_URL,
        //                                                                        _jsonConsumerService.GetSettingValue(Constants.Misc.CALLBACK_URL));

        //    if (result)
        //        await LoginComplete(null);
        //}

        //private async Task OnGoogleLogin()
        //{
        //    bool result = await _federatedAuthentication.FederatedAuthenticator(Constants.Misc.GOOGLE, Constants.Misc.GOOGLE_FED_URL,
        //                                                                        _jsonConsumerService.GetSettingValue(Constants.Misc.CALLBACK_URL));

        //    if (result)
        //        await LoginComplete(null);
        //}

        //private async Task OnAppleLogin()
        //{
        //    bool result = await _federatedAuthentication.FederatedAuthenticator(Constants.Misc.APPLE, Constants.Misc.APPLE_FED_URL,
        //                                                                        _jsonConsumerService.GetSettingValue(Constants.Misc.CALLBACK_URL));

        //    if (result)
        //        await LoginComplete(null);
        //}

        private async Task LoginComplete(LoginResult loginResult)
        {
            INavigationParameters parameters = new NavigationParameters { { AppConstants.ParameterKeys.LoginResult, loginResult } };
            var result = await _navigationService.NavigateAsync($"/{AppConstants.Navigation.Master}/{AppConstants.Navigation.Nav}/{AppConstants.Navigation.Exercises}", parameters);
            return;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            ShowLoginButton = true;

            if (parameters.ContainsKey(AppConstants.ParameterKeys.LoginComplete) &&
                parameters.GetValue<bool>(AppConstants.ParameterKeys.LoginComplete))
            {
                await LoginComplete(parameters.GetValue<LoginResult>(AppConstants.ParameterKeys.LoginComplete));
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.SignUp) &&
                     parameters.GetValue<bool>(AppConstants.ParameterKeys.SignUp))
            {
                await OnSignUp();
            }            
        }
    }
}
