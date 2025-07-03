using System;
namespace ASWorkoutTracker.Helpers
{
    public static class WeightLiftingHelper
    {
        public static int CalculateOneRepMax(int weight, int reps)
        {
            //https://en.wikipedia.org/wiki/One-repetition_maximum
            //Epley
            //1RM = w (1 + (r/30)), assumes r > 1
            //_oneRepMax = (int)(max.Weight * (max.Reps/30));

            //Brzycki
            //1RM = w / (1.0278 - 0.0278r), assumes r >= 1
            return (int)(weight / (1.0278 - (0.0278 * reps)));
        }
    }
}
