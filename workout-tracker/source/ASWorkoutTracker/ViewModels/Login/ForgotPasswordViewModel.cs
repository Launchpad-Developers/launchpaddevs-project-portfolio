using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AS.Forms.Controls.Extensions;
using AS.Forms.Controls.ViewModels;
using ASCommonServices.Interfaces;
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
    public class ForgotPasswordViewModel : BaseViewModel
    {
        protected IASWTDatastoreService _datastoreService;
        protected IFirebaseAuthentication _firebaseAuthentication;

        public ForgotPasswordViewModel(INavigationService navigationService,
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
            SignUpCommand = new DelegateCommand(async () => await OnSignUp());
            SendCommand = new DelegateCommand(async () => await OnSend());

            ShowSendButton = true;
        }

        public ICommand SendCommand { get; }
        public ICommand SignUpCommand { get; }

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

        private bool _showSendButton;
        public bool ShowSendButton
        {
            get { return _showSendButton; }
            set { _showSendButton = value; RaisePropertyChanged(() => ShowSendButton); }
        }

        private async Task OnSend()
        {
            ShowSendButton = false;

            if (IsInvalidEmail)
            {
                ReportForgotPasswordIssue("Invalid Email");
                return;
            }
            else if (!_session.InternetConnected)
            {
                ReportForgotPasswordIssue("Device Offline");
                return;
            }

            SetLoadingState(true, "Submitting Email");
            await _firebaseAuthentication.ForgotPassword(Email);
            SetLoadingState(false, string.Empty);

            ShowSendButton = true;

            _dialogService.ShowMessage("Email Sent", $"Instructions on how to reset your password have been sent to ${Email}.");
        }

        private void ReportForgotPasswordIssue(string message)
        {
            ShowSendButton = true;
            _dialogService.ShowMessage("Error", message);
#if DEBUG
            Analytics.TrackEvent("Forgot Password Issue", new Dictionary<string, string> { { "Issue", message} });
#endif
        }

        private async Task OnSignUp()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { AppConstants.ParameterKeys.SignUp, "true" }
            };

            await _navigationService.GoBackAsync(parameters);
        }

    }
}