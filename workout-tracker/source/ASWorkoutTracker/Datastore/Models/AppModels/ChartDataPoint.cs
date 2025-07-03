using System;
namespace ASWorkoutTracker.Datastore.Models
{
    public class ChartDataPoint
    {
        public ChartDataPoint()
        {
        }

        //X Axis(Bottom - Horizontal)
        //Y Axis (Side - Vertical)
        public DateTime XBindingPath { get; set; }
        public double YBindingPath { get; set; }
    }
}
