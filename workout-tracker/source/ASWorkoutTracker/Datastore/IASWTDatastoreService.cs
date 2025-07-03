using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASCommonServices.Enums;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore.Models;
using ASWorkoutTracker.Enums;

namespace ASWorkoutTracker.Datastore
{
    public interface IASWTDatastoreService
    {
        //Utility
        Task FinishLogin(LoginResult loginResult);

        /// <summary>
        /// Current version number of the local datastore.
        /// </summary>
        /// <returns>Int value for current version number.</returns>
        Task<int> GetDatabaseVersion();

        /// <summary>
        /// Determines if the local database is out of sync with Firebase data.
        /// </summary>
        /// <returns>TRUE if there are updates on the remote Firebase store.</returns>
        Task<bool> CheckForRemoteDatabaseUpdates();

        /// <summary>
        /// Determines if the database exists and has integrity
        /// </summary>
        /// <returns>TRUE if the database file already exists.</returns>
        bool DatabaseIsIntact();

        /// <summary>
        /// Delete data from the specified table types. 
        /// </summary>
        /// <param name="type">Type of tables to clear</param>
        /// <returns>True if tables are successfully cleared.</returns>
        Task<bool> ClearData(DatabaseDataType type);

        /// <summary>
        /// Completely erases local data including user data and exercise logs, then recreates the database using latest Firebase store data.
        /// </summary>
        /// <returns>TRUE if the operation completed successfully.</returns>
        Task<bool> RestoreDatabase();

        /// <summary>
        /// Creates a new record of type {T}.
        /// </summary>
        /// <typeparam name="T">BaseSQLModel</typeparam>
        /// <param name="obj">Object of type BaseSQLModel</param>
        /// <returns>True if record created successfully.</returns>
        Task<bool> Create<T>(T obj) where T : new();

        /// <summary>
        /// Returns a record of type {T} whose ID matches the supplied value.
        /// </summary>
        /// <typeparam name="T">BaseSQLModel</typeparam>
        /// <param name="obj">Object of type BaseSQLModel</param>
        /// <returns>Instance of {T} matching iD.</returns>
        Task<T> Read<T>(int iD) where T : new();

        /// <summary>
        /// Updates or inserts object of type {T}.
        /// </summary>
        /// <typeparam name="T">BaseSQLModel</typeparam>
        /// <param name="obj">Object of type BaseSQLModel</param>
        /// <returns>True if record created successfully.</returns>
        Task<bool> Update<T>(T obj) where T : new();

        /// <summary>
        /// Set DateLastUpdated on the given routine to today's date.
        /// </summary>
        /// <param name="routineId">Id of the routine to be updated.</param>
        /// <returns>True if successful.</returns>
        Task<bool> UpdateRoutineWorkoutDate(int routineId);

        /// <summary>
        /// Set DateLastUpdated on the given routine to today's date.
        /// </summary>
        /// <param name="routineId">Id of the routine to be updated.</param>
        /// <returns>True if successful.</returns>
        Task<bool> UpdateExerciseWorkoutDate(int exerciseId);

        /// <summary>
        /// Deletes record of type {T}.
        /// </summary>
        /// <typeparam name="T">BaseSQLModel</typeparam>
        /// <param name="obj">Object of type BaseSQLModel</param>
        /// <returns>True if record deleted successfully.</returns>
        Task<bool> Delete<T>(T obj) where T : new();

        /// <summary>
        /// Deletes Exercise, RoutineExercises based on this Exercise, and any History for this exercise.
        /// </summary>
        /// <returns>True if records deleted successfully.</returns>
        Task<bool> DeleteExercise(Exercise exercise);

        /// <summary>
        /// Deletes Routine, RoutineExercises based on this Routine, and any History for this routine.
        /// </summary>
        /// <returns>True if records deleted successfully.</returns>
        Task<bool> DeleteRoutine(Routine routine);

        /// <summary>
        /// Returns all objects of type {T}.
        /// </summary>
        /// <typeparam name="T">BaseSQLModel</typeparam>
        /// <returns>List<T></returns>
        Task<List<T>> ReadAllRecordsOfType<T>() where T : new();
        
        /// <summary>
        /// Returns all records of type {T} matching conditional.
        /// </summary>
        /// <typeparam name="T">BaseSQLModel</typeparam>
        /// <param name="condition">Func<T, bool></param>
        /// <returns>List<T></returns>
        Task<List<T>> ReadWithCondition<T>(Func<T, bool> condition, int? page, int? take) where T : new();
        
        /// <summary>
        /// Returns all Exercises whose RoutineID matches the supplied value.
        /// </summary>
        /// <param name="routineId">Object of type BaseSQLModel</param>
        /// <returns>List<RoutineExercise> matching routineId.</returns>
        Task<List<RoutineExercise>> ReadRoutineExercises(int routineId, int? page, int? take);

        /// <summary>
        /// Returns the default routine exercise for the given exercise ID.
        /// </summary>
        /// <returns>RoutineExercise matching routineId.</returns>
        Task<RoutineExercise> ReadDefaultRoutineExercise(int exerciseID);

        /// <summary>
        /// Returns all Exercises not already added to the calling routine.
        /// </summary>
        /// <param name="existingExerciseIDs">List<int> containing all exercises already in the calling Routine.</param>
        /// <returns>List<Exercise></returns>
        //Task<List<Exercise>> ReadAvailableRoutineExercises(List<int> existingExerciseIDs, int? page, int? take);

