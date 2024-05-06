using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MvcMusicStore.Models
{
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] username, string[] roleNames)
        {
            using (MvcMusicStoreEntities _Context = new MvcMusicStoreEntities())
            {
                var user = _Context.Users.FirstOrDefault(u => u.Username == username.ToString());

                if (user != null)
                {
                    _Context.UserRoleMappings.AddRange(roleNames.Select(roleName =>
                       new UserRoleMapping
                       {
                           UserId = user.Id,
                           RoleId = (int)(_Context.Roles.FirstOrDefault(r => r.RoleName == roleName)?.Id)
                       }
                   ));

                    _Context.SaveChangesAsync();
                    //throw new NotImplementedException();
                }
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            using (MvcMusicStoreEntities _Context = new MvcMusicStoreEntities())
            {
                //var userRoles = (from User in _Context.UserRoleMappings select User.UserId.ToString()).ToArray();
                var userRoles = (from Users in _Context.Users
                                 join roleMapping in _Context.UserRoleMappings
                                 on Users.Id equals roleMapping.UserId
                                 join role in _Context.Roles
                                 on roleMapping.RoleId equals role.Id
                                 where Users.Username == username
                                 select role.RoleName).ToArray();
                return userRoles;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}