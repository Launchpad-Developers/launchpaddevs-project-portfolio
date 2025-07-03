using System;
using System.Collections.Generic;
using System.Linq;
using ASCommonServices.Models;
using ASWorkoutTracker.Helpers;
using Syncfusion.SfChart.XForms;
using Xamarin.Essentials;

namespace ASWorkoutTracker.Datastore.Models
{
    public class ChartData
    {
        public ChartData(List<Grouping<DateTime, ExerciseLog>> dataSource, RoutineExercise routineExercise)
        {
            XAxisLabel = "Workout Date";
            DataPoints = new List<ChartDataPoint>();

            double min = 0;
            double max = 0;

            if (routineExercise.RecordsWeight && routineExercise.RecordsReps)
            {
                ChartTitle = "One Rep Max";
                LineLabel = "One Rep Max";
                YAxisLabel = $"One Rep Max ({Preferences.Get(AppConstants.App.WEIGHT_SYSTEM, AppConstants.App.DEFAULT_WEIGHT_SYSTEM)})";
                ShowTrendLine = true;
                ShowDataMarkers = true;
                TrendlineType = ChartTrendlineType.Linear;
                TrendLineLabel = "One Rep Max Trend";
                ForwardForecast = 3;

                foreach (var group in dataSource)
                {
                    var maxWeight = group.PublicItems.Max(x => x.Weight);
                    var maxReps = group.PublicItems.Where(x => x.Weight == maxWeight).Max(x => x.Reps);
                    if (maxWeight > 0 && maxReps > 0)
                    {
                        var oneRepMax = WeightLiftingHelper.CalculateOneRepMax((int)maxWeight, maxReps);
                        min = oneRepMax < min || min == 0 ? oneRepMax : min;
                        max = oneRepMax > max ? oneRepMax : max;

                        DataPoints.Add(new ChartDataPoint
                        {
                            YBindingPath = oneRepMax,
                            XBindingPath = group.Key.Date
                        });
                    }
                }
            }
            else if (routineExercise.RecordsDistance)
            {
                ChartTitle = "Distance";
                LineLabel = "Distance";
                YAxisLabel = $"Distance ({Preferences.Get(AppConstants.App.DISTANCE_SYSTEM, AppConstants.App.DEFAULT_DISTANCE_SYSTEM)})";
                ShowTrendLine = false;
                ShowDataMarkers = true;

                foreach (var group in dataSource)
                {
                    var maxDistance = group.PublicItems.Max(x => x.Distance);
                    if (maxDistance > 0)
                    {
                        min = maxDistance < min || min == 0 ? maxDistance : min;
                        max = maxDistance > max ? maxDistance : max;

                        DataPoints.Add(new ChartDataPoint
                        {
                            XBindingPath = group.Key.Date,
                            YBindingPath = maxDistance
                        });
                    }
                }
            }
            else if (routineExercise.RecordsElevation)
            {                
                ChartTitle = "Elevation";
                LineLabel = "Elevation";
                YAxisLabel = $"Elevation ({Preferences.Get(AppConstants.App.ELEVATION_SYSTEM, AppConstants.App.DEFAULT_ELEVATION_SYSTEM)})";
                ShowTrendLine = false;
                ShowDataMarkers = true;

                foreach (var group in dataSource)
                {
                    var maxDistance = group.PublicItems.Max(x => x.Distance);
                    if (maxDistance > 0)
                    {
                        min = maxDistance < min || min == 0 ? maxDistance : min;
                        max = maxDistance > max ? maxDistance : max;

                        DataPoints.Add(new ChartDataPoint
                        {
                            XBindingPath = group.Key.Date,
                            YBindingPath = maxDistance
                        });
                    }
                }
            }
            else if (routineExercise.RecordsTime)
            {
                ChartTitle = "Time";
                LineLabel = "Time";
                YAxisLabel = "Total Minutes";
                ShowTrendLine = false;
                ShowDataMarkers = true;
                YInterval = 5;
                var minT = TimeSpan.Zero;
                var maxT = TimeSpan.Zero;

                foreach (var group in dataSource)
                {
                    var maxTime = group.PublicItems.Max(x => x.Time);
                    var minTime = group.PublicItems.Min(x => x.Time);
                    if (maxTime > TimeSpan.Zero && minTime > TimeSpan.Zero)
                    {
                        minT = minTime < minT || minT == TimeSpan.Zero ? minTime : minT;
                        maxT = maxTime > maxT ? maxTime : maxT;

                        DataPoints.Add(new ChartDataPoint
                        {
                            XBindingPath = group.Key.Date,
                            YBindingPath = maxTime.TotalMinutes
                        });
                    }
                }

                min = minT.TotalMinutes;
                max = maxT.TotalMinutes;
            }
            else if (routineExercise.RecordsSteps)
            {
                ChartTitle = "Steps";
                LineLabel = "Steps";
                YAxisLabel = "Total Steps";
                ShowTrendLine = false;
                ShowDataMarkers = true;
                YInterval = 1000;

                foreach (var group in dataSource)
                {
                    var maxSteps = group.PublicItems.Max(x => x.Steps);
                    if (maxSteps > 0)
                    {
                        min = maxSteps < min || min == 0 ? maxSteps : min;
                        max = maxSteps > max ? maxSteps : max;

                        DataPoints.Add(new ChartDataPoint
                        {
                            XBindingPath = group.Key.Date,
                            YBindingPath = maxSteps
                        });
                    }
                }
            }

            YMaximum = (int)(max * 1.1);
            YMinimum = (int)(min * .9);
            YInterval = (int)((YMaximum - YMinimum) * .05);

            if (YMaximum > 0)
                ChartGradientStop = YMinimum / YMaximum;
            else
                NoData = true;
        }

        public bool NoData { get; set; }

        public string ChartTitle { get; set; }
        public string LineLabel { get; set; }
        public bool ShowDataMarkers { get; set; }

        public bool ShowTrendLine { get; set; }
        public string TrendLineLabel { get; set; }
        public ChartTrendlineType TrendlineType { get; set; }
        public int ForwardForecast { get; set; }
        public double ChartGradientStop { get; set; }

        //X Axis (Bottom - Horizontal)
        //Y Axis (Side - Vertical)
        public string XAxisLabel { get; set; }

        public string YAxisLabel { get; set; }
        public int YMinimum { get; set; }
        public int YMaximum { get; set; }
        public int YInterval { get; set; }


        public List<ChartDataPoint> DataPoints { get; set; }
    }
}
