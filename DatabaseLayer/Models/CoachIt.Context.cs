﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CoachItEntities : DbContext
    {
        public CoachItEntities()
            : base("name=CoachItEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<MetricClass> MetricClasses { get; set; }
        public virtual DbSet<MetricRecord> MetricRecords { get; set; }
        public virtual DbSet<MetricType> MetricTypes { get; set; }
        public virtual DbSet<MetricUnit> MetricUnits { get; set; }
        public virtual DbSet<webpages_Membership> webpages_Membership { get; set; }
        public virtual DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public virtual DbSet<webpages_Roles> webpages_Roles { get; set; }
        public virtual DbSet<webpages_Users> webpages_Users { get; set; }
        public virtual DbSet<WorkoutExerciseCategory> WorkoutExerciseCategories { get; set; }
        public virtual DbSet<WorkoutExercis> WorkoutExercises { get; set; }
        public virtual DbSet<WorkoutUser> WorkoutUsers { get; set; }
        public virtual DbSet<WorkoutExercisesLInk> WorkoutExercisesLInks { get; set; }
    }
}
