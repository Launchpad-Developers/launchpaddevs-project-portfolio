using Android.App;
using ASCommonServices.Interfaces;

namespace ASWorkoutTracker.Android.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : INotificationService //,FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        //NotificationHub hub;

        //public override void OnTokenRefresh()
        //{
        //    var refreshedToken = FirebaseInstanceId.Instance.Token;
        //    Log.Debug(TAG, "FCM token: " + refreshedToken);
        //    AppSettings.AddOrUpdateValue(Constants.Misc.DevicePushTokenKey, FirebaseInstanceId.Instance.Token);
        //}

        public void SendRegistrationToServer(string employeeID)
        {
//#if DEBUG
//            return;
//#endif

            //try
            //{
            //    var token = FirebaseInstanceId.Instance.Token;
            //    if (token == null)
            //    {
            //        Analytics.TrackEvent("AppDelegate Instance DeviceToken null", new Dictionary<string, string> {
            //            { "aps", AppSettings.ToString() }
            //        });
            //        token = AppSettings.GetValueOrDefault(Constants.Misc.DevicePushTokenKey, null);
            //    }

            //    if (token == null)
            //    {
            //        Analytics.TrackEvent("AppSettings DeviceToken null", new Dictionary<string, string> {
            //            { "aps", AppSettings.ToString() }
            //        });
            //        return;
            //    }

            //    // Register with Notification Hubs
            //    hub = new NotificationHub(Constants.Misc.NotificationHubName, Constants.Misc.ListenConnectionString, Android.App.Application.Context);

            //    List<string> tags = new List<string> { $"{employeeID}MA" };
            //    await Task.Run(() => hub.UnregisterAll(token));
            //    Registration reg = await Task.Run(() => hub.Register(token, tags.ToArray()));

            //    if (string.IsNullOrEmpty(reg.RegistrationId))
            //    {
            //        CrossToastPopUp.Current.ShowToastError("ERRORpushnotifications", Plugin.Toast.Abstractions.ToastLength.Long);
            //        Log.Debug(TAG, $"Failed registration of employee {employeeID}");

            //        Analytics.TrackEvent("Registration Error", new Dictionary<string, string> {
            //            { "aps", AppSettings.ToString() },
            //            { "token", token },
            //            { "tags", tags.ToString() }
            //        });
            //    }
            //    else
            //    {
            //        AppSettings.AddOrUpdateValue(Constants.Misc.PNSHandleKey, reg.PNSHandle);
            //        CrossToastPopUp.Current.ShowToastSuccess("SUCCESSpushnotifications", Plugin.Toast.Abstractions.ToastLength.Short);
            //        AppSettings.AddOrUpdateValue(Constants.Misc.IsRegisteredForPushNotification, true);
            //        Log.Debug(TAG, $"Successful registration of ID {reg.RegistrationId}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    CrossToastPopUp.Current.ShowToastError("ERRORpushnotifications", Plugin.Toast.Abstractions.ToastLength.Long);
            //    Log.Debug(TAG, $"Failed registration: {ex.Message}");
            //    Analytics.TrackEvent("Failed registration", new Dictionary<string, string>{
            //        { "userInfo", employeeID},
            //        { "aps", AppSettings.ToString() },
            //        { "error", ex.Message }
            //    });
            //    return;
            //}
        }

        public void SendUnregistrationToServer()
        {
            //if (hub == null)
            //    hub = new NotificationHub(Constants.Misc.NotificationHubName, Constants.Misc.ListenConnectionString, Android.App.Application.Context);

            //var handle = AppSettings.GetValueOrDefault(Constants.Misc.PNSHandleKey, null);
            //if (!string.IsNullOrEmpty(handle))
            //{
            //    await Task.Run(() => hub.UnregisterAll(handle));
            //    AppSettings.AddOrUpdateValue(Constants.Misc.IsRegisteredForPushNotification, false);
            //}
        }
    }
}
