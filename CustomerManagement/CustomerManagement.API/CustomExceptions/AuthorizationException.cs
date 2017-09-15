using System;

namespace CustomerManagement.API.CustomExceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string s) : base(s)
        {
            
        }
    }
}