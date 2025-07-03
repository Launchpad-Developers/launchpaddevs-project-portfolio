using Syncfusion.SfRotator.XForms.iOS;
using  Syncfusion.XForms.iOS.Graphics;
using Syncfusion.XForms.iOS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ASCommonServices;
using ASCommonServices.Interfaces;
using ASCommonServices.iOS.Services;
using ASCommonServices.Services;
using ASWorkoutTracker.Datastore;
using ASWorkoutTracker.Repository;
using Foundation;
using Google.MobileAds;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using Prism;
using Prism.Ioc;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using Syncfusion.SfPicker.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.XForms.iOS.Border;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.Pickers.iOS;
using ASWorkoutTracker.iOS.Auth;

namespace ASWorkoutTracker.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static AppDelegate Instance { get; private set; }
        public NSData DeviceToken { get; private set; }
        //public SBNotificationHub Hub { get; set; }

        public object NavigationController { get; private set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            SfRotatorRenderer.Init();
            Core.Init();

            SfButtonRenderer.Init();
            SfBorderRenderer.Init();
            SfGradientViewRenderer.Init();
            SfRadioButtonRenderer.Init();
            SfPickerRenderer.Init();
            SfChartRenderer.Init();
            SfTimePickerRenderer.Init();

            MobileAds.SharedInstance.Start(CompletionHandler);
            Firebase.Core.App.Configure();

            var localNotificationService = new LocalNotificationService_iOS();
            LoadApplication(new App(new IOSInitializer(localNotificationService)));

            //if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            //{
            //    UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
            //        (granted, error) =>
            //        {
            //            if (granted)
            //                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
            //        });

            //    UNUserNotificationCenter.Current.Delegate = localNotificationService;
            //}
            //else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //{
            //    UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());
            //    UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            //    UIApplication.SharedApplication.RegisterForRemoteNotifications();
            //}
            //else
            //{
            //    UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
            //    UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            //}

            Instance = this;

            return base.FinishedLaunching(app, options);
        }

        private void CompletionHandler(InitializationStatus status) { }

        //NOTE This method will not fire in simulator
        //public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        //{
        //    //We do not want to register for notifications yet
        //    //Instead we will store the deviceToken for later when the user successfully logs in
        //    //In an app where the users do not share devices we would probably register immediately.
        //    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        //    {
        //        byte[] bytes = deviceToken.ToArray<byte>();
        //        string[] hexArray = bytes.Select(b => b.ToString("x2")).ToArray();
        //        var token = string.Join(string.Empty, hexArray);
        //        DeviceToken = NSData.FromString(token);
        //    }
        //    else
        //    {
        //        DeviceToken = deviceToken;
        //    }

        //    DeviceToken = deviceToken;
        //    Preferences.Set(Constants.Misc.DevicePushTokenKey, JsonConvert.SerializeObject(DeviceToken));
        //}

        //[Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        //public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        //{
        //    Analytics.TrackEvent("iOS FailedToRegisterForRemoteNotifications", new Dictionary<string, string> {
        //        { "error", error.ToString()}
        //    });
        //}

        //public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        //{
        //    ProcessNotification(userInfo, false);
        //}

        //[Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        //public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    NSDictionary aps = new NSDictionary();
        //    NSString alert = null;
        //    try
        //    {
        //        aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
        //        if (aps.ContainsKey(new NSString("alert")))
        //            alert = aps.ObjectForKey(new NSString("alert")) as NSString;

        //        //show alert
        //        LocalNotificationService_iOS notificationService = new LocalNotificationService_iOS();
        //        notificationService.DisplayLocalNotification(alert);
        //    }
        //    catch (Exception ex)
        //    {
        //        Analytics.TrackEvent("iOS DidReceiveRemoteNotification Error", new Dictionary<string, string> {
        //            { "userInfo", userInfo.ToString()},
        //            { "containsAlert", aps.ContainsKey(new NSString("alert")).ToString()},
        //            { "alertIsNull", (alert == null).ToString()}
        //        });
        //    }
        //}

        [Export("applicationWillTerminate:")]
        public async override void WillTerminate(UIApplication application)
        {
            //Due to upcoming changes on the back end, this will no longer be necessary.
            //NotificationService_iOS notificationService = new NotificationService_iOS();
            //notificationService.SendUnregistrationToServer();

            //RepositoryService repositoryService = new RepositoryService(null, null, null, null);
            //await repositoryService.Logout(true);
        }

        //private void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        //{
        //    if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active ||
        //        UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background)
        //    {
        //        NSDictionary aps = new NSDictionary();
        //        NSString alert = null;
        //        try
        //        {
        //            aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
        //            if (aps.ContainsKey(new NSString("alert")))
        //                alert = aps.ObjectForKey(new NSString("alert")) as NSString;

        //            //show alert
        //            LocalNotificationService_iOS notificationService = new LocalNotificationService_iOS();
        //            notificationService.DisplayLocalNotification(alert);
        //        }
        //        catch (Exception ex)
        //        {
        //            Analytics.TrackEvent("iOS ProcessNotification Error", new Dictionary<string, string> {
        //                { "userInfo", options.ToString()},
        //                { "containsAlert", aps.ContainsKey(new NSString("alert")).ToString()},
        //                { "alertIsNull", (alert == null).ToString()}
        //            });
        //        }
        //    }
        //}

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Platform.OpenUrl(app, url, options))
                return true;

            return base.OpenUrl(app, url, options);
        }

        private class IOSInitializer : IPlatformInitializer
        {
            private ILocalNotificationService _localNotificationService;
            public IOSInitializer(ILocalNotificationService localNotificationService)
            {
                _localNotificationService = localNotificationService;
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                containerRegistry.RegisterInstance<IDeviceStorageService>(new DeviceStorageService_iOS());
                containerRegistry.RegisterInstance<INotificationService>(new NotificationService_iOS());
                containerRegistry.RegisterInstance<IAudioService>(new AudioService_iOS());
                containerRegistry.RegisterInstance<IBadgeService>(new BadgeService_iOS());
                containerRegistry.RegisterInstance<ILocalNotificationService>(_localNotificationService);
                //containerRegistry.RegisterInstance<IUpdateService>(new UpdateService_iOS());

                containerRegistry.RegisterInstance<IAppVersionService>(new AppVersionService_iOS(AppConstants.App.APPCENTER_VERSION_IOS_URL, AppConstants.App.APPCENTER_INSTALL_IOS_URL));

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
    }

}
