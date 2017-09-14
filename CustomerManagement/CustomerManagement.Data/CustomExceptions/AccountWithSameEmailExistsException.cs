using System;

namespace CustomerManagement.Data.CustomExceptions
{
    public class AccountWithSameEmailExistsException : Exception
    {
        public AccountWithSameEmailExistsException(string s) : base(s)
        {
        }
    }
}