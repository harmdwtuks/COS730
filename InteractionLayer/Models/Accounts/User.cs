using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models.Accounts
{
    public class User
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string FullNames { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public byte[] ProfilePhoto { get; set; }

        public int RoleId { get; set; }
        public int[] TeamIds { get; set; }

        public List<Role> Roles { get; set; }
        public List<Team> Teams { get; set; }
    }
    
}