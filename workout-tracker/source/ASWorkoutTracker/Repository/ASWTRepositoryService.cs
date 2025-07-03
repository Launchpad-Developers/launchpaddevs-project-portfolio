using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ASWorkoutTracker.Repository
{
    public class ASWTRepositoryService : IASWTRepositoryService
    {
        protected IFirebaseRepositoryService _firebaseRepositoryService;
        protected IRESTRepositoryService _restRepositoryService;
        private Dictionary<string, string> _firebaseKeys;

        public ASWTRepositoryService(IFirebaseRepositoryService firebaseRepositoryService,
                                     IRESTRepositoryService restRepositoryService,
                                     IJsonConsumerService jsonConsumerService)
        {
            _firebaseRepositoryService = firebaseRepositoryService;
            _restRepositoryService = restRepositoryService;
            _firebaseKeys = jsonConsumerService.GetSettingsDictionary("firebase_keys");
        }

        public async Task<DatabaseModel> GetAllSystemData()
        {
            DatabaseModel data = new DatabaseModel();
            var json = await _firebaseRepositoryService.GetFullDatabaseJson();
            if (!string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<DatabaseModel>(json);

            return data;
        }

        public async Task<List<Exercise>> GetExercises()
        {
            List<Exercise> list = new List<Exercise>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["Exercises"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<Exercise>>(json);

            //Dictionary<string, Exercise> entryDict = JsonConvert.DeserializeObject<Dictionary<string, Exercise>>(json);
            //list = entryDict.Select(x => x.Value).ToList();

            return list;
        }

        public async Task<List<Routine>> GetRoutines()
        {
            List<Routine> list = new List<Routine>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["Routines"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<Routine>>(json);

            return list;
        }

        public async Task<List<ExerciseType>> GetExerciseTypes()
        {
            List<ExerciseType> list = new List<ExerciseType>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["ExerciseTypes"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<ExerciseType>>(json);

            return list;
        }

        public async Task<List<BodyAreaType>> GetBodyAreaTypes()
        {
            List<BodyAreaType> list = new List<BodyAreaType>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["BodyAreaTypes"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<BodyAreaType>>(json);

            return list;
        }

        public async Task<List<RoutineExercise>> GetRoutineExercises()
        {
            List<RoutineExercise> list = new List<RoutineExercise>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["RoutineExercises"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<RoutineExercise>>(json);

            return list;
        }

        public async Task<List<EquipmentType>> GetEquipmentTypes()
        {
            List<EquipmentType> list = new List<EquipmentType>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["EquipmentTypes"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<EquipmentType>>(json);

            return list;
        }

        public async Task<List<Level>> GetLevels()
        {
            List<Level> list = new List<Level>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["Levels"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<Level>>(json);

            return list;
        }

        //public async Task<List<RoutineType>> GetRoutineTypes()
        //{
        //    List<RoutineType> list = new List<RoutineType>();
        //    var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["RoutineTypes"]);
        //    if (!string.IsNullOrEmpty(json))
        //        list = JsonConvert.DeserializeObject<List<RoutineType>>(json);

        //    return list;
        //}

        public async Task<List<Medal>> GetMedals()
        {
            List<Medal> list = new List<Medal>();
            var json = await _firebaseRepositoryService.GetRemoteDataJson(_firebaseKeys["Medals"]);
            if (!string.IsNullOrEmpty(json))
                list = JsonConvert.DeserializeObject<List<Medal>>(json);

            return list;
        }

        public async Task<DatabaseInfoModel> GetRemoteDatabaseInfoJson()
        {
            DatabaseInfoModel data = new DatabaseInfoModel();
            var json = await _firebaseRepositoryService.GetRemoteDatabaseInfoJson();
            if (!string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<DatabaseInfoModel>(json);

            return data;
        }
    }
}
