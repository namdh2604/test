using System;

namespace Voltage.Witches.Exceptions
{
    public class EnvironmentMissingException : UserFacingException
    {
        public EnvironmentMissingException() : base("Could not retrieve server environment")
        {
        }
    }
}

