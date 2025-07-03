using Prism.Commands;
using Prism.Navigation;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Datastore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Events;
using Xamarin.Essentials;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Prism.Services.Dialogs;
using AS.Forms.Controls.Extensions;

namespace ASWorkoutTracker.ViewModels.Routines
{
    public class RoutinesViewModel : ASWTListBaseViewModel<Routine>, INavigationAware
    {
        public RoutinesViewModel(INavigationService navigationService,
                                 IASWTDatastoreService datastoreService,
                                 IDialogService dialogService,
                                 IEventAggregator eventAggregator)
            : base(eventAggregator, dialogService)
        {
            _datastoreService = datastoreService;
            _navigationService = navigationService;

            Filters = new ObservableCollection<Filter>();

            Title = "Routines";

            AddEdit = new DelegateCommand<Routine>(async (item) => await OnAddEdit(item));
            Open = new DelegateCommand<Routine>(async (item) => await OnOpen(item));
            ModelUpdated = new DelegateCommand<Routine>(async (routine) => await OnModelUpdated(routine));
            ShowHideDetails = new DelegateCommand<Routine>((routine) => OnShowHideDetails(routine));
            SelectedFilterChanged = new DelegateCommand<Filter>(async (filter) => await OnSelectedFilterChanged(filter));
            OpenUrl = new DelegateCommand<string>(async (url) => await OnOpenUrl(url));

            SetLoadingState(true, "Loading Routines");
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (!Filters.Any())
                await UpdateFilters();

            if (!parameters.ContainsKey(AppConstants.ParameterKeys.Reload) ||
                 parameters.GetValue<bool>(AppConstants.ParameterKeys.Reload))
                await ReloadCollection();

            SetLoadingState(false, string.Empty);
        }

        public async override Task ReloadCollection()
        {
            List<Routine> routines = (List<Routine>)await PopulateCollection();

            if (SelectedFilter == null || SelectedFilter.FilterID == 0)
                routines = routines.OrderByDescending(x => x.IsFavorite).ThenBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();
            else if (SelectedFilter.FilterID == 1)
                routines = routines.Where(x => x.IsFavorite).OrderBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();
            else
                routines = routines.Where(x => x.ProgramID == SelectedFilter.FilterID).OrderByDescending(x => x.IsFavorite).ThenBy(x => x.DateCreated).ThenBy(x => x.Name).ToList();

            ListCollection.Clear();

            foreach (var routine in routines)
            {
                if (routine.ID != 1000000)
                    ListCollection.Add(routine);
            }

            NoRecords = !ListCollection.Any();

            RaisePropertyChanged(() => CollectionCount);
        }

        protected override async Task<IEnumerable<Routine>> PopulateCollection()
        {
            return await _datastoreService.ReadAllRecordsOfType<Routine>();
        }

        protected override async Task UpdateFilters()
        {
            var filters = await _datastoreService.ReadRoutineFilters();
            foreach (var filter in filters)
                Filters.Add(filter);

            RaisePropertyChanged(() => Filters);

            Filter selectedFilter = JsonConvert.DeserializeObject<Filter>(Preferences.Get(AppConstants.App.ROUTINES_FILTER, string.Empty));
            if (selectedFilter != null)
            {
                SelectedFilter = selectedFilter;
                SelectedFilterIndex = Filters.IndexOf(Filters.Where(x => x.FilterID == selectedFilter.FilterID).FirstOrDefault());
            }            
        }

        #region Command Methods

        private async Task OnAddEdit(Routine item)
        {
            IsShowingDetails = false;
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.AddEditRoutine,
                                    new NavigationParameters {
                                        { AppConstants.ParameterKeys.Add, item == null },
                                        { AppConstants.ParameterKeys.Edit, item != null && !item.IsSystem },
                                        { AppConstants.ParameterKeys.Item, item } });
            return;
        }

        private async Task OnOpen(Routine routine)
        {
            routine.IsShowingDetails = false;
            IsShowingDetails = false;
            var result = await _navigationService.NavigateAsync(AppConstants.Navigation.RoutineExercises,
                                    new NavigationParameters { { AppConstants.ParameterKeys.Item, routine } });
            return;
        }

        private async Task OnModelUpdated(Routine routine)
        {
            var updated = await _datastoreService.Update(routine);
            if (updated)
            {
                var newList = ListCollection.ToList();

                //Replace object
                var oldIndex = ListCollection.IndexOf(routine);
                newList.RemoveAt(oldIndex);
                newList.Add(routine);

                newList = newList.OrderByDescending(x => x.IsFavorite).ThenByDescending(x => x.DateCreated).ThenBy(x => x.Name).ToList();
                var newIndex = newList.IndexOf(routine);

                ListCollection.Move(oldIndex, newIndex);

                if (DeviceInfo.Platform == DevicePlatform.Android)
                    OnPropertyChanged(nameof(ListCollection));
            }
        }

        private void OnShowHideDetails(Routine routine)
        {
            if (routine.IsShowingDetails)
            {
                Details = routine.WebViewNotes;
                IsShowingDetails = true;
            }
            else
            {
                IsShowingDetails = false;
                Details = new HtmlWebViewSource();
            }

            foreach (var r in ListCollection)
                r.IsShowingDetails = r.ID == routine.ID && routine.IsShowingDetails;

            MessagingCenter.Send(this, AppConstants.ParameterKeys.RoutineIsShowingDetailsChanged);
        }

        protected override async Task OnSelectedFilterChanged(Filter filter)
        {
            Preferences.Set(AppConstants.App.ROUTINES_FILTER, JsonConvert.SerializeObject(filter));
            await ReloadCollection();
            return;
        }

        protected override async Task OnDeleteRecord(Routine routine)
        {
            var confirm = await _dialogService.ConfirmAsync("Whoa!", $"This will delete {((IBaseSQLModel)routine).Name} and all of it's logs. Are you sure?");
            if (confirm)
            {
                SetLoadingState(true, $"Deleting {((IBaseSQLModel)routine).Name}");
                var result = await _datastoreService.DeleteRoutine(routine);
                SetLoadingState(false, string.Empty);

                if (!result)
                {
                    _dialogService.ShowMessage("Oops.", "Something went wrong. It's not you, it's us.");
                }

                await ReloadCollection();
            }
        }

        #endregion
    }
}
