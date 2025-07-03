using System;
using ASCommonServices.Models;
using ASWorkoutTracker.Enums;

namespace ASWorkoutTracker.Datastore.Models
{
    public class Medal : BaseSQLModel
    {
        public MedalRank Rank { get; set; }
    }
}
