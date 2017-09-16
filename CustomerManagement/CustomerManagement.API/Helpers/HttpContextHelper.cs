using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CustomerManagement.API.Helpers
{
    public class HttpContextHelper : IHttpContextHelper
    {
        public string Scheme
        {
            get { return HttpContext.Current.Request.Url.Scheme; }
        }

        public string AbsoluteUri { get { return HttpContext.Current.Request.Url.AbsoluteUri; } }
        public string SchemeDelimiter { get { return Uri.SchemeDelimiter; }}

        public string CurrentUserName { get { return HttpContext.Current.User.Identity.GetUserName(); } }
        public bool IsCurrentUserRole(string role)
        {
            var userManager = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();

            var user = userManager.FindByName(CurrentUserName);

            if (user == null) return false;

            return userManager.IsUerInRole(user.Id, role);
        }

        public string Host
        {
            get { return HttpContext.Current.Request.Url.Host; }
        }

        public int Port
        {
            get { return HttpContext.Current.Request.Url.Port; }
        }
        public string Path
        {
            get { return HttpContext.Current.Request.Url.AbsolutePath; }
        }
    }
}