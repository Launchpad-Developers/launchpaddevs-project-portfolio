using System;
using ASCommonServices.Models;
using SQLite;

namespace ASWorkoutTracker.Datastore.Models
{
    public class RoutineExercise : BaseSQLModel
    {
        public RoutineExercise()
        {
            TimeToComplete = TimeSpan.FromMinutes(0);
            TimeBetweenSets = TimeSpan.FromMinutes(0);
            TimeAfterExercise = TimeSpan.FromMinutes(0);
            TargetNumberOfSets = 1;
            Order = 0;
        }

        [NotNull]
        public int RoutineID { get; set; }
        [NotNull]
        public int ExerciseID { get; set; }


        public bool RecordsWeight { get; set; }
        public bool RecordsReps { get; set; }
        public bool RecordsTime { get; set; }
        public bool RecordsSteps { get; set; }
        public bool RecordsDistance { get; set; }
        public bool RecordsElevation { get; set; }
        public bool RecordsHeartRate { get; set; }
        public bool RecordsCalories { get; set; }
        public string SpecialInstructions { get; set; }

        //Perform the exercise at least this long
        public TimeSpan TimeToComplete { get; set; }

        //Take this much time between sets
        public int TargetNumberOfSets { get; set; }
        public TimeSpan TimeBetweenSets { get; set; }

        //Take this much time after exercise
        public TimeSpan TimeAfterExercise { get; set; }

        //New Name so we can get rid of the [NotNull] attribute
        public new string Name { get; set; }

        private int _order;
        [NotNull]
        public int Order
        {
            get { return _order; }
            set { _order = value; OnPropertyChanged(nameof(Order)); }
        }

        #region VmProperties

        [Ignore]
        public Routine Routine { get; set; }
        [Ignore]
        public Exercise Exercise { get; set; }

        private bool _isFirst;
        [Ignore]
        public bool IsFirst
        {
            get { return _isFirst; }
            set { _isFirst = value; OnPropertyChanged(nameof(IsFirst)); }
        }

        private bool _isLast;
        [Ignore]
        public bool IsLast
        {
            get { return _isLast; }
            set { _isLast = value; OnPropertyChanged(nameof(IsLast)); }
        }

        #endregion
    }
}
