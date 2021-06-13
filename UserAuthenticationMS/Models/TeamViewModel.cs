using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserAuthenticationMS.Models
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public DateTime Timestamp { get; set; }
        public int CreatorId { get; set; }
        public string Creator { get; set; }
        public List<UserViewModel> TeamMembers { get; set; }
    }
}