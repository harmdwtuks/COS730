using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseLayer.Models.Workout
{
    public class Exercise
    {
        public int Id { get; set; }
        public int WorkoutCategoryId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public int Repititions { get; set; }
        public double Weight { get; set; }
        public TimeSpan Duration { get; set; }
    }
}