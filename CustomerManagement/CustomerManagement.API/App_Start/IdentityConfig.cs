using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using CustomerManagement.API.Models;
using Microsoft.Owin.Security;

namespace CustomerManagement.API
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public bool AddToRole(string userId, string role)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var roleModel = ctx.Roles.SingleOrDefault(r => r.Name.Equals(role, StringComparison.OrdinalIgnoreCase));
                var user = ctx.Users.SingleOrDefault(u => u.Id.Equals(userId));

                if (user == null)
                {
                    throw new Exception($"Invalid userId: {userId}");
                }

                if (roleModel == null)
                {
                    throw new Exception($"Invalid Role Name: {role}");
                }

                if (IsUerInRole(user.Id, roleModel.Name))
                {
                    throw new Exception($"User Id {userId} is already has role: {role}");
                }
                var userRole = new ApplicationUserRole()
                {
                    RoleId = roleModel.Id,
                    UserId = userId
                };

                ctx.UserRoles.Add(userRole);
                ctx.SaveChanges();
                return true;
            }
        }

        public bool IsUerInRole(string userId, string roleName)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var role = ctx.Roles.SingleOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
                var user = ctx.Users.SingleOrDefault(r => r.Id.Equals(userId));
                
                if (role != null)
                {
                    return ctx.UserRoles.Any(x => x.UserId.Equals(user.Id) && x.RoleId.Equals(role.Id));
                }

                return false;
            }
        }

    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, AuthenticationType);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store)
        {
        }

        //public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        //{
        //    var applicationRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
            
        //    return applicationRoleManager;
        //}
    }

}
