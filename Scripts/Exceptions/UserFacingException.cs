using System;

namespace Voltage.Witches.Exceptions
{
    public class UserFacingException : WitchesException
    {
        public UserFacingException(string message) : base(message)
        {
        }
    }
}

