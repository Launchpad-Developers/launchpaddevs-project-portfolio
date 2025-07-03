using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AS.Forms.Controls.Models;
using AS.Forms.Controls.Validation;
using AS.Forms.Controls.ViewModels;
using ASCommonServices;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASCommonServices.Utils;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Enums;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace ASWorkoutTracker.ViewModels
{
    public class ASWTModelBaseViewModel<T> : BaseViewModel where T : new()
    {
        protected IASWTDatastoreService _datastoreService;

        public ASWTModelBaseViewModel(IJsonConsumerService jsonConsumerService,
                                      IEventAggregator _eventAggregator,
                                      IPageDialogService pageDialogService,
                                      IDialogService dialogService)
        {
            _jsonConsumerService = jsonConsumerService;
            EventAggregator = _eventAggregator;
            _pageDialogService = pageDialogService;
            _dialogService = dialogService;

            Save = new DelegateCommand(async () => { await OnSave(); });
            ValidateName = new DelegateCommand(OnValidateName);
            BackCommand = new DelegateCommand(async () => { await OnBackCommand(); });
        }

        public ICommand Save { get; }
        public ICommand ValidateName { get; protected set; }

        #region Fields

        protected CancellationTokenSource throttleCts = new CancellationTokenSource();

        private T _editModel;
        public T EditModel
        {
            get { return _editModel; }
            set { _editModel = value; RaisePropertyChanged(() => EditModel); }
        }

        private T _model;
        public T Model
        {
            get { return _model; }
            set { _model = value; RaisePropertyChanged(() => Model); }
        }

        private bool _canEditName;
        public bool CanEditName
        {
            get { return _canEditName; }
            set { _canEditName = value; RaisePropertyChanged(() => CanEditName); }
        }

        protected ValidatableObject<string> _name;
        public ValidatableObject<string> Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }

        protected ValidatableObject<string> _briefDescription;
        public ValidatableObject<string> BriefDescription
        {
            get { return _briefDescription; }
            set { _briefDescription = value; RaisePropertyChanged(() => BriefDescription); }
        }

        protected ValidatableObject<string> _detailedDescription;
        public ValidatableObject<string> DetailedDescription
        {
            get { return _detailedDescription; }
            set { _detailedDescription = value; RaisePropertyChanged(() => DetailedDescription); }
        }
        #endregion

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey(AppConstants.ParameterKeys.Add) && parameters.GetValue<bool>(AppConstants.ParameterKeys.Add))
            {
                EditModel = new T();
                Model = new T();
                Title = $"Add {typeof(T).Name}";
                IsReadOnly = false;
                CanEditName = true;
                await PopulateLists();
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.Edit) && parameters.GetValue<bool>(AppConstants.ParameterKeys.Edit))
            {
                Model = parameters.GetValue<T>(AppConstants.ParameterKeys.Item);
                EditModel = GenericClone.Clone(Model);
                Title = $"Edit {typeof(T).Name}";
                IsReadOnly = false;
                IsEdit = true;
                await PopulateLists();
                EditModelSetup();
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.Item))
            {
                Model = parameters.GetValue<T>(AppConstants.ParameterKeys.Item);
                var model = Model as BaseSQLModel;
                Title = model.Name;
                IsReadOnly = true;
            }
        }

        protected virtual async Task OnBackCommand()
        {
            if (IsReadOnly)
            {
                await _navigationService.GoBackAsync();
                return;
            }
            else
            {
                if (Model != null)
                    SetModelValues();

                if (Model != null && !Model.Equals(EditModel))
                {
                    //Something changed; confirm what user wants to do
                    IActionSheetButton saveAction = ActionSheetButton.CreateButton("Save", new Action(async () => { await OnSave(); }));
                    IActionSheetButton continueAction = ActionSheetButton.CreateButton("Continue Without Saving", new Action(async () => { await _navigationService.GoBackAsync(new NavigationParameters { { AppConstants.ParameterKeys.Reload, false } }); }));
                    IActionSheetButton cancelAction = ActionSheetButton.CreateCancelButton("Cancel", new Action(() => { return; }));

                    await _pageDialogService.DisplayActionSheetAsync("Choose Action", saveAction, continueAction, cancelAction);
                }
                else
                {
                    //Nothing changed, just go back
                    NavigationParameters parameters = new NavigationParameters { { AppConstants.ParameterKeys.Reload, false } };
                    await _navigationService.GoBackAsync(parameters);
                }
            }
        }

        protected virtual async Task OnSave()
        {
            if (!await Validate())
                return;

            SetModelValues();

            var model = Model as BaseSQLModel;
            if (model.ID > 0)
                await _datastoreService.Update(Model);
            else
                await _datastoreService.Create(Model);

            NavigationParameters parameters = new NavigationParameters { { AppConstants.ParameterKeys.Reload, true } };
            await _navigationService.GoBackAsync(parameters);
        }
        
        private void OnValidateName()
        {
            if (!CanEditName)
                return;

            _name.Validate();
            var delay = Preferences.Get(Constants.Misc.TypeSearchDelay, Constants.Misc.DEFAULT_SEARCH_TYPE_DELAY);
            Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
            Task.Delay(TimeSpan.FromMilliseconds(delay), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                .ContinueWith(async
                   delegate { await _name.ValidateAsync(); },
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected virtual void OnPickerItemSelected(PickerDefinition definition)
        {
            if (definition == null) return;

            NavigationParameters parameters = new NavigationParameters
            {
                {AS.Forms.Controls.Constants.Constants.ParameterKeys.Property, definition.SelectedObjectProperty},
                {AS.Forms.Controls.Constants.Constants.ParameterKeys.Clear, false},
                {AS.Forms.Controls.Constants.Constants.ParameterKeys.SelectedItem, definition.SelectedItem}
            };
            ProcessSelectedItem(parameters);
        }

        protected void SetPropertyValue<U>(string settingsKey, PickerItemPropertyNames pickerItem, bool clear, U value = null) where U : BaseSQLModel
        {
            if (value == null)
            {
                value = JsonConvert.DeserializeObject<U>(Preferences.Get(settingsKey, string.Empty));
                if (value == null || clear)
                {
                    value = Activator.CreateInstance<U>();
                    value.ID = -1;
                    value.Name = "-- Select --";
                }
            }

            NavigationParameters parameters = new NavigationParameters
            {
                {AS.Forms.Controls.Constants.Constants.ParameterKeys.Property, pickerItem},
                {AS.Forms.Controls.Constants.Constants.ParameterKeys.Clear, clear},
                {AS.Forms.Controls.Constants.Constants.ParameterKeys.SelectedItem, value}
            };
            ProcessSelectedItem(parameters);
        }

        protected virtual void EditModelSetup()
        {
            throw new NotImplementedException("EditModelSetup must be implemented by inheriting class.");
        }

        protected virtual void ProcessSelectedItem(INavigationParameters parameters)
        {
            throw new NotImplementedException("ProcessSelectedItem must be implemented by inheriting class.");
        }

        protected virtual Task PopulateLists()
        {
            throw new NotImplementedException("PopulateLists must be implemented by inheriting class.");
        }

        protected virtual void SetModelValues()
        {
            throw new NotImplementedException("SetModelValues must be implemented by inheriting class.");
        }
    }
}
