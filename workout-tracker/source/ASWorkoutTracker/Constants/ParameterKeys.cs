using System;
namespace ASWorkoutTracker
{
    public static partial class AppConstants
    {
        public class ParameterKeys
        {
            //NavigationParameters
            public const string Reload = "reload";
            public const string Definition = "definition";
            public const string Item = "item";
            public const string Add = "add";
            public const string Edit = "edit";
            public const string Route = "route";
            public const string RoutineExercise = "routineExercise";
            public const string RoutineID = "routineID";
            public const string RoutineOrder = "routineOrder";
            public const string ExerciseIDs = "exerciseIDs";
            public const string Exercise = "exercise";
            public const string GroupedResults = "groupedResults";
            public const string Routine = "routine";
            public const string RoutineExercises = "routineExercises";
            public const string SignUp = "signup";
            public const string LoginComplete = "logincomplete";
            public const string LoginResult = "loginresult";

            //DialogParameters
            public const string SelectedRoutineExercise = "selectedRoutineExercise";

            //MessagingCenter
            public const string RoutineIsShowingDetailsChanged = "RoutineIsShowingDetailsChanged";
            public const string ExerciseIsShowingDetailsChanged = "ExerciseIsShowingDetailsChanged";
        }
    }
}
