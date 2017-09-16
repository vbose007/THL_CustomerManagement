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
        }
    }
}
