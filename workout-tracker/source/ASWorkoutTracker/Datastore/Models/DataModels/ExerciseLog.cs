using System;
using System.Collections.Generic;
using ASCommonServices.Models;
using ASWorkoutTracker.Enums;
using SQLite;

namespace ASWorkoutTracker.Datastore.Models
{
    public class ExerciseLog : BaseSQLModel
    {
        public int UserID { get; set; }
        public int RoutineExerciseID { get; set; }
        public int ExerciseID { get; set; }
        public int RoutineID { get; set; }
        public int ProgramID { get; set; }

        public int Reps { get; set; }
        public int Steps { get; set; }
        public int Calories { get; set; }
        public int HeartRate { get; set; }
        public TimeSpan Time { get; set; }
        public double Weight { get; set; }
        public double Distance { get; set; }
        public double Elevation { get; set; }
        public string WeightSystemString { get; set; }
        public string DistanceSystemString { get; set; }
        public string ElevationSystemString { get; set; }

        public DateTime ExerciseDate { get; set; }
        public MeasurementSystem MeasurementSystem { get; set; }

        [Ignore]
        public string ResultDetail { get { return LogDetail(); } }
        [Ignore]
        public double ResultFontSize { get { return 22.0; } }

        [Ignore]
        public new string Name { get; set; }
        [Ignore]
        public new string NameCode { get; set; }

        //[Ignore]
        //public Exercise Exercise { get; set; }
        //[Ignore]
        //public RoutineExercise RoutineExercise { get; set; }
        //[Ignore]
        //public Routine Routine { get; set; }
        //[Ignore]
        //public Program Program { get; set; }

        public override bool Equals(object obj)
        {
            var other = (ExerciseLog)obj;
            return other.ID == ID &&
                    other.Name == Name &&
                    other.NameCode == NameCode &&
                    other.IsSystem == IsSystem &&
                    other.DateCreated == DateCreated &&
                    other.DateLastUpdated == DateLastUpdated &&
                    other.UserID == UserID &&
                    other.ExerciseID == ExerciseID &&
                    other.RoutineExerciseID == RoutineExerciseID &&
                    other.ProgramID == ProgramID &&
                    other.Reps == Reps &&
                    other.Time == Time &&
                    other.Weight == Weight &&
                    other.Distance == Distance &&
                    other.Elevation == Elevation &&
                    other.WeightSystemString == WeightSystemString &&
                    other.DistanceSystemString == DistanceSystemString &&
                    other.ElevationSystemString == ElevationSystemString;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private string LogDetail()
        {
            if (Weight > 0)
            {
                if (Reps > 0)
                    return $"Performed {Reps} reps @ {Weight} {WeightSystemString}";
                else if (Distance > 0)
                    return $"Carried {Weight} {WeightSystemString} {Distance} {DistanceSystemString}";
            }

            if (Steps > 0)
            {
                if (Distance > 0)
                {
                    if (Time > TimeSpan.Zero)
                        return $"Travelled {Distance} {DistanceSystemString} and took {Steps} steps in {string.Format(@"{0:h\:mm\:ss}", Time)}";
                    else
                        return $"Travelled {Distance} {DistanceSystemString} and took {Steps} steps";
                }

                if (Elevation > 0)
                {
                    if (Time > TimeSpan.Zero)
                        return $"Climbed {Elevation} {ElevationSystemString} and took {Steps} steps in {string.Format(@"{0:h\:mm\:ss}", Time)}";
                    else
                        return $"Climbed {Elevation} {ElevationSystemString} and took {Steps} steps";
                }

                if (Time > TimeSpan.Zero)
                    return $"Took {Steps} steps in {string.Format(@"{0:h\:mm\:ss}", Time)}";
            }         

            if (Distance > 0 && Time > TimeSpan.Zero)            
                return $"Travelled {Distance} {DistanceSystemString} in {string.Format(@"{0:h\:mm\:ss}", Time)}";            


            //Generic label
            List<string> parts = new List<string>();

            if (Weight > 0)
                parts.Add($"Weight: {Weight} {WeightSystemString}");

            if (Reps > 0)
                parts.Add($"Reps: {Reps}");

            if (Elevation > 0)
                parts.Add($"Elevation: {Elevation} {ElevationSystemString}");

            if (Distance > 0)
                parts.Add($"Distance: {Distance} {DistanceSystemString}");

            if (Steps > 0)
                parts.Add($"Steps: {Steps}");

            if (Time > TimeSpan.Zero)
                parts.Add($"Time: {string.Format(@"{0:h\:mm\:ss}", Time)}");

            return string.Join(" | ", parts.ToArray());
        }
    }
}
