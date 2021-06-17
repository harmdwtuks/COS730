//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseLayer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkoutExercisesLInk
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkoutExercisesLInk()
        {
            this.WorkoutExerciseLinkCompleteds = new HashSet<WorkoutExerciseLinkCompleted>();
        }
    
        public int Id { get; set; }
        public int WorkoutUsersId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Repititions { get; set; }
        public System.TimeSpan Duration { get; set; }
        public double Weight { get; set; }
    
        public virtual WorkoutExercis WorkoutExercis { get; set; }
        public virtual WorkoutUser WorkoutUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkoutExerciseLinkCompleted> WorkoutExerciseLinkCompleteds { get; set; }
    }
}
