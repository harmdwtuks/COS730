using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models.Workout
{
    public class WorkoutViewModel
    {
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutTitle { get; set; }
        public TimeSpan EstimatedDuration { get; set; }

        public List<Exercise> Exercises { get; set; }
    }
}