using System;

namespace CustomerManagement.API.CustomExceptions
{
    public class BadRequestException: Exception
    {
        public BadRequestException(string msg):base(msg)
        {
            
        }
    }
}