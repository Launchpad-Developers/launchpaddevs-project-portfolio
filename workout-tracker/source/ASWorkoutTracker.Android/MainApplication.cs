using System;
using Android.App;
using Android.Runtime;
using Plugin.CurrentActivity;

namespace ASWorkoutTracker.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
        : base(handle, transer)
        {
            return;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);
        }
    }
}
