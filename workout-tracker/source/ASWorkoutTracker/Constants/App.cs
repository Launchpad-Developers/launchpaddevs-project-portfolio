using System;

namespace ASWorkoutTracker
{
    public static partial class AppConstants
    {
        public class App
        {
            //TODO Get correct API endpoint URLs
            public const string APPCENTER_INSTALL_IOS_URL = "https://installs.myfacilityfitpro.com/";
            public const string APPCENTER_INSTALL_ANDROID_URL = "https://installs.myfacilityfitpro.com/";

            public const string APPCENTER_VERSION_IOS_URL = "https://installs.myfacilityfitpro.com/";
            public const string APPCENTER_VERSION_ANDROID_URL = "https://installs.myfacilityfitpro.com/";

            public const string DATA_FOLDER = "LocalData";
            public const string DATABASE_FILE_NAME = "WorkoutTrackerDB.sqlite";
            public const string DID_RATE_APP = "DID_RATE_APP";

            public const string DEFAULT_EQUIPMENT_TYPE = "DEFAULT_EQUIPMENT_TYPE";
            public const string DEFAULT_BODY_AREA_TYPE = "DEFAULT_BODY_AREA_TYPE";
            public const string DEFAULT_EXERCISE_TYPE = "DEFAULT_EXERCISE_TYPE";
            public const string DEFAULT_MEASUREMENT_TYPE = "DEFAULT_MEASUREMENT_TYPE";
            public const string DEFAULT_LEVEL = "DEFAULT_LEVEL";
            public const string DEFAULT_ROUTINE_TYPE = "DEFAULT_ROUTINE_TYPE";

            public const string DEFAULT_MEASUREMEMT_SYSTEM = "DEFAULT_MEASUREMEMT_SYSTEM";            
            public const string MEASUREMEMT_SYSTEM = "MEASUREMEMT_SYSTEM";

            public const string DEFAULT_WEIGHT_SYSTEM = "lb";
            public const string WEIGHT_SYSTEM = "WEIGHT_SYSTEM";

            public const string DEFAULT_DISTANCE_SYSTEM = "mi";
            public const string DISTANCE_SYSTEM = "DISTANCE_SYSTEM";

            public const string DEFAULT_ELEVATION_SYSTEM = "ft";
            public const string ELEVATION_SYSTEM = "ELEVATION_SYSTEM";

            public const string DEFAULT_1RM_SYSTEM = "Brzycki";
            public const string ONE_RM_SYSTEM = "ONE_RM_SYSTEM";

            public const string EXERCISES_FILTER = "EXERCISES_FILTER";
            public const string ROUTINES_FILTER = "ROUTINES_FILTER";
            public const string ADD_ROUTINE_EXERCISE_FILTER = "ADD_ROUTINE_EXERCISE_FILTER";
            public const int DEFAULT_ROUTINE_ID = 1000000;

            public const int GROUPED_HISTORY_PAGE_SIZE = 10;
            public const int ROUTINE_EXERCISES_PAGE_SIZE = 100;
            public const int EXERCISES_PAGE_SIZE = 100;

            public const string APPID_IOS = "AppIDiOS";
            public const string APPID_DROID = "AppIDDroid";
        }
    }
}
