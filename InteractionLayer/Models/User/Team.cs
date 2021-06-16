using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models.User
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public DateTime Timestamp { get; set; }
        public int CreatorId { get; set; }
        public string Creator { get; set; }
        public List<UserViewModel> TeamMembers { get; set; }
    }
}