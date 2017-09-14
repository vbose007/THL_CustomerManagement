using System;
using System.Collections.Generic;
using System.Linq;
using CustomerManagement.API.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CustomerManagement.API.Startup))]

namespace CustomerManagement.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            using (var context = new ApplicationDbContext())
            {
                AuthenticationDbInitializer.Seed(context);
            }
        }
    }
}