        /// <summary>
        /// Swaps the Order value of the two given RoutineExercise records.
        /// Lowest value appears first in the 
        /// </summary>
        /// <param name="moveUp">The Order value of this RoutineExercise will increment.</param>
        /// <param name="moveDown">The Order value of this RoutineExercise will decrement.</param>
        /// <returns>True if transaction completes successfully.</returns>
        Task<bool> SwapRoutineExercises(RoutineExercise moveUp, RoutineExercise moveDown);

        /// <summary>
        /// Returns all Exercises for the current user, fully populated.
        /// </summary>
        /// <returns>List<Exercise></returns>
        Task<List<Exercise>> ReadAllExercises();

        /// <summary>
        /// Returns last workout for the given exercise.
        /// </summary>
        /// <param name="exerciseId">The ID of the referenced Exercise</param>
        /// <param name="routineExerciseId">Nullable ID of the referenced Routine</param>
        /// <returns>List<ExerciseLog></returns>
        Task<List<ExerciseLog>> ReadLastWorkout(int exerciseId, int? routineExerciseId = null);

        /// <summary>
        /// Returns last workout for the given exercise.
        /// </summary>
        /// <param name="exercise">The referenced Exercise</param>
        /// <param name="routineExerciseId">Nullable ID of the referenced Routine</param>
        /// <returns>Exerciseg</returns>
        Task ReadLastSet(Exercise exercise, int? routineExerciseId = null);

        /// <summary>
        /// Returns all Exercise Logs for the given Exercise ID recorded since the given start date.
        /// </summary>
        /// <param name="exercise">Exercise</param>
        /// <param name="startDate">DateTime starting date</param>
        /// <returns>Dictionary<DateTime, ExerciseLog></returns>
        Task<List<Grouping<DateTime, ExerciseLog>>> ReadGroupedExerciseLogs(Exercise exercise, DateTime startDate, int? page, int? take, int? routineExerciseId = null);

        /// <summary>
        /// Returns number of workouts and total sets for a given exercise or routine exercise.
        /// </summary>
        /// <param name="exerciseId">Required ExerciseID</param>
        /// <param name="startDate">Date to pull from</param>
        /// <param name="routineExerciseId">Optional RoutineID</param>
        /// <returns>GenericWorkoutHistoryData</returns>
        Task<GenericWorkoutHistoryData> ReadTotalWorkoutsAndSets(int exerciseId, DateTime startDate, int? routineExerciseId = null);

        /// <summary>
        /// Returns Min, Avg, and Max Time for the given Exercise or Routine Exercise.
        /// </summary>
        /// <param name="exerciseId">Required ExerciseID</param>
        /// <param name="startDate">Date to pull from</param>
        /// <param name="routineExerciseId">Optional RoutineID</param>
        /// <returns>TimedWorkoutHistoryData</returns>
        Task<TimedWorkoutHistoryData> ReadTotalWorkoutTimes(int exerciseId, DateTime startDate, int? routineExerciseId = null);

        /// <summary>
        /// Returns Longest Distance and Total Distance for the given Exercise or Routine Exercise.
        /// </summary>
        /// <param name="exerciseId">Required ExerciseID</param>
        /// <param name="startDate">Date to pull from</param>
        /// <param name="routineExerciseId">Optional RoutineID</param>
        /// <returns>DistanceHistoryData</returns>
        Task<DistanceHistoryData> ReadDistances(int exerciseId, DateTime startDate, int? routineExerciseId = null);

        /// <summary>
        /// Returns Highest Elevation and Total Elevation for the given Exercise or Routine Exercise.
        /// </summary>
        /// <param name="exerciseId">Required ExerciseID</param>
        /// <param name="startDate">Date to pull from</param>
        /// <param name="routineExerciseId">Optional RoutineID</param>
        /// <returns>ElevationHistoryData</returns>
        Task<ElevationHistoryData> ReadElevations(int exerciseId, DateTime startDate, int? routineExerciseId = null);

        /// <summary>
        /// Returns Max Steps and Total Steps for the given Exercise or Routine Exercise.
        /// </summary>
        /// <param name="exerciseId">Required ExerciseID</param>
        /// <param name="startDate">Date to pull from</param>
        /// <param name="routineExerciseId">Optional RoutineID</param>
        /// <returns>StepsHistoryData</returns>
        Task<StepsHistoryData> ReadSteps(int exerciseId, DateTime startDate, int? routineExerciseId = null);

        /// <summary>
        /// Gets count of records for each table.
        /// </summary>
        /// <returns>Dictionary<string, int> where key is the table name and value is the count.</returns>
        Task<Dictionary<string, int>> GetStats();

        /// <summary>
        /// Updates all system data to the latest versions but leaves user defined data intact.
        /// </summary>
        /// <returns>TRUE if the operation completed successfully.</returns>
        Task<bool> RefreshDatabase();

        /// <summary>
        /// Updates system data for the requested type to the latest versions but leaves user defined data intact.
        /// </summary>
        /// <returns>TRUE if the operation completed successfully.</returns>
        Task<bool> RefreshFirebaseData(FirebaseTable table, bool retainUserData);

        /// <summary>
        /// Checks if record by same name exists in the given table.
        /// </summary>
        /// <typeparam name="T">Table model</typeparam>
        /// <param name="name">Name</param>
        /// <returns>True if name does not exist and record can be saved.</returns>
        Task<bool> CanSaveUniqueRecord<T>(string name) where T : new();

        /// <summary>
        /// Returns filters applicable to Exercise lists.
        /// </summary>
        /// <returns>List<Filter></returns>
        Task<List<Filter>> ReadExerciseFilters();

        /// <summary>
        /// Returns filters applicable to Routine lists.
        /// </summary>
        /// <returns>List<Filter></returns>
        Task<List<Filter>> ReadRoutineFilters();
    }
}
