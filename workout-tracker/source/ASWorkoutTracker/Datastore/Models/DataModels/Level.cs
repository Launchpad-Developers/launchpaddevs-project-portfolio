using System;
using ASCommonServices.Models;

namespace ASWorkoutTracker.Datastore.Models
{
    public class Level : BaseSQLModel
    {
        public string WebViewDetail { get { return $"<strong>Level:</strong>       {Name}<br />"; } }
    }
}
