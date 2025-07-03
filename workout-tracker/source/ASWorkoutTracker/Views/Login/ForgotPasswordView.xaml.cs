using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using AS.Forms.Controls.BaseControls;

namespace ASWorkoutTracker.Views.Login
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordView : ASContentPage
    {
        public ForgotPasswordView()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
        }
    }
}