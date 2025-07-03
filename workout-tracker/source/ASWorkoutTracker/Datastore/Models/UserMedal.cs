using System;
using ASCommonServices.Models;
using SQLite;

namespace ASWorkoutTracker.Datastore.Models
{
    public class UserMedal : BaseSQLModel
    {
        public int UserID { get; set; }
        public int MedalID { get; set; }
        public DateTime DateAchieved { get; set; }

        [Ignore]
        public Medal Medal { get; set; }
    }
}
