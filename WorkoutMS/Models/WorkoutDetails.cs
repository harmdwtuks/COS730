using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkoutMS.Models
{
    public class WorkoutDetails
    {
        public string WorkoutTitle { get; set; }
        public TimeSpan EstimatedDuration { get; set; }

        public List<Exercise> Exercises { get; set; }
    }
}