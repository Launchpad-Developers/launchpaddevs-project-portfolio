using System;
using System.Linq;
using ASCommonServices;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Datastore.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace ASWorkoutTracker.Datastore
{
    public class ASWTSession : ISession
    {
        public bool InternetConnected
        {
            get
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var allowCellular = Preferences.Get(Constants.Misc.AllowCellData, false);

                    if (allowCellular)
                        return (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi) || Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular));
                    else
                        return Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
                }

                return false;
            }
        }

        public IUser User
        {
            get { return JsonConvert.DeserializeObject<ASWTUser>(Preferences.Get(Constants.Misc.User, string.Empty)); }
            set { Preferences.Set(Constants.Misc.User, JsonConvert.SerializeObject(value)); }
        }
    }
}
