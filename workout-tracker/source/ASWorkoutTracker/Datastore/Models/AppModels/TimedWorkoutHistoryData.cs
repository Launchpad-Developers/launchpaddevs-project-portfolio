using System;
namespace ASWorkoutTracker.Datastore.Models
{
    public class TimedWorkoutHistoryData
    {
        public TimeSpan MinTime { get; set; }
        public TimeSpan AverageTime { get; set; }
        public TimeSpan MaxTime { get; set; }
    }
}
