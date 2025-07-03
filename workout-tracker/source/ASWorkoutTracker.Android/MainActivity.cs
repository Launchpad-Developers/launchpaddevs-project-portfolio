using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Prism.Ioc;
using Android.Content;
using Xamarin.Forms;
using Android.Util;
using System.Linq;
using ASCommonServices.Interfaces;
using ASCommonServices;
//using AS.CommonServices.Droid.Services;
using ASWorkoutTracker.Repository;
using ASWorkoutTracker.Datastore;
using System.Reflection;
using ASCommonServices.Services;
using AS.Forms.Controls.BaseControls;
//using ASCommonServices.Droid.Services.Push;
using AS.Forms.Controls.Views;
using ASWorkoutTracker.Views.Navigation;
using Firebase;
using ASWorkoutTracker.Droid.Auth;
using Plugin.CurrentActivity;
using Prism;
using ASWorkoutTracker.Android.Services;

namespace ASWorkoutTracker.Droid
{
    [Activity(Label = "@string/ApplicationName",
              Icon = "@drawable/icon",
              Theme = "@style/MainTheme",
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string TAG = "MainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, "ca-app-pub-9921792637906818~9227025004");
            Forms.Init(this, savedInstanceState);

            //WS 102819
            //If you receive the error "Make sure to call FirebaseApp.initializeApp(Context) first."
            //More than likely you need to clean and rebuild the Android project.
            //Also, check that the google-services.json file Build Action is set to GoogleServicesJson  
            //The following line should be called automatically, and you should never need to call it manually
            FirebaseApp.InitializeApp(Android.App.Application.Context);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key != null)
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    }
                }
            }

            CreateNotificationChannel();

            LoadApplication(new App(new AndroidInitializer(this)));
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = Resources.GetString(Resource.String.channel_name);
            var description = GetString(Resource.String.channel_description);
            var channel = new NotificationChannel(Constants.Misc.CHANNEL_ID, name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Xamarin.Essentials.Platform.OnResume();
        }

        #region BackButtonHack

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            ASContentPage.AndroidAction = () =>
            {
                AndroidX.AppCompat.Widget.Toolbar toolbar = this.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
            };

            base.OnPostCreate(savedInstanceState);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //if we are not hitting the internal "home" button, just return without any action
            if (item.ItemId != Android.Resource.Id.Home)
                return base.OnOptionsItemSelected(item);

            //this one triggers the hardware back button press handler - so we are back in XF without even mentioning it
            this.OnBackPressed();

            // return true to signal we have handled everything fine
            return true;
        }

        public override void OnBackPressed()
        {
            var mainPage = Xamarin.Forms.Application.Current.MainPage;
            if (!(mainPage is ASMasterDetailPage))
            {
                //We're not fully logged into the app yet, let the back button operate as normal
                base.OnBackPressed();
            }
            else
            {
                if (((ASNavigationView)((ASMasterDetailPage)mainPage).Detail).Pages.Count() <= 1)
                {
                    //We're at the root page; open/close the Menu
                    ((ASMasterDetailPage)mainPage).IsPresented = !((ASMasterDetailPage)mainPage).IsPresented;
                    return;
                }
                else
                {
                    var currentpage = ((ASNavigationView)((ASMasterDetailPage)mainPage).Detail).Pages.LastOrDefault();

                    if (currentpage != null && currentpage is ASContentPage && ((ASContentPage)currentpage).BackCommand != null)
                    {
                        ((ASContentPage)currentpage).BackCommand.Execute(null);
                    }
                    else
                    {
                        base.OnBackPressed();
                    }
                }
            }
        }

        #endregion

        //public override void OnBackPressed()
        //{
        //    var page = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
        //    if (page is ASContentPage)
        //    {
        //        var currentpage = (ASContentPage)page;
        //        if (currentpage?.BackCommand != null)
        //            currentpage?.BackCommand.Execute(null);
        //        else
        //            base.OnBackPressed();
        //    }
        //    else
        //    {
        //        base.OnBackPressed();
        //    }
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    if (item.ItemId == 16908332)
        //    {
        //        var page = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
        //        if (page is ASContentPage)
        //        {
        //            var currentpage = (ASContentPage)page;
        //            if (currentpage?.BackCommand != null)
        //            {
        //                currentpage?.BackCommand.Execute(null);
        //                return false;
        //            }
        //        }

        //        return base.OnOptionsItemSelected(item);
        //    }
        //    else
        //    {
        //        return base.OnOptionsItemSelected(item);
        //    }
        //}

        private class AndroidInitializer : IPlatformInitializer
        {
            private readonly Context _context;
            public AndroidInitializer(Context context)
            {
                _context = context;
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                containerRegistry.RegisterInstance(_context);
                containerRegistry.RegisterInstance<IDeviceStorageService>(new DeviceStorageService_Droid());
                containerRegistry.RegisterInstance<IAudioService>(new AudioService_Droid());
                containerRegistry.RegisterInstance<IBadgeService>(new BadgeService_Droid());
                containerRegistry.RegisterInstance<INotificationService>(new MyFirebaseIIDService());
                containerRegistry.RegisterInstance<ILocalNotificationService>(new MyFirebaseMessagingService());
                //containerRegistry.RegisterInstance<IUpdateService>(new UpdateService_Droid());

                containerRegistry.RegisterInstance<IAppVersionService>(new AppVersionService_Droid(_context));

                Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                containerRegistry.RegisterInstance<IJsonConsumerService>(new JsonConsumerService("ASWorkoutTracker", assembly));

                containerRegistry.RegisterSingleton<IRESTRepositoryService, RESTRepositoryService>();
                containerRegistry.RegisterSingleton<IFirebaseRepositoryService, FirebaseRepositoryService>();
                containerRegistry.RegisterSingleton<IASWTRepositoryService, ASWTRepositoryService>();
                containerRegistry.RegisterSingleton<IASWTDatastoreService, ASWTDatastoreService>();
                containerRegistry.RegisterSingleton<ISQLiteDatastoreService, SQLIteDatastoreService>();
                containerRegistry.RegisterSingleton<IFirebaseAuthentication, FirebaseAuthentication>();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}