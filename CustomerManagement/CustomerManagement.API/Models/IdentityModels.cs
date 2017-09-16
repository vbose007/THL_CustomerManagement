using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace CustomerManagement.API.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }


    public class ApplicationRole : IdentityRole//<String, ApplicationUserRole>
    {
        public ApplicationRole(string roleName)
        {
            this.Name = roleName;
        }

        public ApplicationRole() : base() { }

    }

    public class ApplicationRoleStore : RoleStore<IdentityRole>
    {
        
    }
    public class ApplicationUserRole : IdentityUserRole
    {
        
    }

    public class ApplicationDbContext : IdentityDbContext//<ApplicationUser, ApplicationRole, string, IdentityUserLogin, ApplicationUserRole, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base("name=ApplicationDbContext")
        {
            Database.SetInitializer(new AuthenticationDbInitializer());
            
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<ApplicationUserRole> UserRoles { get; set; }
    } 
    
}