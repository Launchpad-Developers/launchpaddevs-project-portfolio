using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASCommonServices.Models;
using ASWorkoutTracker.Datastore.Models;

namespace ASWorkoutTracker.Repository
{
    public interface IASWTRepositoryService
    {
        Task<DatabaseModel> GetAllSystemData();
        Task<List<Exercise>> GetExercises();
        Task<List<Routine>> GetRoutines();
        Task<List<ExerciseType>> GetExerciseTypes();
        Task<List<BodyAreaType>> GetBodyAreaTypes();
        Task<List<RoutineExercise>> GetRoutineExercises();
        Task<List<EquipmentType>> GetEquipmentTypes();
        Task<List<Level>> GetLevels();
        Task<List<Medal>> GetMedals();
        Task<DatabaseInfoModel> GetRemoteDatabaseInfoJson();

        //Future versions
        //Task<var> GetAllUserData();
        //Task<List<CustomExercise>> GetCustomExercises();
        //Task<List<ExerciseLog>> GetExerciseLogs();
        //Task<List<Routine>> GetRoutines();
        //Task<List<RoutineExercise>> GetRoutineExercises();
    }
}
