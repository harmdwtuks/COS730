using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkoutMS.Models
{
    public class WorkoutExercise
    {
        public int WorkoutCategoryId { get; set; }
        public string WorkoutCategory { get; set; }
        public int ExerciseId { get; set; }
        public string Exercise { get; set; }
        public string Instructions { get; set; }
    }
}