//using System;
//using System.Linq;
//using System.Web.Security;
//using CustomerManagement.API.Models;
//using Microsoft.ApplicationInsights.Extensibility.Implementation;

//namespace CustomerManagement.API.Providers
//{
//    public class ApplicationRoleProvider : SqlRoleProvider
//    {
//        //public override string[] GetUsersInRole(string roleName)
//        //{
//        //    using (var userContext = new ApplicationDbContext())
//        //    {
//        //        var role = userContext.Roles.SingleOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

//        //        userContext.Users.Select(x =>  x.Roles)
//        //        return userContext.Roles.Select(r => r.Name).ToArray();
//        //    }
//        //}

//        //public override string[] GetAllRoles()
//        //    {
//        //        using (var userContext = new ApplicationDbContext())
//        //        {
//        //            return userContext.Roles.Select(r => r.Name).ToArray();
//        //        }
//        //    }

//        //public override string[] FindUsersInRole(string roleName, string usernameToMatch)
//        //{
//        //    throw new System.NotImplementedException();
//        //}


//        public override string[] GetRolesForUser(string username)
//        {
//            using (var userContext = new ApplicationDbContext())
//            {
//                var user = userContext.Users.SingleOrDefault(u => u.UserName == username);


//                if (user != null)
//                {
//                    if (user.Roles == null) return new string[] {};

//                    var uRoles = from uRole in user.Roles
//                                 join role in userContext.Roles
//                                     on uRole.RoleId equals role.Id
//                                 select role.Name;

//                    return uRoles.ToArray();
//                }

//                return new string[] { };
//            }
//        }

//        public override bool IsUserInRole(string username, string roleName)
//        {
//            using (var userContext = new ApplicationDbContext())
//            {
//                var user = userContext.Users.SingleOrDefault(u => u.UserName == username);

//                if (user != null)
//                {
//                    if (user.Roles == null) return false;

//                    var uRoles = from uRole in user.Roles
//                                 join role in userContext.Roles
//                                     on uRole.RoleId equals role.Id
//                                 select role.Name;

//                    return uRoles.Contains(roleName);
//                }

//                return false;
//            }
//        }

//    }
//}

