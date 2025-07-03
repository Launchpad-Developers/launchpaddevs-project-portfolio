using System.ComponentModel;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace ASWorkoutTracker.Views.Navigation
{
    public partial class ASTabbedView : Xamarin.Forms.TabbedPage, INavigatedAware
    {
        public ASTabbedView()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            UnselectedTabColor = (Color)Xamarin.Forms.Application.Current.Resources["AndroidUnfocusedMenuTextColor"];
            SelectedTabColor = (Color)Xamarin.Forms.Application.Current.Resources["MenuTextColor"];
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            // Not used
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            // You need to manually raise OnNavigatedTo in child pages on 1st time navigation
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                // Prism always raises OnNavigatedTo on 1st tabbed page so this prevents the first tab being initialised twice
                if (Children.Count == 1)                
                    return;
                
                for (var pageIndex = 1; pageIndex < Children.Count; pageIndex++)
                {
                    var page = Children[pageIndex];
                    (page?.BindingContext as INavigatedAware)?.OnNavigatedTo(parameters);
                }
            }
        }
    }
}
