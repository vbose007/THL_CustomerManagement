using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;
using CustomerManagement.API.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace CustomerManagement.API.Models
{
    public class AuthenticationDbInitializer:DropCreateDatabaseAlways<ApplicationDbContext>
    {

        protected  override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));



            if (!roleManager.RoleExists(RolesEnum.Admin.ToString()))
            {
                var roleresult = roleManager.Create(new ApplicationRole(RolesEnum.Admin.ToString()));
            }

            if (!roleManager.RoleExists(RolesEnum.User.ToString()))

            {

                var roleresult = roleManager.Create(new ApplicationRole(RolesEnum.User.ToString()));

            }

            //Create default admin user
            var userName = ApplicationConstants.DefaulAdminUserName;
            var password = ApplicationConstants.DefaulAdminPassword;
            var role = RolesEnum.Admin.ToString();

            CreateUser(userManager, userName, password, role);


            //Create default non-admin user
            userName = ApplicationConstants.DefaulUserName;
            password = ApplicationConstants.DefaulUserPassword;
            role = RolesEnum.User.ToString();

            CreateUser(userManager, userName, password, role);
        }

        private static void CreateUser(ApplicationUserManager userManager, string userName, string password, string role)
        {
            var user = userManager.FindByName(userName);

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };

                var userResult = userManager.Create(user, password);
                var roleResult = userManager.AddToRole(user.Id, role);
            }
        }
    }
}