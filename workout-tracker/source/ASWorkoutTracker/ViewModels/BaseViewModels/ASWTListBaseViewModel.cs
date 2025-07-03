using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AS.Forms.Controls.Extensions;
using AS.Forms.Controls.ViewModels;
using ASCommonServices;
using ASCommonServices.Events;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ASWorkoutTracker.ViewModels
{
    public class ASWTListBaseViewModel<T> : BaseViewModel where T : new()
    {
        protected IASWTDatastoreService _datastoreService;

        public ASWTListBaseViewModel(IEventAggregator _eventAggregator,
                                     IDialogService dialogService)
        {
            EventAggregator = _eventAggregator;
            _dialogService = dialogService;

            ListCollection = new ObservableCollection<T>();

            Delete = new DelegateCommand<T>(async (item) => { await OnDeleteRecord(item); });
            Search = new DelegateCommand(async () => { await DelayedQueryForKeyboardTypingSearches().ConfigureAwait(false); });
        }

        public ICommand Delete { get; }
        public ICommand Search { get; }
        public ICommand AddEdit { get; protected set; }
        public ICommand Open { get; protected set; }
        public ICommand ModelUpdated { get; protected set; }
        public ICommand ShowHideDetails { get; protected set; }
        public ICommand Filter { get; protected set; }
        public ICommand SelectedFilterChanged { get; protected set; }

        #region Fields

        public int CollectionCount { get { return ListCollection.Count; } }
        public ObservableCollection<T> ListCollection { get; protected set; }

        private HtmlWebViewSource _details;
        public HtmlWebViewSource Details
        {
            get { return _details; }
            set { _details = value; RaisePropertyChanged(() => Details); }
        }

        private bool _isShowingDetails;
        public bool IsShowingDetails
        {
            get { return _isShowingDetails; }
            set { _isShowingDetails = value; RaisePropertyChanged(() => IsShowingDetails); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(() => SearchText); }
        }

        public ObservableCollection<Filter> Filters { get; set; }
        private Filter _selectedFilter;
        public Filter SelectedFilter
        {
            get { return _selectedFilter; }
            set { _selectedFilter = value; RaisePropertyChanged(() => SelectedFilter); }
        }

        private int _selectedFilterIndex;
        public int SelectedFilterIndex
        {
            get { return _selectedFilterIndex; }
            set { _selectedFilterIndex = value >= 0 ? value : 0; RaisePropertyChanged(() => SelectedFilterIndex); }
        }
        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            EventAggregator.GetEvent<DatastoreUpdatedEvent>().Subscribe(OnDataUpdatedEvent);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            EventAggregator.GetEvent<DatastoreUpdatedEvent>().Unsubscribe(OnDataUpdatedEvent);
        }

        protected async void OnDataUpdatedEvent()
        {
            await ReloadCollection();
        }

        protected virtual async Task OnDeleteRecord(T record)
        {
            var confirm = await _dialogService.ConfirmAsync("Confirm Delete", $"This will delete {((IBaseSQLModel)record).Name}. Are you sure?");
            if (confirm)
            {
                SetLoadingState(true, $"Deleting {((IBaseSQLModel)record).Name}");
                var result = await _datastoreService.Delete(record);
                SetLoadingState(false, string.Empty);

                if (!result)
                {
                    _dialogService.ShowMessage("Oops.", "Something went wrong. It's not you, it's us.");
                }

                await ReloadCollection();
            }
        }

        protected CancellationTokenSource throttleCts = new CancellationTokenSource();
        private async Task DelayedQueryForKeyboardTypingSearches()
        {
            try
            {
                //TODO Add delay value to settings page
                var delay = Preferences.Get(Constants.Misc.TypeSearchDelay, Constants.Misc.DEFAULT_SEARCH_TYPE_DELAY);
                Interlocked.Exchange(ref throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(delay), throttleCts.Token)
                            .ContinueWith(task => OnSearchChanged(),
                            CancellationToken.None,
                            TaskContinuationOptions.OnlyOnRanToCompletion,
                            TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                //Ignore any Threading errors
            }
        }

        private async void OnSearchChanged()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                SetLoadingState(true, "Loading");
                await PopulateCollection();
                SetLoadingState(false, string.Empty);
                return;
            }

            SetLoadingState(true, string.Empty);

            string[] searchWords = SearchText.Split(new Char[] { ' ', ',', '.', ':', '\t' });

            List<BaseSQLModel> filtered = new List<BaseSQLModel>();

            foreach (var word in searchWords)
            {
                var wordResults = ListCollection.Where(x => ((BaseSQLModel)(object)x).Name.ToUpper().Contains(word.ToUpper()));
                foreach (var item in wordResults)
                    filtered.Add((BaseSQLModel)(object)item);
            }

            ListCollection.Clear();
            foreach (var item in filtered)
                ListCollection.Add((T)(object)item);

            SetLoadingState(false, string.Empty);
        }

        protected virtual Task<IEnumerable<T>> PopulateCollection()
        {
            throw new NotImplementedException("PopulateCollection must be implemented by inheriting class.");
        }

        public virtual Task ReloadCollection()
        {
            throw new NotImplementedException("ReloadCollection must be implemented by inheriting class.");
        }

        protected virtual Task UpdateFilters()
        {
            throw new NotImplementedException("UpdateFilters must be implemented by inheriting class.");
        }

        protected virtual Task OnSelectedFilterChanged(Filter filter)
        {
            throw new NotImplementedException("Must be implemented from inherited class.");
        }
    }
}
