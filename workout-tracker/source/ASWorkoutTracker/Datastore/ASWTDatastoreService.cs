using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore.Models;
using ASWorkoutTracker.Repository;
using Microsoft.AppCenter.Analytics;
using System.Linq;
using ASWorkoutTracker.Enums;
using Prism.Events;
using ASCommonServices.Events;
using ASCommonServices.Enums;

namespace ASWorkoutTracker.Datastore
{
    public class ASWTDatastoreService : IASWTDatastoreService
    {
        protected ISQLiteDatastoreService _sQLiteDatastoreService;
        protected IASWTRepositoryService _aSWTRepositoryService;
        protected IEventAggregator _eventAggregator;
        protected ISession _session;

        private string _table;

        public ASWTDatastoreService(ISQLiteDatastoreService sQLiteDatabaseService,
                                    IASWTRepositoryService aSWTRepositoryService,
                                    IEventAggregator eventAggregator,
                                    ISession session)
        {
            _sQLiteDatastoreService = sQLiteDatabaseService;
            _aSWTRepositoryService = aSWTRepositoryService;
            _eventAggregator = eventAggregator;
            _session = session;
        }

        private void TrackEvent(Exception ex, [CallerMemberName] string memberName = "")
        {
            Analytics.TrackEvent("Database Error", new Dictionary<string, string> {
                { "method", memberName},
                { "exception", ex.ToString() },
                { "table", _table }
            });
        }

        private bool _databaseisIntact { get; set; }

        public bool DatabaseIsIntact()
        {
            if (_databaseisIntact)
                return true;

            _databaseisIntact = _sQLiteDatastoreService.DatabaseIsIntact();
            return _databaseisIntact;
        }

        public async Task<bool> ClearData(DatabaseDataType type)
        {
            var result = await _sQLiteDatastoreService.ClearData(type);
            return result;
        }

        public async Task<bool> RestoreDatabase()
        {
            Type[] tables = {
                typeof(DatabaseInfoModel),
                typeof(ASWTUser),
                typeof(EquipmentType),
                typeof(Exercise),
                typeof(Routine),
                typeof(ExerciseType),
                typeof(ExerciseLog),
                typeof(RoutineExercise),
                typeof(BodyAreaType),
                typeof(Level),
                typeof(Program),
                typeof(Medal),
                typeof(UserMedal)
            };

            try
            {
                var result = await _sQLiteDatastoreService.CreateDatabase(tables, false);
                return result;
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        LoginResult _tempLoginResult;
        public async Task FinishLogin(LoginResult loginResult)
        {
            if (!DatabaseIsIntact())
                return;
            
            //Check if AuthID exists; if not, new user
            ASWTUser user = new ASWTUser();
            Func<ASWTUser, bool> condition = x => x.AuthID == loginResult.AuthID;
            user = await _sQLiteDatastoreService.GetRecordWithCondition(condition);

            if (user == null)
            {
                //New user
                var localUser = new ASWTUser
                {
                    AuthID = loginResult.AuthID,
                    PrimaryEmail = loginResult.Email,
                    UserName = loginResult.Username,
                    ProviderID = loginResult.ProviderID
                };

                await PopulateUserMedals(localUser);
                _session.User = localUser;

                var result = await Create(localUser);
                return;
            }
            else
            {
                await PopulateUserMedals(user);
                _session.User = user;
            }

            return;
        }

        public async Task<int> GetDatabaseVersion()
        {
            if (!DatabaseIsIntact())
                return 0;

            var localData = await _sQLiteDatastoreService.GetRecords<DatabaseInfoModel>();
            var data = localData.FirstOrDefault();
            return data != null ? data.Version : 0;
        }

        public async Task<bool> CheckForRemoteDatabaseUpdates()
        {
            if (!DatabaseIsIntact())
                return true;

            var localData = await _sQLiteDatastoreService.GetRecords<DatabaseInfoModel>();
            var latestVersion = await _aSWTRepositoryService.GetRemoteDatabaseInfoJson();

            if (localData == null || !localData.Any() ||
                (latestVersion != null && (localData.First().Version < latestVersion.Version)))
                return true;

            return false;
        }

        public async Task<bool> RefreshDatabase()
        {
            try
            {
                //if (!DatabaseIsIntact())
                //{
                //    var created = await RestoreDatabase();
                //    if (!created)
                //        return false;
                //}

                DatabaseModel database = await _aSWTRepositoryService.GetAllSystemData();
                database.SetSystemData();

                _table = nameof(Exercise);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.Exercises);

                _table = nameof(Routine);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.Routines);

                _table = nameof(ExerciseType);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.ExerciseTypes);

                _table = nameof(BodyAreaType);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.BodyAreas);

