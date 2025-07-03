using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AS.Forms.Controls.ViewModels;
using ASWorkoutTracker.Datastore.Models.AppModels;
using Prism.Commands;
using Syncfusion.SfRotator.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ASWorkoutTracker.ViewModels.Support
{
    [Preserve(AllMembers = true)]
    public class OnBoardingAnimationViewModel : BaseViewModel
    {

        public OnBoardingAnimationViewModel()
        {
            SkipCommand = new DelegateCommand(OnSkip);
            NextCommand = new DelegateCommand<object>(OnNext);
            Boardings = new ObservableCollection<Boarding>
            {
                new Boarding()
                {
                    ImagePath = "ReSchedule.png",
                    Header = "RESCHEDULE",
                    Content = "Drag and drop meetings in order to reschedule them easily.",
                    //RotatorItem = new WalkthroughItemPage()
                },
                new Boarding()
                {
                    ImagePath = "ViewMode.png",
                    Header = "VIEW MODE",
                    Content = "Display your meetings using four configurable view modes",
                    //RotatorItem = new WalkthroughItemPage()
                },
                new Boarding()
                {
                    ImagePath = "TimeZone.png",
                    Header = "TIME ZONE",
                    Content = "Display meetings created for different time zones.",
                    //RotatorItem = new WalkthroughItemPage()
                }
            };

            // Set bindingcontext to content view.
            foreach (var boarding in this.Boardings)
            {
                boarding.RotatorItem.BindingContext = boarding;
            }
        }

        public ICommand SkipCommand { get; set; }
        public ICommand NextCommand { get; set; }

        private ObservableCollection<Boarding> _boardings;
        public ObservableCollection<Boarding> Boardings
        {
            get { return _boardings; }
            set { _boardings = value; RaisePropertyChanged(() => Boardings); }
        }

        private string _nextButtonText;
        public string NextButtonText
        {
            get { return _nextButtonText; }
            set { _nextButtonText = value; RaisePropertyChanged(() => NextButtonText); }
        }

        private bool _isSkipButtonVisible;
        public bool IsSkipButtonVisible
        {
            get { return _isSkipButtonVisible; }
            set { _isSkipButtonVisible = value; RaisePropertyChanged(() => IsSkipButtonVisible); }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; RaisePropertyChanged(() => SelectedIndex); }
        }


        private bool ValidateAndUpdateSelectedIndex(int itemCount)
        {
            if (this.SelectedIndex >= itemCount - 1)
            {
                return true;
            }

            this.SelectedIndex++;
            return false;
        }

        private void OnSkip()
        {
            this.MoveToNextPage();
        }

        private void OnNext(object obj)
        {
            var itemCount = (obj as SfRotator).ItemsSource.Count();
            if (this.ValidateAndUpdateSelectedIndex(itemCount))
            {
                this.MoveToNextPage();
            }
        }

        private void MoveToNextPage()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }

    }
}
