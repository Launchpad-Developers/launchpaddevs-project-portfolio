using System;
using ASCommonServices.Models;

namespace ASWorkoutTracker.Datastore.Models
{
    public class EquipmentType : BaseSQLModel
    {
        public string WebViewDetail { get { return $"<strong>Equipment:</strong>   {Name}<br />"; } }
    }
}
