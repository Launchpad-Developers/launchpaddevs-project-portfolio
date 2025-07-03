using System;
using ASCommonServices.Models;
using SQLite;

namespace ASWorkoutTracker.Datastore.Models
{
    public class ExerciseType : BaseSQLModel
    {
        [Ignore]
        public string VmNameLabel { get { return Name.ToUpper(); } }
    }
}
