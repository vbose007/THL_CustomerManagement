using System.Text.RegularExpressions;

namespace CustomerManagement.Data.Helpers
{
    public class EmailValidator
    {

        public static bool IsValid(string email)
        { 
            // An empty or null string is not valid
            if (string.IsNullOrEmpty(email))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(email))
                return (true);
            else
                return (false);
        }
    }
}