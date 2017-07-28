using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POTracker.Models
{
    public class RoleUserGroup
    {
        public string RoleUser { get; set; }
        public List<RoleUser> Users { get; set; }
    }
    public class RoleUser
    {
        public string Username { get; set; }
    }
}