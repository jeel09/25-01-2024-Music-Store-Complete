using System;
using MvcMusicStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MvcMusicStore.CustomAuthentication
{
    public class CustomMembershipUser : MembershipUser
    {
        #region User Properties

        public int UserId { get; set; }
        public ICollection<Role> Roles { get; set; }

        #endregion

        public CustomMembershipUser(User user):base("CustomMembership", user.Username, user.Id, user.Email, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            UserId = user.Id;
            Roles = user.Roles;
        }
    }
}