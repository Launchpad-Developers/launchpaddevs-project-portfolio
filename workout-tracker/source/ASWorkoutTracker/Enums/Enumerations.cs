using System;
using AS.Forms.Controls.Enums;

namespace ASWorkoutTracker.Enums
{
    public enum FirebaseTable
    {
        BodyAreaType,
        EquipmentType,
        Exercise,
        ExerciseLog,
        ExerciseType,
        Routine,
        RoutineExercise,
        DatabaseModel,
        Medal,
        UserMedal,
        Level
    }

    public enum TypeTable
    {
        BodyAreaType,
        EquipmentType,
        ExerciseType,
        Level
    }

    public enum MeasurementSystem
    {
        Imperial,
        Metric
    }

    public enum AutoRoutine
    {
        User,
        Auto
    }

    public enum TimedRoutine
    {
        NotTimed,
        Timed        
    }

    public enum TimeRoutine
    {
        TimeToBeat,
        TimeToComplete
    }

    public enum RepsRoutine
    {
        NoReps,
        Reps
    }

    public enum SetsRoutine
    {
        OneSet,
        MultipleSets
    }

    public enum BreakRoutine
    {
        NoBreak,
        Break
    }

    public enum TimerControlCommands
    {
        StartTimer,
        StopTimer,
        PauseTimer,
        ResumeTimer,
        ResetTimer
    }

    public enum MedalRank
    {
        Gold,
        Silver,
        Bronze
    }

    public enum RoutineType
    {
        SingleExercise = 0,
        RoutineWorkout
    }

    //Used to determine which control panels are showing
    public enum RoutineExerciseType
    {
        Manual = 0,
        Timed
    }

    public enum RoutineState
    {
        Manual_InProgress = 0,
        Timed_Start,
        Timed_InProgress,
        Set_Recovery,
        Exercise_Recovery
    }

    public sealed class PickerItemPropertyNames : ASEnum
    {
        public static readonly PickerItemPropertyNames SelectedExerciseType = new PickerItemPropertyNames("SelectedExerciseType");
        public static readonly PickerItemPropertyNames SelectedEquipmentType = new PickerItemPropertyNames("SelectedEquipmentType");
        public static readonly PickerItemPropertyNames SelectedBodyAreaType = new PickerItemPropertyNames("SelectedBodyAreaType");
        public static readonly PickerItemPropertyNames SelectedLevel = new PickerItemPropertyNames("SelectedLevel");
        public static readonly PickerItemPropertyNames SelectedRoutineType = new PickerItemPropertyNames("SelectedRoutineType");

        private PickerItemPropertyNames(string value)
          : base(value)
        {
        }
    }

    public sealed class RoutineExerciseSetupOptionTypes : ASEnum
    {
        public static readonly RoutineExerciseSetupOptionTypes AutoManualOptionType = new RoutineExerciseSetupOptionTypes("AutoManualOptionType");
        public static readonly RoutineExerciseSetupOptionTypes TimedNotTimedOptionType = new RoutineExerciseSetupOptionTypes("TimedNotTimedOptionType");
        public static readonly RoutineExerciseSetupOptionTypes TimeCompleteTimeBeatOptionType = new RoutineExerciseSetupOptionTypes("TimeCompleteTimeBeatOptionType");
        public static readonly RoutineExerciseSetupOptionTypes RepsNoRepsOptionType = new RoutineExerciseSetupOptionTypes("RepsNoRepsOptionType");
        public static readonly RoutineExerciseSetupOptionTypes SetsOptionType = new RoutineExerciseSetupOptionTypes("SetsOptionType");
        public static readonly RoutineExerciseSetupOptionTypes BreakNoBreakOptionType = new RoutineExerciseSetupOptionTypes("BreakNoBreakOptionType");

        private RoutineExerciseSetupOptionTypes(string value)
          : base(value)
        {
        }
    }
}
