using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using AS.Forms.Controls.BaseControls;

namespace ASWorkoutTracker.Views.Login
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpView : ASContentPage
    {
        public SignUpView()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
        }
    }
}