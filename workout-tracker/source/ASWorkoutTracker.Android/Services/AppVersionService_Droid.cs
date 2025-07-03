using System;
using System.Reflection;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.Interop;
using Plugin.CurrentActivity;
using Xamarin.Forms;

using static Android.Provider.Settings;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASWorkoutTracker.Android.Services;

[assembly: Dependency(typeof(AppVersionService_Droid))]
namespace ASWorkoutTracker.Android.Services
{
    public class AppVersionService_Droid : IAppVersionService
    {
        private Context _context;
        public AppVersionService_Droid(Context context)
        {
            _context = context;
        }
        public string VersionNumber { get { return $"{CurrentVersionName}.{CurrentVersionNumber}"; } }
        string CurrentVersionName => _context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionName;
        int CurrentVersionNumber => _context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionCode;

        public string VersionCheckUrl => string.Empty; // Constants.Misc.APPCENTER_VERSION_ANDROID_URL;
        public string InstallUrl => string.Empty; // Constants.Misc.APPCENTER_INSTALL_ANDROID_URL;

        public bool NewerVersionAvailable(ReleaseInfo latestRelease)
        {
            var latestVersion = new Version(latestRelease.ShortVersion);
            var current = new Version(CurrentVersionName);
            //var number = CurrentVersionNumber;

            return latestVersion.CompareTo(current) > 0;
        }

        public string GetDeviceId(string prefix = null, string suffix = null)
        {
            var id = GetDeviceSerial();
            if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
            {
                try
                {
                    var context = CrossCurrentActivity.Current.Activity ?? Android.App.Application.Context;
                    id = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex);
                }
            }

            //var deviceId = $"{id}_{Guid.NewGuid().ToString("N")}";
            var deviceId = id;

            if (!string.IsNullOrEmpty(prefix))
                deviceId = $"{prefix}_Droid:{deviceId}";
            else
                deviceId = $"Droid:{deviceId}";

            return deviceId;
        }

        public string GetFormsVersion()
        {
            var assembly = typeof(Page).GetTypeInfo().Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);
            var version = assemblyName.Version.ToString();

            return version;
        }

        static JniPeerMembers buildMembers = new XAPeerMembers("android/os/Build", typeof(Build));
        public string GetDeviceSerial()
        {
            try
            {
                const string id = "SERIAL.Ljava/lang/String;";
                var value = buildMembers.StaticFields.GetObjectValue(id);
                return JNIEnv.GetString(value.Handle, JniHandleOwnership.TransferLocalRef);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetDeviceBaseUrl()
        {
            return "file:///android_asset";
        }
    }
}
