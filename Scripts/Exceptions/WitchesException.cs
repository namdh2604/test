using System;

namespace Voltage.Witches.Exceptions
{
    public class WitchesException : Exception
    {
        private const string EXCEPTION_PREFIX = "WitchesException: ";

        public WitchesException(string message) : base(message)
        {
        }
    }
}

