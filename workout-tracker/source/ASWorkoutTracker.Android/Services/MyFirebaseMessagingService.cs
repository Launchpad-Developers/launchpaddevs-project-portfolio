using System;
using Android.App;
using Android.Content;
using Android.Util;
using ASCommonServices.Interfaces;

namespace ASWorkoutTracker.Android.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : ILocalNotificationService //,FirebaseMessagingService
    {

        const string TAG = "MyFirebaseMsgService";

        //public override void OnMessageReceived(RemoteMessage message)
        //{
        //    Log.Debug(TAG, "From: " + message.From);
        //    var setTime = AppSettings.GetValueOrDefault(Constants.AppSettingsKeys.AreasSetTime, DateTime.MinValue).ToLocalTime();
        //    if (!AppSettings.GetValueOrDefault(Constants.AppSettingsKeys.AreasSet, false) ||
        //        setTime > DateTime.Now.AddSeconds(-Constants.Misc.NotificationDelaySeconds))
        //    {
        //        return;
        //    }
        //    else if (message.GetNotification() != null)
        //    {
        //        //These is how most messages will be received
        //        Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
        //        DisplayLocalNotification(message.GetNotification().Body);
        //    }
        //    else
        //    {
        //        //Only used for debugging payloads sent from the Azure portal
        //        DisplayLocalNotification(message.Data.Values.First());
        //    }

        //}

        public async void DisplayLocalNotification(string messageBody)
        {
            try
            {
                //var context = Android.App.Application.Context;
                //var intent = new Intent(this, typeof(MainActivity));
                //intent.AddFlags(ActivityFlags.ClearTop);
                //var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);

                //// Build the notification:
                //var builder = new NotificationCompat.Builder(context, Constants.Misc.CHANNEL_ID)
                //              .SetAutoCancel(true)
                //              .SetContentIntent(pendingIntent)
                //              .SetContentTitle("New Task")
                //              .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                //              .SetContentText(messageBody);

                //// Finally, publish the notification:
                //var notificationManager = NotificationManagerCompat.From(context);
                //notificationManager.Notify(Constants.Misc.NOTIFICATION_ID, builder.Build());

                //var activity = CrossCurrentActivity.Current.Activity;
                //activity.RunOnUiThread(() => DisplayLocalAlert(messageBody, activity));

                //var tone = AppSettings.GetValueOrDefault(Constants.Misc.NotificationTone, string.Empty);
                //if (!string.IsNullOrEmpty(tone) && tone != Constants.Misc.SilentNotificationTone)
                //{
                //    AudioService_Droid player = new AudioService_Droid();
                //    await player.PlayAudioFile(tone, null);
                //}
            }
            catch (Exception ex)
            {
                Log.Debug(TAG, $"From: {ex.Message}");
            }
        }

        private void DisplayLocalAlert(string message, Activity activity)
        {

            AlertDialog.Builder alert = new AlertDialog.Builder(activity);
            alert.SetTitle("Tasks Updated");
            alert.SetMessage(message);
            alert.SetPositiveButton("OK", (senderAlert, args) => {
                //This call will fire UI changes, but the UI thread is currently under
                //the notification Activity's control, and if called before
                //control is returned, the app will crash
                //MessagingCenter.Send<ILocalNotificationService>(this, "RefreshUserData");
            });

            Dialog dialog = alert.Create();
            dialog.Show();                     
        }
    }
}
