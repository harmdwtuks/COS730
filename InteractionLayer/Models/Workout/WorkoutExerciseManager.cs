using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models
{
    public class WorkoutExerciseManager
    {
        public List<WorkoutCategory> WorkoutCategories { get; set; }
        public List<WorkoutExercise> CurrentExercises { get; set; }
        public int RowNumber { get; set; }
    }
}