using System;
using ASCommonServices.Models;

namespace ASWorkoutTracker.Datastore.Models
{
    public class BodyAreaType : BaseSQLModel
    {
        public string WebViewDetail { get { return $"<strong>Targets:</strong>     {Name}<br />"; } }
    }
}
