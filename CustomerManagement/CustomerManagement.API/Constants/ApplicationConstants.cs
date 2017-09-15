using System.Configuration;

namespace CustomerManagement.API.Constants
{
    public class ApplicationConstants
    {
        public static readonly int DefaultPageSize = 10;
        public static readonly int DefaultPageNumber = 1;
        public static readonly string DefaulAdminUserName;
        public static readonly string DefaulAdminPassword;
        public static readonly string DefaulUserName;
        public static readonly string DefaulUserPassword;

        static ApplicationConstants()
        {
            int defaultPageSize;
            int.TryParse(ConfigurationManager.AppSettings.Get("DefaultPageSize"), out defaultPageSize);
            if (defaultPageSize > 0)
            {
                DefaultPageSize = defaultPageSize;
            }
            int defaultPageNumber;
            int.TryParse(ConfigurationManager.AppSettings.Get("DefaultPageNumber"), out defaultPageNumber);
            if (defaultPageNumber > 0)
            {
                DefaultPageNumber = defaultPageNumber;
            }

            DefaulAdminUserName = ConfigurationManager.AppSettings.Get("DefaulAdminUserName");
            DefaulAdminPassword = ConfigurationManager.AppSettings.Get("DefaulAdminPassword");
            DefaulUserName = ConfigurationManager.AppSettings.Get("DefaulUserName");
            DefaulUserPassword = ConfigurationManager.AppSettings.Get("DefaulUserPassword");
        }
    }
}