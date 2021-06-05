using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UserAuthenticationMS.Models
{
    public sealed class UsersContext : DbContext
    {
        public UsersContext()
            : base("CoachItEntities")
        {
        }

        public DbSet<webpages_Users> UserProfiles { get; set; }
    }

}