using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AS.Forms.Controls.Extensions;
using AS.Forms.Controls.ViewModels;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore;
using Microsoft.AppCenter.Analytics;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Forms.Internals;

namespace ASWorkoutTracker.ViewModels.Login
{
    [Preserve(AllMembers = true)]
    public class SignUpViewModel : BaseViewModel
    {
        protected IASWTDatastoreService _datastoreService;
        protected IFirebaseAuthentication _firebaseAuthentication;

        public SignUpViewModel(INavigationService navigationService,
                               IDialogService dialogService,
                               IEventAggregator eventAggregator,
                               IASWTDatastoreService datastoreService,
                               IAppVersionService appVersionService,
                               INotificationService notificationService,
                               ILoggerService loggerService,
                               IFirebaseAuthentication firebaseAuthentication,
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

            LoginCommand = new DelegateCommand(async () => await OnLogin());
            SignUpCommand = new DelegateCommand(async () => await OnSignUp());
            ShowSignUpButton = true;

            //#if DEBUG
            //            Email = "william@appsmithsllc.com";
            //            Password = "D0rothy3";
            //            ConfirmPassword = "D0rothy3";
            //#endif
        }

        public ICommand LoginCommand { get; set; }
        public ICommand SignUpCommand { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }

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

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; RaisePropertyChanged(() => ConfirmPassword); }
        }

        private bool _showSignUpButton;
        public bool ShowSignUpButton
        {
            get { return _showSignUpButton; }
            set { _showSignUpButton = value; RaisePropertyChanged(() => ShowSignUpButton); }
        }

        private async Task OnLogin()
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnSignUp()
        {
            ShowSignUpButton = false;

            if (IsInvalidEmail)
            {
                ReportSignUpIssue("Invalid Email");
                return;
            }
            else if (!_session.InternetConnected)
            {
                ReportSignUpIssue("Device Offline");
                return;
            }
            else if (string.IsNullOrEmpty(Email))
            {
                ReportSignUpIssue("Email is required");
                return;
            }
            else if (string.IsNullOrEmpty(Password))
            {
                ReportSignUpIssue("Password is required");
                return;
            }
            else if (string.IsNullOrEmpty(ConfirmPassword))
            {
                ReportSignUpIssue("Confirm Password is required");
                return;
            }
            else if (!Password.Equals(ConfirmPassword))
            {
                ReportSignUpIssue("Passwords don't match.");
                return;
            }

            SetLoadingState(true, "Logging In");
            LoginResult result = await _firebaseAuthentication.AddUser(Name, Email, Password);
            SetLoadingState(false, string.Empty);

            ShowSignUpButton = true;

            if (result.Success)
            {
                NavigationParameters parameters = new NavigationParameters
                {
                    { AppConstants.ParameterKeys.LoginComplete, "true" },
                    { AppConstants.ParameterKeys.LoginResult, result }
                };

                await _navigationService.GoBackAsync(parameters);
            }
            else
            {
                _dialogService.ShowMessage("SignUp Failed", result.Message);
                Analytics.TrackEvent("SignUp Failed", new Dictionary<string, string> {
                    { "Info", "SignUp Failed." },
                    { "result.Message", result.Message }
                });
            }
        }

        private void ReportSignUpIssue(string message)
        {
            ShowSignUpButton = true;

            _dialogService.ShowMessage("Error", message);
#if DEBUG
            Analytics.TrackEvent("SignUp Issue", new Dictionary<string, string> { { "Issue", message} });
#endif
        }
    }
}