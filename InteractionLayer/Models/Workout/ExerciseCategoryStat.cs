using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models.Workout
{
    public class ExerciseCategoryStat
    {
        public string Category { get; set; }
        public int NumExercises { get; set; }
        public string GraphSectionColor { get; set; }
    }
}