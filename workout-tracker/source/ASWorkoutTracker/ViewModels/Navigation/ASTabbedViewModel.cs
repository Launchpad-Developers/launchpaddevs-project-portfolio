using System;
using AS.Forms.Controls.ViewModels;
using ASWorkoutTracker.Datastore.Models;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;

namespace ASWorkoutTracker.ViewModels.Navigation
{
    public class ASTabbedViewModel : BaseViewModel, INavigatedAware
    {
        public ASTabbedViewModel(INavigationService navigationService,
                                 IPageDialogService pageDialogService,
                                 IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            EventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(AppConstants.ParameterKeys.RoutineExercise))
            {
                var model = parameters.GetValue<RoutineExercise>(AppConstants.ParameterKeys.RoutineExercise);
                //Title = model.Exercise.Name;
            }
            else if (parameters.ContainsKey(AppConstants.ParameterKeys.Exercise))
            {
                throw new Exception("TabbedViewModel does not accept Exercise as a parameter, only RoutineExercise(s).");
            }
        }
    }
}