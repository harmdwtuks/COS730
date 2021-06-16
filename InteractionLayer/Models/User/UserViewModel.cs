using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models.User
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
    }
}