                _table = nameof(RoutineExercise);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.RoutineExercises);

                _table = nameof(DatabaseInfoModel);
                await _sQLiteDatastoreService.DeleteAsync("DELETE FROM DatabaseInfo");
                await _sQLiteDatastoreService.InsertOrReplaceResults(new List<DatabaseInfoModel> { database.DatabaseInfo });

                _table = nameof(ExerciseLog);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.ExerciseLogs);

                _table = nameof(EquipmentType);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.EquipmentTypes);

                _table = nameof(Level);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.Levels);

                _table = nameof(Program);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.Programs);

                _table = nameof(Medal);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.Medals);

                _table = nameof(UserMedal);
                await _sQLiteDatastoreService.InsertOrReplaceResults(database.UserMedals);

                //Future development - User tables

                if (DatabaseIsIntact() && database.TotalCount > 0)
                    _eventAggregator.GetEvent<DatastoreUpdatedEvent>().Publish();
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }

            return true;
        }

        public async Task<bool> RunDatabaseUpdateScript(string updateScript)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }

            return true;
        }

        public async Task<bool> Create<T>(T obj) where T : new()
        {
            try
            {
                _table = nameof(T);
                return await _sQLiteDatastoreService.InsertResults(new List<object> { obj });
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<T> Read<T>(int iD) where T : new()
        {
            if (!DatabaseIsIntact())
                return new T();

            try
            {
                _table = nameof(T);
                Func<T, bool> condition = x => ((BaseSQLModel)(object)x).ID == iD;
                return await _sQLiteDatastoreService.GetRecordWithCondition(condition);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new T();
            }
        }

        public async Task<List<T>> ReadAllRecordsOfType<T>() where T : new()
        {
            if (!DatabaseIsIntact())
                return new List<T>();

            try
            {
                _table = nameof(T);
                return await _sQLiteDatastoreService.GetRecords<T>();
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new List<T>();
            }
        }

        public async Task<List<T>> ReadWithCondition<T>(Func<T, bool> condition, int? page, int? take) where T : new()
        {
            if (!DatabaseIsIntact())
                return new List<T>();

            try
            {
                _table = nameof(T);
                return await _sQLiteDatastoreService.GetRecordsWithCondition<T>(condition, page, take);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new List<T>();
            }
        }

        public async Task<bool> CanSaveUniqueRecord<T>(string name) where T : new()
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(T);
                Func<T, bool> condition = x => ((BaseSQLModel)(object)x).Name == name;
                var results = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                return !results.Any();
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<bool> Update<T>(T obj) where T : new()
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(T);
                return await _sQLiteDatastoreService.InsertOrReplaceResults(new List<object> { obj });
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<bool> SwapRoutineExercises(RoutineExercise moveUp, RoutineExercise moveDown)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(RoutineExercise);
                return await _sQLiteDatastoreService.InsertOrReplaceResults(new List<RoutineExercise> { moveUp, moveDown });
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<bool> Delete<T>(T obj) where T : new()
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(T);
                return await _sQLiteDatastoreService.DeleteAsync(new List<object> { obj });
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<Dictionary<string, int>> GetStats()
        {
            List<string> tables = new List<string> {
                nameof(EquipmentType),
                nameof(Exercise),
                nameof(Routine),
                nameof(ExerciseType),
                nameof(ExerciseLog),
                nameof(RoutineExercise),
                nameof(BodyAreaType),
                nameof(Level),
                nameof(Medal),
                nameof(UserMedal)
            };

            try
            {
                _table = "ALL";
                return await _sQLiteDatastoreService.GetStaticsData(tables);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new Dictionary<string, int>();
            }
        }

        public async Task<bool> RefreshFirebaseData(FirebaseTable table, bool retainUserData)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                IEnumerable<BaseSQLModel> data = new List<BaseSQLModel>();

                switch (table)
                {
                    case FirebaseTable.BodyAreaType:
                        _table = nameof(BodyAreaType);
                        data = await _aSWTRepositoryService.GetBodyAreaTypes();
                        break;
                    case FirebaseTable.EquipmentType:
                        _table = nameof(EquipmentType);
                        data = await _aSWTRepositoryService.GetEquipmentTypes();
                        break;
                    case FirebaseTable.Exercise:
                        _table = nameof(Exercise);
                        data = await _aSWTRepositoryService.GetExercises();
                        break;
                    case FirebaseTable.ExerciseType:
                        _table = nameof(ExerciseType);
                        data = await _aSWTRepositoryService.GetExerciseTypes();
                        break;
                    case FirebaseTable.Routine:
                        _table = nameof(Routine);
                        data = await _aSWTRepositoryService.GetRoutines();
                        break;
                    case FirebaseTable.RoutineExercise:
                        _table = nameof(RoutineExercise);
                        data = await _aSWTRepositoryService.GetRoutineExercises();
                        break;
                    case FirebaseTable.Level:
                        _table = nameof(Level);
                        data = await _aSWTRepositoryService.GetLevels();
                        break;
                    case FirebaseTable.Medal:
                        _table = nameof(Medal);
                        data = await _aSWTRepositoryService.GetMedals();
                        break;
                    case FirebaseTable.ExerciseLog:
                    case FirebaseTable.DatabaseModel:
                    case FirebaseTable.UserMedal:
                    default:
                        break;
                }

                foreach (var d in data)
                {
                    d.IsSystem = true;
                    d.DateLastUpdated = d.DateCreated;
                }

                await _sQLiteDatastoreService.InsertOrReplaceResults(data);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }

            return true;
        }

        public async Task<List<RoutineExercise>> ReadRoutineExercises(int routineId, int? page, int? take)
        {
            if (!DatabaseIsIntact())
                return new List<RoutineExercise>();

            try
            {
                _table = nameof(RoutineExercise);
                Func<RoutineExercise, bool> condition = x => x.RoutineID == routineId;
                List<RoutineExercise> rawRoutineExercises = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, page, take);
                List<RoutineExercise> routineExercises = new List<RoutineExercise>();

                foreach (var raw in rawRoutineExercises)
                {
                    _table = nameof(Exercise);
                    Func<Exercise, bool> cond = x => x.ID == raw.ExerciseID;
                    var exercise = await _sQLiteDatastoreService.GetRecordWithCondition(cond);

                    if (exercise.ID == 0)
                        continue;

                    await PopulateExercise(exercise, raw.ID);
                    raw.Exercise = exercise;
                    routineExercises.Add(raw);
                }

                return routineExercises;
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new List<RoutineExercise>();
            }
        }

        //Returns the default RoutineExercise which contains the "RecordsXxxx" properties for each exercise
        public async Task<RoutineExercise> ReadDefaultRoutineExercise(int exerciseID)
        {
            if (!DatabaseIsIntact())
                return new RoutineExercise();

            try
            {
                _table = nameof(RoutineExercise);
                Func<RoutineExercise, bool> condition = x => x.ExerciseID == exerciseID && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID;
                RoutineExercise routineExercise = await _sQLiteDatastoreService.GetRecordWithCondition(condition);
                
                return routineExercise;
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new RoutineExercise();
            }
        }

        public async Task<List<Exercise>> ReadAllExercises()
        {
            if (!DatabaseIsIntact())
                return new List<Exercise>();

            try
            {
                List<Exercise> exercises = await ReadAllRecordsOfType<Exercise>();

                foreach (var exercise in exercises)
                    await PopulateExercise(exercise);

                return exercises;
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new List<Exercise>();
            }
        }

        //public async Task<List<Exercise>> ReadAvailableRoutineExercises(List<int> existingExerciseIDs, int? page, int? take)
        //{
        //    if (!DatabaseIsIntact())
        //        return new List<Exercise>();

        //    try
        //    {
        //        _table = nameof(Exercise);
        //        Func<Exercise, bool> condition = x => !existingExerciseIDs.Contains(x.ID);
        //        List<Exercise> exercises = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, page, take);
        //        foreach (var exercise in exercises)                
        //            await PopulateExercise(exercise);
                
        //        return exercises;
        //    }
        //    catch (Exception ex)
        //    {
        //        TrackEvent(ex);
        //        return new List<Exercise>();
        //    }
        //}

        public async Task<List<ExerciseLog>> ReadLastWorkout(int exerciseId, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new List<ExerciseLog>();

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exerciseId && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID;

                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                if (logs != null && logs.Count > 0)
                {
                    var date = logs.Max(x => x.ExerciseDate);
                    var lastWorkout = logs.Where(x => x.ExerciseDate == date).ToList();

                    //foreach (var log in lastWorkout)
                    //    await PopulateExerciseLog(log);
                    
                    return lastWorkout;
                }
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new List<ExerciseLog>();
        }

        public async Task ReadLastSet(Exercise exercise, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return;

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exercise.ID && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                if (logs != null && logs.Count > 0)
                {
                    var date = logs.Max(x => x.ExerciseDate);
                    var lastset = logs.Where(x => x.ExerciseDate == date).FirstOrDefault();
                    if (lastset != null)
                    {
                        //await PopulateExerciseLog(lastset);
                        //lastset.Exercise = exercise;
                        exercise.LastSetDate = lastset.ExerciseDate;
                        exercise.LastSetDetail = lastset.ResultDetail;                        
                    }

                    exercise.TotalSets = $"{logs.Count} Total Sets";
                }
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return;
        }

        public async Task<List<Grouping<DateTime, ExerciseLog>>> ReadGroupedExerciseLogs(Exercise exercise, DateTime startDate, int? page, int? take, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new List<Grouping<DateTime, ExerciseLog>>();

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exercise.ID && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID && x.ExerciseDate >= startDate;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value && x.ExerciseDate >= startDate;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, page, take);
                //foreach (var log in logs)
                //    log.Exercise = exercise;
                
                var groupedLogs = logs.OrderByDescending(x => x.ExerciseDate).GroupBy(x => x.ExerciseDate.Date).ToDictionary(group => group.First().ExerciseDate, group => group.OrderBy(x => x.ExerciseDate).ToList());

                List<Grouping<DateTime, ExerciseLog>> final = new List<Grouping<DateTime, ExerciseLog>>();
                foreach (var grouping in groupedLogs)                
                    final.Add(new Grouping<DateTime, ExerciseLog>(grouping.Key, grouping.Value));
                
                return final;
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new List<Grouping<DateTime, ExerciseLog>>();
        }

        public async Task<GenericWorkoutHistoryData> ReadTotalWorkoutsAndSets(int exerciseId, DateTime startDate, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new GenericWorkoutHistoryData
                {
                    NumberOfWorkouts = 0,
                    NumberOfSets = 0
                };

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exerciseId && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID && x.ExerciseDate >= startDate;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value && x.ExerciseDate >= startDate;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);                
                var groupedLogs = logs.OrderByDescending(x => x.ExerciseDate)
                    .GroupBy(x => x.ExerciseDate.Date)
                    .ToDictionary(group => group.First().ExerciseDate, group => group.OrderBy(x => x.ExerciseDate)
                    .ToList());

                int logCount = 0;
                List<Grouping<DateTime, ExerciseLog>> final = new List<Grouping<DateTime, ExerciseLog>>();
                foreach (var grouping in groupedLogs)
                    logCount += grouping.Value.Count;

                return new GenericWorkoutHistoryData
                {
                    NumberOfWorkouts = groupedLogs.Count,
                    NumberOfSets = logCount
                };
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new GenericWorkoutHistoryData
            {
                NumberOfWorkouts = 0,
                NumberOfSets = 0
            };
        }

        public async Task<TimedWorkoutHistoryData> ReadTotalWorkoutTimes(int exerciseId, DateTime startDate, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new TimedWorkoutHistoryData
                {
                    MinTime = TimeSpan.Zero,
                    AverageTime = TimeSpan.Zero,
                    MaxTime = TimeSpan.Zero
                };

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exerciseId && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID && x.ExerciseDate >= startDate;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value && x.ExerciseDate >= startDate;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                var minTime = logs.Where(x => x.Time > TimeSpan.Zero).Min(x => x.Time);
                var maxTime = logs.Where(x => x.Time > TimeSpan.Zero).Max(x => x.Time);
                var avgTimeTicks = logs.Where(x => x.Time > TimeSpan.Zero).Average(x => x.Time.TotalMilliseconds);
                var avgTime = avgTimeTicks > 0 ? TimeSpan.FromMilliseconds(avgTimeTicks) : TimeSpan.Zero;

                return new TimedWorkoutHistoryData
                {
                    MinTime = minTime,
                    AverageTime = avgTime,
                    MaxTime = maxTime
                };
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new TimedWorkoutHistoryData
            {
                MinTime = TimeSpan.Zero,
                AverageTime = TimeSpan.Zero,
                MaxTime = TimeSpan.Zero
            };
        }

        public async Task<DistanceHistoryData> ReadDistances(int exerciseId, DateTime startDate, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new DistanceHistoryData
                {
                    LongestDistance = 0.00,
                    TotalDistance = 0.00
                };

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exerciseId && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID && x.ExerciseDate >= startDate;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value && x.ExerciseDate >= startDate;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                var maxDistance = logs.Where(x => x.Distance > 0).Max(x => x.Distance);
                var totalDistance = logs.Sum(x => x.Distance);

                return new DistanceHistoryData
                {
                    LongestDistance = maxDistance,
                    TotalDistance = totalDistance
                };
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new DistanceHistoryData
            {
                LongestDistance = 0.00,
                TotalDistance = 0.00
            };
        }

        public async Task<ElevationHistoryData> ReadElevations(int exerciseId, DateTime startDate, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new ElevationHistoryData
                {
                    HighestElevation = 0.00,
                    TotalElevation = 0.00
                };

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exerciseId && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID && x.ExerciseDate >= startDate;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value && x.ExerciseDate >= startDate;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                var maxDistance = logs.Where(x => x.Distance > 0).Max(x => x.Distance);
                var totalDistance = logs.Sum(x => x.Distance);

                return new ElevationHistoryData
                {
                    HighestElevation = maxDistance,
                    TotalElevation = totalDistance
                };
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new ElevationHistoryData
            {
                HighestElevation = 0.00,
                TotalElevation = 0.00
            };
        }

        public async Task<StepsHistoryData> ReadSteps(int exerciseId, DateTime startDate, int? routineExerciseId = null)
        {
            if (!DatabaseIsIntact())
                return new StepsHistoryData
                {
                    MaxSteps = 0.00,
                    TotalSteps = 0.00
                };

            try
            {
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> condition = x => x.ExerciseID == exerciseId && x.RoutineID == AppConstants.App.DEFAULT_ROUTINE_ID && x.ExerciseDate >= startDate;
                if (routineExerciseId.HasValue)
                    condition = x => x.RoutineExerciseID == routineExerciseId.Value && x.ExerciseDate >= startDate;

                List<ExerciseLog> logs = await _sQLiteDatastoreService.GetRecordsWithCondition(condition, null, null);
                var maxDistance = logs.Where(x => x.Steps > 0).Max(x => x.Steps);
                var totalDistance = logs.Sum(x => x.Steps);

                return new StepsHistoryData
                {
                    MaxSteps = maxDistance,
                    TotalSteps = totalDistance
                };
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return new StepsHistoryData
            {
                MaxSteps = 0.00,
                TotalSteps = 0.00
            };
        }

        public async Task<bool> UpdateRoutineWorkoutDate(int routineId)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(Routine);
                var sql = $"UPDATE {nameof(Routine)} SET {nameof(Routine.DateLastUpdated)} = {DateTime.Today} WHERE {nameof(Routine.ID)} = {routineId}";
                return await _sQLiteDatastoreService.ExecuteAsync(sql);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }
        
        
        public async Task<bool> UpdateExerciseWorkoutDate(int exerciseId)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(Exercise);
                var sql = $"UPDATE {nameof(Exercise)} SET {nameof(Exercise.DateLastUpdated)} = {DateTime.Today} WHERE {nameof(Exercise.ID)} = {exerciseId}";
                return await _sQLiteDatastoreService.ExecuteAsync(sql);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<bool> DeleteExercise(Exercise exercise)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(RoutineExercise);
                Func<RoutineExercise, bool> routineExerciseCondition = x => x.ExerciseID == exercise.ID;
                List<RoutineExercise> routineExercises = await _sQLiteDatastoreService.GetRecordsWithCondition(routineExerciseCondition, null, null);
                List<RoutineExercise> updatedRoutineExercises = new List<RoutineExercise>();
                foreach (var re in routineExercises)                
                    updatedRoutineExercises.AddRange(await ReorderRoutine(re));
                
                _table = nameof(ExerciseLog);
                Func<ExerciseLog, bool> exerciseLogCondition = x => x.ExerciseID == exercise.ID;
                List<ExerciseLog> exerciseLogs = await _sQLiteDatastoreService.GetRecordsWithCondition(exerciseLogCondition, null, null);
                
                _table = nameof(RoutineExercise);
                if (routineExercises.Any())
                    await _sQLiteDatastoreService.DeleteAsync(routineExercises);

                if (updatedRoutineExercises.Any())
                    await _sQLiteDatastoreService.InsertOrReplaceResults(updatedRoutineExercises);

                _table = nameof(ExerciseLog);
                if (exerciseLogs.Any())
                    await _sQLiteDatastoreService.DeleteAsync(exerciseLogs);

                _table = nameof(Exercise);
                return await _sQLiteDatastoreService.DeleteAsync(new List<object> { exercise });
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        public async Task<bool> DeleteRoutine(Routine routine)
        {
            if (!DatabaseIsIntact())
                return false;

            try
            {
                _table = nameof(RoutineExercise);
                Func<RoutineExercise, bool> routineExerciseCondition = x => x.RoutineID == routine.ID;
                List<RoutineExercise> routineExercises = await _sQLiteDatastoreService.GetRecordsWithCondition(routineExerciseCondition, null, null);

                _table = nameof(ExerciseLog);
                List<ExerciseLog> deleteExerciseLogs = new List<ExerciseLog>();
                foreach (var re in routineExercises)
                {
                    Func<ExerciseLog, bool> exerciseLogCondition = x => (x.RoutineExerciseID == re.ID);
                    deleteExerciseLogs.AddRange(await _sQLiteDatastoreService.GetRecordsWithCondition(exerciseLogCondition, null, null));
                }

                _table = nameof(ExerciseLog);
                if (deleteExerciseLogs.Any())
                    await _sQLiteDatastoreService.DeleteAsync(deleteExerciseLogs);

                _table = nameof(RoutineExercise);
                if (routineExercises.Any())
                    await _sQLiteDatastoreService.DeleteAsync(routineExercises);
                
                _table = nameof(Routine);
                return await _sQLiteDatastoreService.DeleteAsync(new List<object> { routine });
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return false;
            }
        }

        private async Task<List<RoutineExercise>> ReorderRoutine(RoutineExercise routineExercise)
        {
            if (!DatabaseIsIntact())
                return new List<RoutineExercise>();

            try
            {
                _table = nameof(RoutineExercise);
                Func<RoutineExercise, bool> routineExerciseCondition = x => x.RoutineID == routineExercise.RoutineID;
                List<RoutineExercise> routineExercises = await _sQLiteDatastoreService.GetRecordsWithCondition(routineExerciseCondition, null, null);

                List<RoutineExercise> updates = new List<RoutineExercise>();
                foreach (var re in routineExercises)
                {
                    if (re.Order > routineExercise.Order)
                    {
                        re.Order -= 1;
                        updates.Add(re);
                    }
                }

                return updates;
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
                return new List<RoutineExercise>();
            }
        }

        public async Task<List<Filter>> ReadExerciseFilters()
        {
            if (!DatabaseIsIntact())
                return new List<Filter>();

            List<Filter> filters = new List<Filter>
            {
                new Filter { FilterID = 0, FilterName = "All" },
                new Filter { FilterID = 1, FilterName = "Favorites" }
            };

            try
            {
                List<ExerciseType> types = await ReadAllRecordsOfType<ExerciseType>();                
                filters.AddRange(types.Select(type => new Filter { FilterID = type.ID, FilterName = type.Name }));
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }

            return filters;
        }

        public async Task<List<Filter>> ReadRoutineFilters()
        {
            if (!DatabaseIsIntact())
                return new List<Filter>();

            List<Filter> filters = new List<Filter>
            {
                new Filter { FilterID = 0, FilterName = "All" },
                new Filter { FilterID = 1, FilterName = "Favorites" }
            };

            //try
            //{
            //    List<RoutineType> types = await ReadAllRecordsOfType<RoutineType>();
            //    filters.AddRange(types.Select(type => new Filter { FilterID = type.ID, FilterName = type.Name }));
            //}
            //catch (Exception ex)
            //{
            //    TrackEvent(ex);
            //}

            return filters;
        }

        private async Task PopulateExercise(Exercise exercise, int? routineExerciseID = null)
        {
            if (!DatabaseIsIntact())
                return;

            try
            {
                Func<EquipmentType, bool> equipmentTypeCondition = x => x.ID == exercise.EquipmentTypeID;
                exercise.EquipmentType = await _sQLiteDatastoreService.GetRecordWithCondition(equipmentTypeCondition);

                Func<ExerciseType, bool> exerciseTypeCondition = x => x.ID == exercise.ExerciseTypeID;
                exercise.ExerciseType = await _sQLiteDatastoreService.GetRecordWithCondition(exerciseTypeCondition);

                Func<BodyAreaType, bool> bodyAreaTypeCondition = x => x.ID == exercise.BodyAreaTypeID;
                exercise.BodyAreaType = await _sQLiteDatastoreService.GetRecordWithCondition(bodyAreaTypeCondition);

                Func<Level, bool> levelCondition = x => x.ID == exercise.LevelID;
                exercise.Level = await _sQLiteDatastoreService.GetRecordWithCondition(levelCondition);
                
                if (routineExerciseID.HasValue)                
                    await ReadLastSet(exercise, routineExerciseID);                
                else
                    await ReadLastSet(exercise);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }
        }

        private async Task PopulateExerciseLog(ExerciseLog exerciseLog)
        {
            if (!DatabaseIsIntact())
                return;

            try
            {
                //Func<Exercise, bool> exerciseCondition = x => x.ID == exerciseLog.ExerciseID;
                //exerciseLog.Exercise = await _sQLiteDatastoreService.GetRecordWithCondition(exerciseCondition);

                //Func<RoutineExercise, bool> routineExerciseCondition = x => x.ID == exerciseLog.RoutineExerciseID;
                //exerciseLog.RoutineExercise = await _sQLiteDatastoreService.GetRecordWithCondition(routineExerciseCondition);

                //Func<Routine, bool> routineCondition = x => x.ID == exerciseLog.RoutineID;
                //exerciseLog.Routine = await _sQLiteDatastoreService.GetRecordWithCondition(routineCondition);
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }
        }

        private async Task PopulateUserMedals(ASWTUser user)
        {
            if (!DatabaseIsIntact())
                return;

            try
            {
                Func<UserMedal, bool> medalsCondition = x => x.ID == user.UserID;
                var userMedals = await _sQLiteDatastoreService.GetRecordsWithCondition(medalsCondition, null, null);

                foreach (var userMedal in userMedals)
                {
                    Func<Medal, bool> medalCondition = x => x.ID == userMedal.MedalID;
                    userMedal.Medal = await _sQLiteDatastoreService.GetRecordWithCondition(medalCondition);
                }

                user.GoldMedals = userMedals.Where(x => x.Medal.Rank == MedalRank.Gold).ToList();
                user.SilverMedals = userMedals.Where(x => x.Medal.Rank == MedalRank.Silver).ToList();
                user.BronzeMedals = userMedals.Where(x => x.Medal.Rank == MedalRank.Bronze).ToList();
            }
            catch (Exception ex)
            {
                TrackEvent(ex);
            }
        }
    }
}


