using System;
using System.Collections.Generic;
using ASCommonServices.Models;

namespace ASWorkoutTracker.Datastore.Models
{
    public class DatabaseModel
    {
        public DatabaseModel()
        {
            ExerciseLogs = new List<ExerciseLog>();
            ExerciseTypes = new List<ExerciseType>();
            Routines = new List<Routine>();
            RoutineExercises = new List<RoutineExercise>();
            Exercises = new List<Exercise>();
            BodyAreas = new List<BodyAreaType>();
            EquipmentTypes = new List<EquipmentType>();
            Levels = new List<Level>();
            Programs = new List<Program>();
            Medals = new List<Medal>();
            UserMedals = new List<UserMedal>();
        }

        public DatabaseInfoModel DatabaseInfo { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<ExerciseLog> ExerciseLogs { get; set; }
        public List<ExerciseType> ExerciseTypes { get; set; }
        public List<RoutineExercise> RoutineExercises { get; set; }
        public List<Routine> Routines { get; set; }
        public List<BodyAreaType> BodyAreas { get; set; }
        public List<EquipmentType> EquipmentTypes { get; set; }
        public List<Level> Levels { get; set; }
        public List<Program> Programs { get; set; }
        public List<Medal> Medals { get; set; }
        public List<UserMedal> UserMedals { get; set; }

        public int TotalCount {  get { return Exercises.Count + ExerciseLogs.Count +
                                              ExerciseTypes.Count + RoutineExercises.Count + Routines.Count +
                                              BodyAreas.Count + EquipmentTypes.Count +
                                              Levels.Count + Programs.Count + Medals.Count + UserMedals.Count; } }

        public override string ToString()
        {
            return $"{DatabaseInfo}\n" +
                   $"{nameof(Exercise)}s: {Exercises.Count}\n" +
                   $"{nameof(ExerciseType)}s: {ExerciseTypes.Count}\n" +
                   $"{nameof(Routine)}s: {Routines.Count}\n" +
                   $"{nameof(RoutineExercise)}s: {RoutineExercises.Count}\n" +
                   $"{nameof(BodyAreaType)}s: {BodyAreas.Count}\n" +
                   $"{nameof(ExerciseLog)}s: {ExerciseLogs.Count}\n" +
                   $"{nameof(EquipmentType)}s: {EquipmentTypes.Count}\n" +
                   $"{nameof(Level)}s: {Levels.Count}\n" +
                   $"{nameof(Program)}s: {Programs.Count}\n" +
                   $"{nameof(Medal)}s: {Medals.Count}\n" +
                   $"{nameof(UserMedal)}s: {UserMedals.Count}\n";
        }

        public void SetSystemData()
        {
            foreach (var d in Exercises)
            {
                if (d == null) continue;
                d.IsSystem = true;
                if (d.DateLastUpdated == null)
                    d.DateLastUpdated = DateTime.MinValue;
            }
            foreach (var d in ExerciseLogs)
            {
                if (d == null) continue;
                d.IsSystem = true;
            }
            foreach (var d in ExerciseTypes)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in RoutineExercises)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in Routines)
            {
                if (d == null) continue;
                d.IsSystem = true;
                if (d.DateLastUpdated == null)
                    d.DateLastUpdated = DateTime.MinValue;
            }
            foreach (var d in BodyAreas)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in EquipmentTypes)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in Levels)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in Programs)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in Medals)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
            foreach (var d in UserMedals)
            {
                if (d == null) continue;
                d.IsSystem = true;
                d.DateLastUpdated = d.DateCreated;
            }
        }
    }
}
